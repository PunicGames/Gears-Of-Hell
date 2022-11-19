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
        shootScript = GameObject.Find("ShotOrigin").GetComponent<ShootSystem>();
        defAttackSpeed = shootScript.guns.getGuns()[shootScript.selectedGun].timeBetweenShots;
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (shootScript != null)
        {
            //shootScript.guns.getGuns()[shootScript.selectedGun].timeBetweenShots /= bufAttackSpeed;

            var aux = shootScript.guns.getGuns();

            foreach (Gun g in aux)
            {
                g.timeBetweenShots /= bufAttackSpeed;
            }
        }
        
    }

    private void OnDisable()
    {
        if (shootScript != null)
        {
            //shootScript.guns.getGuns()[shootScript.selectedGun].timeBetweenShots = defAttackSpeed;

            var aux = shootScript.guns.getGuns();

            foreach (Gun g in aux)
            {
                g.timeBetweenShots = defAttackSpeed;
            }

        }
            
    }
}
