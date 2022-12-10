using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigShot : MonoBehaviour
{
    private float defScaleFactor;
    public float bufScaleFactor;
    public float bufDamageFactor = 0.1f;

    [SerializeField] private GameObject shootObject;
    private ShootSystem shootScript;

    private void Start()
    {
        shootScript = shootObject.GetComponent<ShootSystem>();
        defScaleFactor = shootScript.scaleFactor;
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (shootScript != null)
        {
            shootScript.bigShot = true;
            shootScript.scaleFactor *= bufScaleFactor;

            var aux = shootScript.guns.getGuns();


            foreach (Gun g in aux)
            {
                g.bulletDamage = (int)(g.bulletDamage+g.bulletDamage* bufDamageFactor);
            }
        }
    }

    private void OnDisable()
    {
        if (shootScript != null)
        {
            shootScript.bigShot = false;
            shootScript.scaleFactor = defScaleFactor;
        }
    }
}
