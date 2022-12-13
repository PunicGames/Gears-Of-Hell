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
    private void OnDestroy()
    {
        foreach(var sound in sounds)
        {
            sound.OnDestroy();
        }
    }

    public void PlaySound(string name)
    {
        SoundClip aux;
        soundsDictionary.TryGetValue(name,out  aux);
        aux?.Play();
    }

    public void OverlapedPlaySound(string name)
    {
        SoundClip aux;
        soundsDictionary.TryGetValue(name, out aux);
        aux?.PlayOverlaped();
    }

    public void PauseSound(string name)
    {
        SoundClip aux;
        var sound = soundsDictionary.TryGetValue(name, out aux);
        aux?.Pause();
    }
}
