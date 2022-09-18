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
    }

    public void TakeDamage(int amount, Vector3 hitPoint) { // hitPoint es el punto exacto del impacto (preparado para spawnear un efecto de particulas de impacto si as� se quisiera)
        if (isDead) return;

        Debug.Log("Vida enemigo: " + currentHealth);
        currentHealth -= amount;

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
