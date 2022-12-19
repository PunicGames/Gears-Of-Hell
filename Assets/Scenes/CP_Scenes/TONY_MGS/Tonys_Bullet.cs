using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tonys_Bullet : MonoBehaviour
{
 
    private Rigidbody rb;
    public float damage = 10;
    public float timeToDestroy = .25f;
    public int numberOfCollisions = 0;
    public bool stoppedByWalls = true;

    [HideInInspector] public bool laserShot;

    // Shoot System reference to update bullets record from player
    private PlayerStats pS = null;

    // Bullet Shooted by
    public enum BulletOwner { PLAYER, ENEMY }
    public BulletOwner owner;

    private List<GameObject> alreadyHitted = new List<GameObject>();

    private void Awake()
    {
        rb = transform.GetComponent<Rigidbody>();
    }
    private void Start()
    {
        Destroy(gameObject, timeToDestroy);
        Invoke(nameof(FuckUpBulletDamage1), .08f);
        Invoke(nameof(FuckUpBulletDamage2), .15f);
        
    }
    private void FuckUpBulletDamage1()
    {
        damage = Random.Range(5, 11);
    }
    private void FuckUpBulletDamage2()
    {
        damage = Random.Range(0, 6);
    }

    public void SetForce(float force)
    {
        rb.AddForce(transform.forward.normalized * force, ForceMode.Impulse);
    }

    public void SetForce(Vector3 direction, float force)
    {
        rb.AddForce(direction * force, ForceMode.Impulse);
    }

    
    public void SetPlayerStats(PlayerStats ps)
    {
        pS = ps;
    }

    public void SetBulletColors(Color albedo, Color emissive)
    {
        this.GetComponent<Renderer>().material.SetColor("_Color", albedo);
        this.GetComponent<Renderer>().material.SetColor("_EmissionColor", emissive);
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (owner)
        {
            case BulletOwner.PLAYER:
                if (other.gameObject.tag == "Enemy")
                {
                    // Quitamos vida al enemigo
                    EnemyHealth enemyHealth = other.gameObject.GetComponent<EnemyHealth>();
                    if ((enemyHealth != null) && (!alreadyHitted.Contains(other.gameObject)))
                    {
                        enemyHealth.TakeDamage((int)damage);
                        pS.numBulletsHit++;
                        if (!laserShot)
                        {
                            Destroy(gameObject);
                        }
                        else
                        {
                            numberOfCollisions--;
                            if (numberOfCollisions == 0)
                                Destroy(gameObject);

                        }
                    }

                    alreadyHitted.Add(other.gameObject);

                }
                else
                {
                    pS.numBulletsMissed++;
                }
                break;
            case BulletOwner.ENEMY:
                if (other.gameObject.tag == "Player")
                {

                    // Quitamos vida al jugador
                    Health playerHealth = other.gameObject.GetComponent<Health>();

                    if ((playerHealth != null) && (!alreadyHitted.Contains(other.gameObject)))
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
        {
            if (stoppedByWalls)
                Destroy(gameObject);
            else
            {
                numberOfCollisions--;
                if (numberOfCollisions == 0)
                    Destroy(gameObject);
            }

        }
    }
}


