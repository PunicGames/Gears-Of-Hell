using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserShot : MonoBehaviour
{
    private float defBulletForce;
    public float bufBulletForce;

    private int defBulletMaxCollisions;
    public int bufBulletMaxCollisions;

    private int tier =0;
    private int wallEnableTier = 4;

    [SerializeField] private GameObject shootObject;
    private ShootSystem shootScript;

    private void Start()
    {
        shootScript = shootObject.GetComponent<ShootSystem>();
        defBulletForce = shootScript.guns.getGuns()[shootScript.selectedGun].shootForce;

        defBulletMaxCollisions= shootScript.numberOfCollisions;
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (shootScript != null)
        {
            tier++;
            shootScript.laserShot = true;
            shootScript.guns.getGuns()[shootScript.selectedGun].shootForce *= bufBulletForce;
            shootScript.numberOfCollisions += bufBulletMaxCollisions;
            if (tier >= wallEnableTier)
                shootScript.canBeStoppedByWalls = false;
        }
    }

    private void OnDisable()
    {
        if (shootScript != null)
        {
            shootScript.laserShot = false;
            shootScript.guns.getGuns()[shootScript.selectedGun].shootForce = defBulletForce;
            shootScript.numberOfCollisions = defBulletMaxCollisions;
        }
    }
}

