using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSpider : MonoBehaviour
{

    public float speed = 6f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(-1.0f*Time.deltaTime * speed, 0.0f, 1.0f*Time.deltaTime * speed);
    }
}
