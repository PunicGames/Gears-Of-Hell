using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEnemy : MonoBehaviour
{

    Rigidbody rb;
    private float damage;
    private int timeToDestroy = 3;

    private void Awake()
    {
        Destroy(gameObject, timeToDestroy);
    }

    public void setForce(float force)
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward.normalized * force, ForceMode.Impulse);
    }

    public void setDamage(float d) {
        damage = d;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Colision con jugador");
            // Quitamos vida al jugador
            Health playerHealth = other.gameObject.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        if (other.gameObject.tag == "Wall")
            Destroy(gameObject);


    }
}
