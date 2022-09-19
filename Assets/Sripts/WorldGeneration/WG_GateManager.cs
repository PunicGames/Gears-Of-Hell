using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WG_GateManager : MonoBehaviour
{
    public Color open, closed;
    private MeshRenderer meshRenderer;

    public void UpdateColor(bool state)
    {
        if (state)
        {
            meshRenderer.material.color = open;
        } else
        {
            meshRenderer.material.color = closed;
        }
    }

    public void Awake()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }
}
