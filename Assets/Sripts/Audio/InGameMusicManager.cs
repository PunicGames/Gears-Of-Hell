using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMusicManager : MonoBehaviour
{
    [SerializeField] AudioClip start, loop, end;

    AudioSource mainMusic, shopMusic;

    public void Awake()
    {
        var sources = GetComponents<AudioSource>();
        mainMusic = sources[0];
        mainMusic.clip = start;
        mainMusic.loop = false;
        mainMusic.Play();
        Invoke("SetLoop", start.length);

        shopMusic = sources[1];
    }

    public void SetLoop() 
    {
        mainMusic.clip = loop;
        mainMusic.loop = true;
        mainMusic.Play();
    }

    public void SetGameOverMusic()
    {
        mainMusic.clip = end;
        mainMusic.loop = false;
        mainMusic.Play();
    }

    public void PlayShopMusic()
    {
        SetMutePlayerSounds(false);
        SetMainMusic(true);
    }

    public void StopShopMusic()
    {
        SetMutePlayerSounds(true);
        SetMainMusic(false);
    }


    // False = Mute, True = Resume
    private void SetMutePlayerSounds(bool state)
    {

        var player = GameObject.FindGameObjectWithTag("Player");

        var sound = new List<AudioSource>(player.GetComponents<AudioSource>());

        sound.Add(player.GetComponentInChildren<AudioSource>());

        foreach (var i in sound) 
        {
            i.enabled = state;
        }
    }

    private void SetMainMusic(bool state)
    {
        if (state)
        {
            mainMusic.Pause();
            shopMusic.Play();
        }            
        else
        {
            mainMusic.UnPause();
            shopMusic.Stop();
        }
            
    }

}
