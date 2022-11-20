using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gunsmith : MonoBehaviour
{
    public float ammoRegenRate;
    public int ammoRegenAmount;

    private ShootSystem shootScript;

    private void Start()
    {
        shootScript = GameObject.Find("ShootSystem").GetComponent<ShootSystem>();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        InvokeRepeating("AmmunitionRegen", 0f, ammoRegenRate);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void AmmunitionRegen()
    {
        for(int i = 0; i < shootScript.guns.getGuns().Length; i++)
        {
            var sg = shootScript.guns.getGuns()[i];

            if (shootScript.availableGuns[i])
            {
                sg.totalBullets += ammoRegenAmount;
            }
        }
    }
}
