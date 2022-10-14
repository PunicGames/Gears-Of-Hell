using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPlayer : MonoBehaviour
{

    private int timeToDestroy = 3;
    public int damage;

    private void Awake()
    {
        Destroy(gameObject, timeToDestroy); // El gameObject se destruye al cabo de x segundos
    }

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
            Destroy(gameObject);
        }
    }
}
