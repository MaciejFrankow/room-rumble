using UnityEngine;

[CreateAssetMenu(fileName = "Player Stats", menuName = "ScriptableObjects/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    [Header("Health & Stamina")]
    public float currentHealth;
    public float maxHealth;
    public float currentStamina;
    public float maxStamina;

    [Header("Combat")]
    public int maxAmmo;
    public float damage;
    public float armor;
    public float fireRate;
    public float criticalChance;
    public float reloadSpeed; 

    [Header("Movement")]
    public float movementSpeed;
}
