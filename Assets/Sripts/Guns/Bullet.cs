using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody rb;
    private float damage;
    private int timeToDestroy = 3;
    [HideInInspector] public bool laserShot;

    // Bullet Shooted by
    public enum BulletOwner { PLAYER, ENEMY}
    public BulletOwner owner;

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

    private void OnTriggerEnter(Collider other)
    {
        switch (owner) {
            case BulletOwner.PLAYER:
                if (other.gameObject.tag == "Enemy")
                {
                    // Quitamos vida al enemigo
                    EnemyHealth enemyHealth = other.gameObject.GetComponent<EnemyHealth>();
                    if (enemyHealth != null)
                    {
                        enemyHealth.TakeDamage((int)damage);

                        if (!laserShot)
                        {
                            Destroy(gameObject);
                        }
                    }
                }
                break;
            case BulletOwner.ENEMY:
                if (other.gameObject.tag == "Player")
                {

                    // Quitamos vida al jugador
                    Health playerHealth = other.gameObject.GetComponent<Health>();
                    if (playerHealth != null)
                    {
                        playerHealth.TakeDamage(damage);
                    }
                    Destroy(gameObject);
                }
                break;
            default:
                break;
        }


        if (other.gameObject.tag == "Wall" || other.gameObject.tag == "Shop")
            Destroy(gameObject);
    }
}
