using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public bool active;
    private bool psPlaying = false;

    private ParticleSystem ps;

    private void Awake()
    {
        ps = gameObject.GetComponentInChildren<ParticleSystem>();
    }

    private void Update()
    {
        if(active && !psPlaying)
        {
            ps.Play();
            psPlaying = true;
        }
        if(!active && psPlaying)
        {
            ps.Stop();
            psPlaying = false;
        }
    }

}
