using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParametricTires : MonoBehaviour
{
    [SerializeField] GameObject tire;
    [SerializeField] int maxTires = 4;
    [SerializeField] float ySpacing = 0.2f;

    private void Awake()
    {
        var nTires = Random.Range(0, maxTires);
        var yPos = transform.position.y;
        for (int i = 0; i < nTires; i++)
        {
            var obj = Instantiate(tire, new Vector3(transform.position.x, yPos, transform.position.z), Quaternion.identity);
            obj.transform.parent = transform;
            yPos += ySpacing;
        }
    }
}
