using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserShot : MonoBehaviour
{
    private float defBulletForce;
    public float bufBulletForce;

    private ShootSystem shootScript;

    private void Start()
    {
        shootScript = GameObject.Find("ShotOrigin").GetComponent<ShootSystem>();
        defBulletForce = shootScript.guns.getGuns()[shootScript.selectedGun].shootForce;
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (shootScript != null)
        {
            shootScript.laserShot = true;
            shootScript.guns.getGuns()[shootScript.selectedGun].shootForce *= bufBulletForce;
        }
    }

    private void OnDisable()
    {
        if (shootScript != null)
        {
            shootScript.laserShot = false;
            shootScript.guns.getGuns()[shootScript.selectedGun].shootForce = defBulletForce;
        }
    }
}

