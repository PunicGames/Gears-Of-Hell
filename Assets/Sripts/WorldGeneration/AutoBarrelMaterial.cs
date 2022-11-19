using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoBarrelMaterial : MonoBehaviour
{
    [SerializeField] Material[] materials;
    [SerializeField] bool canUnInstantiate;

    public void Awake()
    {
        if (canUnInstantiate && (Random.value >= 0.5))
        {
            Destroy(transform.parent.gameObject);
        }

        gameObject.GetComponent<MeshRenderer>().material = materials[Random.Range(0, materials.Length)];
    }
}
