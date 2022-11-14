using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public bool active;
    private bool psPlaying = false;

    // Rotation
    private bool spining;
    private float rotation;
    private float differenceRotation = 0;
    [SerializeField] private float panelSpeedRotation;

    private ParticleSystem ps;

    [SerializeField] private Transform panel;

    private void Awake()
    {
        ps = gameObject.GetComponentInChildren<ParticleSystem>();
    }

    private void Start()
    {
        //transform.Rotate(Vector3.up, Random.Range(0.0f, 359.9f));
        transform.Rotate(Vector3.up, 45f);
    }

    private void Update()
    {
        if(active && !psPlaying)
        {
            ps.Play();
            psPlaying = true;
            spining = true;
        }

        if(!active && psPlaying)
        {
            ps.Stop();
            psPlaying = false;
            spining = true;
        }

        // Animacion de rotacion
        if (spining) {
            rotation = panelSpeedRotation * Time.deltaTime;
            panel.Rotate(0, 0, rotation, Space.Self);
            differenceRotation += rotation;
            if (differenceRotation >= 180) { 
                spining = false;
                differenceRotation = 0;
            }
        }

    }
}
