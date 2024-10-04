using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerShooting : MonoBehaviour
{
    private Camera mainCamera;

    [Header("Projectile")]
    public GameObject projectilePrefab;
    public Transform firePoint;          
    public float projectileSpeed = 20f;

    [Header("Hitscan")]
    public float weaponRange = 100f;
    public int damage = 10;           
    public LayerMask hitLayers;
    public GameObject impactEffect;     // Optional: Visual effect when hitting something

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            ShootHitscan();
        }
        if (Input.GetButtonDown("Fire2"))
        {
            ShootProjectile();
        }
    }

    private void ShootProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.velocity = firePoint.forward * projectileSpeed;
    }

    private void ShootHitscan()
    {
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, weaponRange, hitLayers))
        {
            Debug.Log("Hit: " + hit.collider.name);

            if (hit.collider.TryGetComponent<Health>(out var target))
            {
                target.TakeDamage(damage);
                if (impactEffect != null)
                {
                    Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                }
            }
        }
    }
}
