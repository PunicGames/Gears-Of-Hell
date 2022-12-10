using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public AudioMixer audioMixer;

    private static float generalVolume;
    void Awake()
    {
        // Get Volume from options menu
        float value;
        bool result = audioMixer.GetFloat("volume", out value);
        if (result) generalVolume = (value + 80) / 80; // 80 tiene que ver con el audio mixer
        var source = gameObject.GetComponent<AudioSource>();
        foreach (Sound s in sounds)
        {
            s.source = source;
            s.source.clip = s.clips[0];
            s.source.volume = s.volume ;
            s.source.pitch = s.pitch;
        }
    }

    public void Play(string name) {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
    }

    public void Play(int idx) {
        sounds[idx].Play();
    }
    public void PlaySecundary(int idx)
    {
        sounds[idx].PlaySecundary();
    }
    public void PlayLaser(int idx)
    {
        sounds[idx].PlayLaser();
    }

    public static float getGeneralVolume()
    {
        return generalVolume;
    }
}
