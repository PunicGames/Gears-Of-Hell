using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;


public class WorkerBotWeaponBehabior : MonoBehaviour
{
    // Start is called before the first frame update
    [HideInInspector] public GameObject player { get; set; }
    [HideInInspector] public Health playerHealth { get; set; }
    [HideInInspector] public int attackDamage { get; set; }
    [HideInInspector] public EnemyHealth health { get; set; }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
            DamagePlayer();
    }
    private void DamagePlayer()
    {
        if (enabled)
        {

            if (playerHealth.currentHealth > 0)
            {

                if (playerHealth.electricBarrier)
                {
                    health.Death();
                    player.GetComponentInChildren<ElectricBarrier>().ConsumeBarrier();
                }
                else
                {
                    playerHealth.TakeDamage(attackDamage);
                }

            }
        }
        enabled = false;
    }
}
