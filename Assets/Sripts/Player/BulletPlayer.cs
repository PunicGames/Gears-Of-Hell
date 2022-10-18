using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPlayer : MonoBehaviour
{

    private int timeToDestroy = 3;
    public int damage;
    public bool laserShot;

    private void Awake()
    {
        Destroy(gameObject, timeToDestroy); // El gameObject se destruye al cabo de x segundos
    }

    /*
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy") { 
            Debug.Log("Colision con enemigo");
            // Quitamos vida al enemigo
            EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
            if (!laserShot)
            {
                Destroy(gameObject);
            }
        }
    }
    */
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            //Debug.Log("Colision con enemigo");
            // Quitamos vida al enemigo
            EnemyHealth enemyHealth = other.gameObject.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);

                if (!laserShot)
                {
                    Destroy(gameObject);
                }
            }
            
        }
    }
    
}
