using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyMeleeAttack : MonoBehaviour
{
    public float timeBetweenAttacks = 0.5f;
    public int attackDamage = 10;

    GameObject player;
    Health playerHealth;
    bool playerInRange;
    float timer;
    Animator animator;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();

        // Si a alguien le da error esto de aqui haciendo pruebas en PlayerScene es normal, ya que los enemigos estan spawneados antes que el personaje principal
        playerHealth = player.GetComponent<Health>();
    }

    private void FixedUpdate()
    {
        timer += Time.deltaTime;
       
    }

    public void Attack() 
    {
        timer = 0f;

        if (playerHealth.currentHealth > 0) {

            if (playerHealth.electricBarrier)
            {
                gameObject.GetComponent<EnemyHealth>().Death();
                player.GetComponentInChildren<ElectricBarrier>().ConsumeBarrier();
            }
            else
            {
                playerHealth.TakeDamage(attackDamage);
            }
            
        }
    }


    // Para ejecutar los siguientes métodos el objeto debe tener un collider con IsTrigger activado.
    // Al objeto de enemigo le he dado dos collider iguales. Uno con IsTrigger y otro sin él.
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player) {
            GetComponent<NavMeshAgent>().isStopped = true;
            playerInRange = true;
            animator.SetTrigger("Attack");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            GetComponent<NavMeshAgent>().isStopped = false;
            playerInRange = false;
        }
    }

}
