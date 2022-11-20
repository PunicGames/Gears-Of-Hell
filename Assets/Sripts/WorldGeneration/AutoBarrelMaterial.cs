using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoBarrelMaterial : MonoBehaviour
{
    [SerializeField] Material[] materials;
    [SerializeField] float probDestroy = 0.5f;
    [SerializeField] bool canUnInstantiate;

    public void Awake()
    {
        if (canUnInstantiate && (Random.value <= probDestroy))
        {
            Destroy(transform.parent.gameObject);
        }

        gameObject.GetComponent<MeshRenderer>().material = materials[Random.Range(0, materials.Length)];
    }
}
