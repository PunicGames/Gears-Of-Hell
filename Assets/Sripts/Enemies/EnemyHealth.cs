using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int startingHealth = 100;
    public int currentHealth;
    public int scoreValue = 10;

    BoxCollider collider;
    bool isDead;

    private int timeAnimationDead = 1;
    private void Awake()
    {
        collider = GetComponent<BoxCollider>();
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

        GetComponent<EnemyAttack>().enabled = false;

        // CODIGO PROVISIONAL PARA EL ENEMIGO PERSEGUIDOR PARA DESACTIVAR SU SCRIPT DE MOVIMIENTO
        EnemiesMovement eM = GetComponent<EnemiesMovement>();
        if (eM != null) {
            eM.enabled = false;
        }

        // Faltan sonidos y animaciones de muerte etc etc

        Destroy(gameObject, timeAnimationDead);
    }
}
