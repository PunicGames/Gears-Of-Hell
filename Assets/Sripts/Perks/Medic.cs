using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medic : MonoBehaviour
{
    public float healRate;
    public float healPercentAmount;

    private Health healthScript;

    private void Start()
    {
        healthScript = GameObject.Find("Player_Character").GetComponent<Health>();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        InvokeRepeating("MedicHeal", 0f, healRate);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void MedicHeal()
    {
        healthScript.currentHealth += healthScript.maxHealth / 100 * healPercentAmount;
        if(healthScript.currentHealth > healthScript.maxHealth)
        {
            healthScript.currentHealth = healthScript.maxHealth;
        }
        print(healthScript.currentHealth);
    }
}
