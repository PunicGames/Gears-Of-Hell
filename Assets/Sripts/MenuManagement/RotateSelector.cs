using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSelector : MonoBehaviour
{
    [SerializeField] bool isLocal = true;
    [SerializeField] float speed = 5f;

    void Update()
    {
        if (!isLocal) transform.Rotate(Vector3.up * (speed * 10) * Time.deltaTime,Space.World);
        else transform.Rotate(Vector3.up * (speed * 10) * Time.deltaTime, Space.Self);
    }
}
