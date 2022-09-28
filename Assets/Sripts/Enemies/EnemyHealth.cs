using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int startingHealth = 100;
    public int currentHealth;
    public int scoreValue = 10;

    CapsuleCollider collider;
    bool isDead;

    private void Awake()
    {
        collider = GetComponent<CapsuleCollider>();
        currentHealth = startingHealth;
        Debug.Log("Health: " + currentHealth);
    }

    public void TakeDamage(int amount) {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log("Vida enemigo: " + currentHealth);

        if (currentHealth <= 0) {
            Death();
        }
    }

    private void Death() {
        isDead = true;
        collider.enabled = false;
        // Faltan sonidos y animaciones de muerte etc etc

        Destroy(this.gameObject);
    }
}
