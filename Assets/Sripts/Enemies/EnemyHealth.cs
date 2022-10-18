using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    public int startingHealth = 100;
    public int currentHealth;
    public int scoreValue = 10;

    [SerializeField]
    private GameObject coin;

    BoxCollider collider;
    bool isDead;

    private int timeAnimationDead = 1;
    private void Awake()
    {
        collider = GetComponent<BoxCollider>();
        currentHealth = startingHealth;
        //Debug.Log("Health: " + currentHealth);
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


        NavMeshAgent navMov = GetComponent<NavMeshAgent>();
        EnemiesMovement eM = GetComponent<EnemiesMovement>();
        EnemyMeleeAttack mE = GetComponent<EnemyMeleeAttack>();
        RangedEnemy rE = GetComponent<RangedEnemy>();
        if (eM != null)
            eM.enabled = false;
        if(rE != null)
            rE.enabled = false;
        if(mE != null)
            mE.enabled = false;
        if (navMov != null)
            navMov.enabled = false;

        // Faltan sonidos y animaciones de muerte etc etc

        // Suelta moneda
        GameObject moneda = Instantiate(coin, transform.position, Quaternion.identity);
        moneda.gameObject.GetComponent<Moneda>().value = scoreValue;


        Destroy(gameObject, timeAnimationDead);
    }
}
