using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstAidKit : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Health healthScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
            ParticleSystem ps = gameObject.GetComponentInChildren<ParticleSystem>();
            SkinnedMeshRenderer smr = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
            if (healthScript != null)
            {
                healthScript.currentHealth = healthScript.maxHealth;
                smr.enabled = false;
                ps.Play();
                Invoke("AuxDestroy", ps.main.duration);
            }
        }
    }

    private void AuxDestroy()
    {
        Destroy(gameObject);
    }
}
