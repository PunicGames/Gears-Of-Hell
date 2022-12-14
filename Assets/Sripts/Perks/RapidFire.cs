using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RapidFire : MonoBehaviour
{

    public float bufAttackSpeed;

    [SerializeField] private GameObject shootObject;
    private ShootSystem shootScript;
    float[] weaponBaseTimeBetweenShoots;

 

    private void Start()
    {
        shootScript = shootObject.GetComponent<ShootSystem>();
        weaponBaseTimeBetweenShoots = new float[shootScript.guns.getGuns().Length];
        gameObject.SetActive(false);

        for (int i = 0; i < shootScript.guns.getGuns().Length; i++)
        {
            weaponBaseTimeBetweenShoots[i] = shootScript.guns.getGuns()[i].timeBetweenShots;
        }
       
       
    }

    private void OnEnable()
    {
        if (shootScript != null)
        {

            var aux = shootScript.guns.getGuns();

           
            foreach (Gun g in aux)
            {
                g.timeBetweenShots *= 0.92f;
            }
        }
        
    }

    private void OnDisable()
    {
        //if (shootScript != null)
        //{

        //    var aux = shootScript.guns.getGuns();

        //    foreach (Gun g in aux)
        //    {
        //        g.timeBetweenShots = defAttackSpeed;
        //    }
        //    for (int i = 0; i < length; i++)
        //    {
        //        gameObject.
        //    }

        }

}
