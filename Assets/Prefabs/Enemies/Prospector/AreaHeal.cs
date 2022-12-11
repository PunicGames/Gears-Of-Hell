using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaHeal : MonoBehaviour
{
    public bool casting = false;
    private float speed = 5.5f;

    private void FixedUpdate()
    {
        if (casting)
            transform.localScale += new Vector3(speed * Time.deltaTime, speed * Time.deltaTime, speed * Time.deltaTime);
    }

    public void ResetScaling()
    {
        casting = true;
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    }
}
