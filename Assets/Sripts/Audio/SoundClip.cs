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
    [HideInInspector] private bool muted = false;

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
        PauseMenu.pauseAllSounds += SetMute;
    }
  

    public void Play()
    {
        if(!source.isPlaying && !muted)
            source.Play();
    }

    public void PlayOverlaped()
    {
        if (!muted)
            source.Play();
    }

    public void Pause()
    {
        if (source.isPlaying)
            source.Pause();
    }

    public void SetMute(bool state)
    {
        if (state)
        {
            if(source) 
                source?.Pause();
        }
        else
        {
            if (source)
                source?.UnPause();

        }
        muted = state;
    }

    public void OnDestroy()
    {
        PauseMenu.pauseAllSounds -= SetMute;
    }
}