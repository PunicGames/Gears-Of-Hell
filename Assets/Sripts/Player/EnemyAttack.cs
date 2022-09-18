using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public float timeBetweenAttacks = 0.5f;
    public int attackDamage = 10;

    GameObject player;
    Health playerHealth;
    bool playerInRange;
    float timer;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<Health>();
    }

    private void FixedUpdate()
    {
        timer += Time.deltaTime;

        if (timer >= timeBetweenAttacks && playerInRange) {
            Attack();
        }
    }

    private void Attack() 
    {
        timer = 0f;

        if (playerHealth.currentHealth > 0) {
            playerHealth.TakeDamage(attackDamage);
        }
    }


    // Para ejecutar los siguientes m�todos el objeto debe tener un collider con IsTrigger activado.
    // Al objeto de enemigo le he dado dos collider iguales. Uno con IsTrigger y otro sin �l.
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player) {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            playerInRange = false;
        }
    }
}
