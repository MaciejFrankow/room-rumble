using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyFly : MonoBehaviour
{
    private Rigidbody rb;                    // Referencja do komponentu Rigidbody, pozwala na manipulowanie fizyk� obiektu
    private Transform player;                // Transform gracza (pozycja, rotacja itp.)
    public LayerMask whatIsGround, whatIsPlayer;
    // Maski warstw, kt�re pozwalaj� okre�la�, co jest "ziemi�" (do patrolowania, sprawdzania kolizji z terenem) i co jest "graczem"

    private Vector3 destination;             // Docelowy punkt, do kt�rego ma si� poruszy� obiekt
    private Vector3 startPosition;           // Pocz�tkowa pozycja obiektu (do wyznaczania patrolu wok� pozycji startowej)
    private Vector3 walkPoint;               // Punkt, do kt�rego patroluje przeciwnik
    private bool _walkPointSet;              // Informacja, czy punkt patrolowania zosta� ustalony
    public float walkPointRange;             // Zasi�g losowania punktu patrolu w ka�d� stron� od bazowej pozycji

    public float timeBetweenAttacks;         // Czas (w sekundach) mi�dzy kolejnymi atakami
    private bool _alreadyAttacked;           // Flaga informuj�ca, czy przeciwnik ju� zaatakowa� (aby ustawi� cooldown mi�dzy atakami)

    public float sightRange, attackRange;    // Zasi�g wykrywania gracza (wzrok) i zasi�g ataku
    private bool playerInSightRange, playerInAttackRange;
    // Flagi informuj�ce, czy gracz jest w zasi�gu widzenia i ataku

    private float speed;                     // Obecna pr�dko�� poruszania si� przeciwnika
    public float damage;                     // Ilo�� obra�e� zadawanych graczowi
    public float chaseSpeed;                 // Pr�dko�� poruszania si� podczas po�cigu
    public float patrolSpeed;                // Pr�dko�� poruszania si� podczas patrolu
    public float rotationSpeed;              // Pr�dko��, z jak� przeciwnik obraca si� w stron� docelowego punktu
    public bool patrolAroundStartOnly;       // Czy przeciwnik ma patrolowa� wy��cznie wok� swojej pozycji startowej?

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
        state = MovementState.patroling;                       // Ustawienie domy�lnego stanu ruchu na "patroling"
        startPosition = transform.position;                    // Zapami�tanie pozycji startowej obiektu
        player = GameObject.Find("PlayerCapsule").transform;   // Wyszukanie obiektu gracza i pobranie jego Transformu
    }

    private void Update()
    {
        // Sprawdzanie, czy gracz znajduje si� w zasi�gu wzroku lub ataku
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        // Je�li gracz nie jest w zasi�gu wzroku i ataku, przejd� do patrolu
        if (!playerInSightRange && !playerInAttackRange)
        {
            Patroling();
            speed = patrolSpeed;
            state = MovementState.patroling;
        }
        // Je�li gracz jest w zasi�gu wzroku, ale jeszcze nie w zasi�gu ataku, przejd� do po�cigu
        if (playerInSightRange && !playerInAttackRange)
        {
            ChasePlayer();
            speed = chaseSpeed;
            state = MovementState.chasing;
        }
        // Je�li gracz jest w zasi�gu ataku, przejd� do atakowania
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
        // Je�li przeciwnik atakuje, nie wykonujemy ruchu
        if (state is MovementState.attacking) return;

        // Oblicz kierunek w stron� celu
        Vector3 direction = (destination - transform.position).normalized;

        // Obr�� przeciwnika w stron� kierunku poruszania
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Poruszaj si� do przodu w kierunku, w kt�rym jest obr�cony obiekt
        Vector3 forward = transform.forward;
        rb.velocity = forward * rb.velocity.magnitude;
        // Utrzymanie dotychczasowej pr�dko�ci, ale w aktualnie obr�conym kierunku

        rb.AddForce(forward * speed, ForceMode.Force);
        // Dodanie si�y w kierunku forward, by rozp�dzi� przeciwnika

        // Ograniczenie pr�dko�ci, �eby nie przekroczy� zdefiniowanej "speed"
        if (rb.velocity.magnitude > speed)
        {
            rb.velocity = forward * speed;
        }
    }

    private void Patroling()
    {
        // Je�li nie ma ustawionego punktu patrolu, wyszukaj nowy
        if (!_walkPointSet) SearchWalkPoint();

        // Je�li mamy punkt patrolu, ustaw go jako cel
        if (_walkPointSet)
        {
            destination = walkPoint;
            speed = patrolSpeed;
        }

        // Oblicz dystans do obecnego punktu patrolu
        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // Je�li przeciwnik zbli�y� si� wystarczaj�co, ustal, �e trzeba znale�� nowy punkt
        if (distanceToWalkPoint.magnitude < 1f)
        {
            _walkPointSet = false;
        }
    }

    private void SearchWalkPoint()
    {
        // Losowo wybierz przesuni�cie w zakresie walkPointRange
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        float randomY = Random.Range(-walkPointRange, walkPointRange);
        float randomZ = Random.Range(-walkPointRange, walkPointRange);

        // Je�li patrol ma si� odbywa� wok� pozycji startowej
        if (patrolAroundStartOnly)
        {
            walkPoint = new Vector3(startPosition.x + randomX, startPosition.y + randomY, startPosition.z + randomZ);
        }
        // W przeciwnym razie patroluj wok� aktualnej pozycji
        else
        {
            walkPoint = new Vector3(transform.position.x + randomX, transform.position.y + randomY, transform.position.z + randomZ);
        }

        // Sprawd�, czy na drodze do wybranego punktu nie ma przeszk�d (warstwa ziemi - whatIsGround)
        Vector3 rayCastDirection = (walkPoint - transform.position).normalized;
        float maxDistance = Vector3.Distance(transform.position, walkPoint) + 1;

        // Je�li Raycast nie trafia w ziemi�, oznacza to �e punkt jest osi�galny
        if (!Physics.Raycast(transform.position, rayCastDirection, maxDistance, whatIsGround))
        {
            _walkPointSet = true;
            Debug.Log("ustawiono");
        }
    }

    private void ChasePlayer()
    {
        // Pod��aj za graczem, ustaw cel na pozycj� gracza
        destination = player.position;
    }

    private void AttackPlayer()
    {
        // Podczas ataku przeciwnik zatrzymuje si� w miejscu
        Health healthSystem = player.GetComponent<Health>(); // Odwo�anie do skryptu gracza z logik� zdrowia
        destination = transform.position;                    // Zatrzymaj si� w miejscu

        // Obr�� si� w stron� gracza
        transform.LookAt(player);

        // Je�li przeciwnik nie atakowa� w ostatnim czasie (cooldown)
        if (!_alreadyAttacked)
        {
            // Zadaj obra�enia graczowi
            if (healthSystem != null)
            {
                healthSystem.TakeDamage(damage);
            }

            // Ustaw flag� i rozpocznij odliczanie do nast�pnego ataku
            _alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        // Reset flagi ataku, by pozwoli� na kolejny atak
        _alreadyAttacked = false;
    }
}
