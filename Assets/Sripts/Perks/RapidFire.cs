using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RapidFire : MonoBehaviour
{

    private float defAttackSpeed;
    public float bufAttackSpeed;

    private ShootSystem shootScript;

    private void Start()
    {
        shootScript = GameObject.Find("GunEnd").GetComponent<ShootSystem>();
        defAttackSpeed = shootScript.timeBetweenShots;
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (shootScript != null)
        {
            shootScript.timeBetweenShots = bufAttackSpeed;
        }
        
    }

    private void OnDisable()
    {
        if (shootScript != null)
        {
            shootScript.timeBetweenShots = defAttackSpeed;
        }
            
    }
}
