using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegAimGrounding : MonoBehaviour
{
    GameObject raycastOrigin;
    int layerMask;
    void Start()
    {
        layerMask = LayerMask.GetMask("Floor");
        raycastOrigin = transform.parent.gameObject;
    }

    void Update()
    {
        // Desde una posicion de inicio de lanzado de rayos (más o menos donde se encuentran las "rodillas de la araña") se lanzan rayos
        // hacia abajo. La posicion de este gameObject(cubos) será la colisión de estos rayos con el suelo.
        RaycastHit hit;
        if (Physics.Raycast(raycastOrigin.transform.position, -transform.up, out hit, Mathf.Infinity, layerMask))
        {
            transform.position = hit.point;
        }
    }
}
