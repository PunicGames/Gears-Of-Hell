using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstAidKit : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Health healthScript = other.gameObject.GetComponent<Health>();
            SkinnedMeshRenderer smr = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
            if (healthScript != null)
            {
                var healing = healthScript.maxHealth - healthScript.currentHealth;

                if(healing > 0)
                {
                    healthScript.Heal(healing);
                }
              
                smr.enabled = false;
                Destroy(gameObject);
            }
        }
    }

   
}
