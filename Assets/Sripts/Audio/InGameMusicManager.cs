using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMusicManager : MonoBehaviour
{
    [SerializeField] AudioClip start, loop, end;
    
    AudioSource audioSource;

    public void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = start;
        audioSource.loop = false;
        audioSource.Play();
        Invoke("SetLoop", start.length);
    }

    public void SetLoop() 
    {
        audioSource.clip = loop;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void SetGameOverMusic()
    {
        audioSource.clip = end;
        audioSource.loop = false;
        audioSource.Play();
    }
}
