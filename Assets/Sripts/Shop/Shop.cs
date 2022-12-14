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
    [HideInInspector] AudioSource source;

    private void Awake()
    {
        ps = gameObject.GetComponentInChildren<ParticleSystem>();
        source = gameObject.GetComponent<AudioSource>();
        PauseMenu.pauseShopMusic += MuteMusic;
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
            source.Play();
            GetComponent<Animator>().SetBool("isOpen", true);
        }

        if(!active && psPlaying)
        {
            ps.Stop();
            psPlaying = false;
            spining = true;
            source.Stop();
            GetComponent<Animator>().SetBool("isOpen", false);
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

    public void UnSuscribe()
    {
        PauseMenu.pauseShopMusic -= MuteMusic;
    }

    public void MuteMusic(bool state)
    {
        if (state)
        {
            if (source)
                source?.Pause();
        }
        else
        {
            if (source)
                source?.UnPause();
        }
    }
}
