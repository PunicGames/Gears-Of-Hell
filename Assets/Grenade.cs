using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{

    Rigidbody rb;

    int damage = 40;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetForce(float force)
    {
        rb.AddForce(transform.forward.normalized * force, ForceMode.Impulse);
    }

    public void SetForce(Vector3 direction, float force)
    {
        rb.AddForce(direction * force, ForceMode.Impulse);
    }

}
