using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody rb;
    private float damage;
    private int timeToDestroy = 3;
    [HideInInspector] public bool laserShot;

    // Shoot System reference to update bullets record from player
    private ShootSystem sS = null;

    // Bullet Shooted by
    public enum BulletOwner { PLAYER, ENEMY}
    public BulletOwner owner;

    private List<GameObject> alreadyHitted = new List<GameObject>();

    private void Awake()
    {
        rb = transform.GetComponent<Rigidbody>();
        Destroy(gameObject, timeToDestroy);
    }

    public void SetForce(float force)
    {
        rb.AddForce(transform.forward.normalized * force, ForceMode.Impulse);
    }

    public void SetForce(Vector3 direction, float force)
    {
        rb.AddForce(direction * force, ForceMode.Impulse);
    }

    public void SetDamage(float d)
    {
        damage = d;
    }

    public void SetLaser(bool option) { 
        laserShot = option;
    }

    public void SetShootSystem(ShootSystem ss) {
        sS = ss;
    }

    public void SetBulletColors(Color albedo, Color emissive) {
        this.GetComponent<Renderer>().material.SetColor("_Color", albedo);
        this.GetComponent<Renderer>().material.SetColor("_EmissionColor", emissive);
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (owner) {
            case BulletOwner.PLAYER:
                if (other.gameObject.tag == "Enemy")
                {
                    // Quitamos vida al enemigo
                    EnemyHealth enemyHealth = other.gameObject.GetComponent<EnemyHealth>();
                    if ((enemyHealth != null) && (!alreadyHitted.Contains(other.gameObject)))
                    {
                        enemyHealth.TakeDamage((int)damage);
                        sS.numBulletsHit++;
                        if (!laserShot)
                        {
                            Destroy(gameObject);
                        }
                    }

                    alreadyHitted.Add(other.gameObject);

                }
                else {
                    sS.numBulletsMissed++;
                }
                break;
            case BulletOwner.ENEMY:
                if (other.gameObject.tag == "Player")
                {

                    // Quitamos vida al jugador
                    Health playerHealth = other.gameObject.GetComponent<Health>();

                    if((playerHealth != null) && (!alreadyHitted.Contains(other.gameObject)))
                    {
                        if (playerHealth.electricBarrier)
                        {
                            other.GetComponentInChildren<ElectricBarrier>().ConsumeBarrier();
                        }
                        else
                        {
                            playerHealth.TakeDamage(damage);
                        }
                        Destroy(gameObject);
                    }

                    alreadyHitted.Add(other.gameObject);

                }
                break;
            default:
                break;
        }


        if (other.gameObject.tag == "Wall" || other.gameObject.tag == "Shop")
            Destroy(gameObject);
    }
}
