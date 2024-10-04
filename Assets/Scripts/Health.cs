using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage.");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Logika œmierci obiektu
        Debug.Log(gameObject.name + " has died.");
        Destroy(gameObject);
    }
}

