using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSelector : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(Vector3.up * 50 * Time.deltaTime, Space.Self);
    }
}
