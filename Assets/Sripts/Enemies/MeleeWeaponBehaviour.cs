using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponBehaviour : MonoBehaviour
{
    public GameObject player { get; set; }
    public Health playerHealth { get; set; }
    [HideInInspector] public int attackDamage { get; set; }


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
                    gameObject.GetComponent<EnemyHealth>().Death();
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
