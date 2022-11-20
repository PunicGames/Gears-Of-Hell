using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigShot : MonoBehaviour
{
    private float defScaleFactor;
    public float bufScaleFactor;

    private ShootSystem shootScript;

    private void Start()
    {
        shootScript = GameObject.Find("ShootSystem").GetComponent<ShootSystem>();
        defScaleFactor = shootScript.scaleFactor;
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (shootScript != null)
        {
            shootScript.bigShot = true;
            shootScript.scaleFactor *= bufScaleFactor;
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
