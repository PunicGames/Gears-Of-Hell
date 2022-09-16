using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{

    Rigidbody rb;

    public void setForce(float force)
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * force,ForceMode.Force);

        Invoke("DestroyBullet", 2.0f);
    }

    private void DestroyBullet()
    {
        Destroy(gameObject);
    }


}
