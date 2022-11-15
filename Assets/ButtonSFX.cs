using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSFX : MonoBehaviour
{

    public AudioClip accept, cancel, toggle;

    public AudioSource interaction;

    public void Awake()
    {
        var audioSource = GetComponents<AudioSource>();
        interaction = audioSource[1];
    }


    public void PlayAccept()
    {
        interaction.clip = accept;
        interaction.Play();
    }
    public void PlayCancel()
    {
        interaction.clip = cancel;
        interaction.Play();
    }
    public void PlayToggle()
    {
        interaction.clip = toggle;
        interaction.Play();
    }
}
