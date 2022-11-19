using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSFX : MonoBehaviour
{

    public AudioClip accept, cancel, toggle, buy;
    [HideInInspector]
    public AudioSource interaction;

    public void Awake()
    {
        interaction = GetComponent<AudioSource>();
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
    public void PlayBuy()
    {
        interaction.clip = buy;
        interaction.Play();
    }
}
