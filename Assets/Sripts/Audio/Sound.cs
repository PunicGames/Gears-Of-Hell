
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip[] clips;
    public AudioClip secundary;
    public AudioClip laser;

    [Range(0f, 1f)]
    public float volume;
    [Range(0.1f, 3f)]
    public float pitch;

    [HideInInspector]
    public AudioSource source;

    public void Play()
    {
        source.clip = clips[Random.Range(0, clips.Length)];
        source.Play();
    }

    public void PlaySecundary() 
    {
        source.clip = secundary;
        source.Play();
    }

    public void PlayLaser() 
    {
        source.clip = laser;
        source.Play();
    }
}
