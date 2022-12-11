using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;



public class EnemySoundManager : MonoBehaviour
{
    public float range = 15;
    public AudioMixerGroup mixer;
    public List<SoundClip> sounds;

    private Dictionary<string,SoundClip> soundsDictionary;

    private void Awake()
    {
        soundsDictionary = new Dictionary<string, SoundClip>();
        foreach (var sound in sounds)
        { 
            var source = gameObject.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = mixer;
            source.dopplerLevel = 0.0f;
            sound.SetSource(source, range);
            soundsDictionary[sound.id] = sound;
        }
    }

    public void PlaySound(string name)
    {
        soundsDictionary[name].Play();
    }

    public void PauseSound(string name)
    {
        soundsDictionary[name].Pause();
    }
}
