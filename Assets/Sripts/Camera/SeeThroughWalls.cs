using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeeThroughWalls : MonoBehaviour
{
    [Range(0, 1)]
    public float transparencyStrength;

    public LayerMask mask;
    private Transform targetPlayer;
    private Renderer lastRenderer;
    private Color lastColor;


    private void Start()
    {
        targetPlayer = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        // IMPORTANTE: para que funcione bien el modo de renderizado de los objetos deseados han de estar en Transparent o Fades


        RaycastHit hit;
        if (Physics.Raycast(transform.position, (targetPlayer.position - transform.position).normalized, out hit, Mathf.Infinity, mask))
        {
            if (hit.collider.gameObject.tag == "Wall")
            {
                if (hit.collider.gameObject.GetComponent<MeshRenderer>() != null)
                {
                    if (lastRenderer != null)
                    { // Si nos movemos de muro a muro, sigue detectando muro, por lo que el anterior muro nunca se pune a true. De ahí esta linea
                        lastRenderer.material.color = lastColor;
                    }

                    Renderer ren = hit.collider.gameObject.GetComponent<Renderer>();
                    Color newColor = ren.materials[0].color;
                    lastColor = newColor;
                    newColor.a = transparencyStrength;
                    lastRenderer = ren;

                    ren.materials[0].color = newColor;
                }
            }
        }
        else
        {
            lastRenderer.material.color = lastColor;
        }
    }
}