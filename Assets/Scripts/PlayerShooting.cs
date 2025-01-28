using UnityEngine;
using System.Collections;

public class PlayerShooting : MonoBehaviour
{
    private Camera mainCamera;
    public PlayerStats playerStats;
    public PlayerUIManager uiManager;
    public Animator weaponAnimator;

    [Header("Hitscan")]
    public float weaponRange = 100f;
    public LayerMask hitLayers;
    public GameObject impactEffect;

    [Header("Effects")]
    public ParticleSystem muzzleFlash; 
    public AudioSource gunAudioSource;
    public AudioClip fireSound;
    public AudioClip reloadSound;

    private int currentAmmo;
    private bool canShoot = true;

    private void Start()
    {
        mainCamera = Camera.main;
        currentAmmo = playerStats.maxAmmo;
        uiManager.UpdateAmmoText(currentAmmo);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1") && canShoot && currentAmmo > 0)
        {
            StartCoroutine(Shoot());
        }

        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < playerStats.maxAmmo)
        {
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Shoot()
    {
        if (currentAmmo <= 0) yield break;

        canShoot = false;
        currentAmmo--;
        uiManager.UpdateAmmoText(currentAmmo);

        // Play muzzle flash effect
        if (muzzleFlash != null) muzzleFlash.Play();

        // Play shooting animation
        weaponAnimator.SetBool("isShooting", true);
        weaponAnimator.SetTrigger("Shoot");

        // Play shooting sound
        if (gunAudioSource != null && fireSound != null)
            gunAudioSource.PlayOneShot(fireSound);

        // Fire hitscan
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, weaponRange, hitLayers))
        {
            if (hit.collider.TryGetComponent<Health>(out var target))
            {
                float finalDamage = playerStats.damage;
                if (Random.Range(0f, 100f) < playerStats.criticalChance)
                {
                    finalDamage *= 2; // Critical hit
                }
                target.TakeDamage(finalDamage);
                if (impactEffect != null) Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }

        yield return new WaitForSeconds(playerStats.fireRate);
        weaponAnimator.SetBool("isShooting", false);
        canShoot = true;
    }

    private IEnumerator Reload()
    {
        canShoot = false;

        if (weaponAnimator != null)
        {
            weaponAnimator.SetFloat("ReloadSpeed", 2f / playerStats.reloadSpeed);
            weaponAnimator.SetTrigger("Reload"); 
        }

        if (gunAudioSource != null && reloadSound != null)
        {
            gunAudioSource.PlayOneShot(reloadSound);
        }

        yield return new WaitForSeconds(playerStats.reloadSpeed); 

        currentAmmo = playerStats.maxAmmo;
        uiManager.UpdateAmmoText(currentAmmo);

        canShoot = true;
    }

}
