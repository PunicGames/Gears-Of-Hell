using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticVest : MonoBehaviour
{
    
    private float defHealth;
    public float bufHealth;

    private Health healthScript;

    private void Start()
    {
        healthScript = GetComponentInParent<Health>();
        defHealth = healthScript.maxHealth;
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (healthScript != null)
        {
            healthScript.maxHealth *= bufHealth;
            healthScript.currentHealth = healthScript.maxHealth;
        }
    }

    private void OnDisable()
    {
        if (healthScript != null)
        {
            healthScript.maxHealth = defHealth;
            healthScript.currentHealth = healthScript.maxHealth;
        }
    }
}
