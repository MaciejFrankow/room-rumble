using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyFly : MonoBehaviour
{
    private Rigidbody rb;                    // Referencja do komponentu Rigidbody, pozwala na manipulowanie fizyk¹ obiektu
    private Transform player;                // Transform gracza (pozycja, rotacja itp.)
    public LayerMask whatIsGround, whatIsPlayer;
    // Maski warstw, które pozwalaj¹ okreœlaæ, co jest "ziemi¹" (do patrolowania, sprawdzania kolizji z terenem) i co jest "graczem"

    private Vector3 destination;             // Docelowy punkt, do którego ma siê poruszyæ obiekt
    private Vector3 startPosition;           // Pocz¹tkowa pozycja obiektu (do wyznaczania patrolu wokó³ pozycji startowej)
    private Vector3 walkPoint;               // Punkt, do którego patroluje przeciwnik
    private bool _walkPointSet;              // Informacja, czy punkt patrolowania zosta³ ustalony
    public float walkPointRange;             // Zasiêg losowania punktu patrolu w ka¿d¹ stronê od bazowej pozycji

    public float timeBetweenAttacks;         // Czas (w sekundach) miêdzy kolejnymi atakami
    private bool _alreadyAttacked;           // Flaga informuj¹ca, czy przeciwnik ju¿ zaatakowa³ (aby ustawiæ cooldown miêdzy atakami)

    public float sightRange, attackRange;    // Zasiêg wykrywania gracza (wzrok) i zasiêg ataku
    private bool playerInSightRange, playerInAttackRange;
    // Flagi informuj¹ce, czy gracz jest w zasiêgu widzenia i ataku

    private float speed;                     // Obecna prêdkoœæ poruszania siê przeciwnika
    public float damage;                     // Iloœæ obra¿eñ zadawanych graczowi
    public float chaseSpeed;                 // Prêdkoœæ poruszania siê podczas poœcigu
    public float patrolSpeed;                // Prêdkoœæ poruszania siê podczas patrolu
    public float rotationSpeed;              // Prêdkoœæ, z jak¹ przeciwnik obraca siê w stronê docelowego punktu
    public bool patrolAroundStartOnly;       // Czy przeciwnik ma patrolowaæ wy³¹cznie wokó³ swojej pozycji startowej?

    public MovementState state;              // Aktualny stan ruchu przeciwnika (patroling / chasing / attacking)
    public enum MovementState
    {
        patroling,
        chasing,
        attacking
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();                        // Pobranie komponentu Rigidbody z obiektu
        state = MovementState.patroling;                       // Ustawienie domyœlnego stanu ruchu na "patroling"
        startPosition = transform.position;                    // Zapamiêtanie pozycji startowej obiektu
        player = GameObject.Find("PlayerCapsule").transform;   // Wyszukanie obiektu gracza i pobranie jego Transformu
    }

    private void Update()
    {
        // Sprawdzanie, czy gracz znajduje siê w zasiêgu wzroku lub ataku
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        // Jeœli gracz nie jest w zasiêgu wzroku i ataku, przejdŸ do patrolu
        if (!playerInSightRange && !playerInAttackRange)
        {
            Patroling();
            speed = patrolSpeed;
            state = MovementState.patroling;
        }
        // Jeœli gracz jest w zasiêgu wzroku, ale jeszcze nie w zasiêgu ataku, przejdŸ do poœcigu
        if (playerInSightRange && !playerInAttackRange)
        {
            ChasePlayer();
            speed = chaseSpeed;
            state = MovementState.chasing;
        }
        // Jeœli gracz jest w zasiêgu ataku, przejdŸ do atakowania
        if (playerInSightRange && playerInAttackRange)
        {
            AttackPlayer();
            state = MovementState.attacking;
        }
    }

    private void FixedUpdate()
    {
        // W FixedUpdate wykonywany jest faktyczny ruch (fizyka)
        MoveToPoint();
    }

    private void MoveToPoint()
    {
        // Jeœli przeciwnik atakuje, nie wykonujemy ruchu
        if (state is MovementState.attacking) return;

        // Oblicz kierunek w stronê celu
        Vector3 direction = (destination - transform.position).normalized;

        // Obróæ przeciwnika w stronê kierunku poruszania
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Poruszaj siê do przodu w kierunku, w którym jest obrócony obiekt
        Vector3 forward = transform.forward;
        rb.velocity = forward * rb.velocity.magnitude;
        // Utrzymanie dotychczasowej prêdkoœci, ale w aktualnie obróconym kierunku

        rb.AddForce(forward * speed, ForceMode.Force);
        // Dodanie si³y w kierunku forward, by rozpêdziæ przeciwnika

        // Ograniczenie prêdkoœci, ¿eby nie przekroczyæ zdefiniowanej "speed"
        if (rb.velocity.magnitude > speed)
        {
            rb.velocity = forward * speed;
        }
    }

    private void Patroling()
    {
        // Jeœli nie ma ustawionego punktu patrolu, wyszukaj nowy
        if (!_walkPointSet) SearchWalkPoint();

        // Jeœli mamy punkt patrolu, ustaw go jako cel
        if (_walkPointSet)
        {
            destination = walkPoint;
            speed = patrolSpeed;
        }

        // Oblicz dystans do obecnego punktu patrolu
        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // Jeœli przeciwnik zbli¿y³ siê wystarczaj¹co, ustal, ¿e trzeba znaleŸæ nowy punkt
        if (distanceToWalkPoint.magnitude < 1f)
        {
            _walkPointSet = false;
        }
    }

    private void SearchWalkPoint()
    {
        // Losowo wybierz przesuniêcie w zakresie walkPointRange
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        float randomY = Random.Range(-walkPointRange, walkPointRange);
        float randomZ = Random.Range(-walkPointRange, walkPointRange);

        // Jeœli patrol ma siê odbywaæ wokó³ pozycji startowej
        if (patrolAroundStartOnly)
        {
            walkPoint = new Vector3(startPosition.x + randomX, startPosition.y + randomY, startPosition.z + randomZ);
        }
        // W przeciwnym razie patroluj wokó³ aktualnej pozycji
        else
        {
            walkPoint = new Vector3(transform.position.x + randomX, transform.position.y + randomY, transform.position.z + randomZ);
        }

        // SprawdŸ, czy na drodze do wybranego punktu nie ma przeszkód (warstwa ziemi - whatIsGround)
        Vector3 rayCastDirection = (walkPoint - transform.position).normalized;
        float maxDistance = Vector3.Distance(transform.position, walkPoint) + 1;

        // Jeœli Raycast nie trafia w ziemiê, oznacza to ¿e punkt jest osi¹galny
        if (!Physics.Raycast(transform.position, rayCastDirection, maxDistance, whatIsGround))
        {
            _walkPointSet = true;
            Debug.Log("ustawiono");
        }
    }

    private void ChasePlayer()
    {
        // Pod¹¿aj za graczem, ustaw cel na pozycjê gracza
        destination = player.position;
    }

    private void AttackPlayer()
    {
        // Podczas ataku przeciwnik zatrzymuje siê w miejscu
        Health healthSystem = player.GetComponent<Health>(); // Odwo³anie do skryptu gracza z logik¹ zdrowia
        destination = transform.position;                    // Zatrzymaj siê w miejscu

        // Obróæ siê w stronê gracza
        transform.LookAt(player);

        // Jeœli przeciwnik nie atakowa³ w ostatnim czasie (cooldown)
        if (!_alreadyAttacked)
        {
            // Zadaj obra¿enia graczowi
            if (healthSystem != null)
            {
                healthSystem.TakeDamage(damage);
            }

            // Ustaw flagê i rozpocznij odliczanie do nastêpnego ataku
            _alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        // Reset flagi ataku, by pozwoliæ na kolejny atak
        _alreadyAttacked = false;
    }
}
