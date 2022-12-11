using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundClip
{
    [SerializeField] public string id;
    [SerializeField] public AudioClip clip;
    [SerializeField] public bool loop;
    [SerializeField] public float volume;

    [HideInInspector] private AudioSource source;

    public void SetSource(AudioSource source, float range)
    {
        this.source = source;
        source.clip = clip;
        this.source.loop = loop;
        this.source.spatialBlend = 1;
        this.source.volume = volume;
        this.source.rolloffMode = AudioRolloffMode.Linear;
        this.source.maxDistance = range;
        this.source.playOnAwake = false;
    }
  

    public void Play()
    {
        if(source.enabled && !source.isPlaying )
            source.Play();
    }

    public void Pause()
    {
        if (source.enabled && source.isPlaying)
            source.Pause();
    }

    public void SetMute(bool state)
    {
        this.source.enabled = !state;
    }
}