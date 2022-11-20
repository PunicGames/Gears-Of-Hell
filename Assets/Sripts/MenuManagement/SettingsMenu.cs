using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    private bool desktop = true;

    public AudioMixer audioMixer;

    [SerializeField] private GameObject PcControls;
    [SerializeField] private GameObject MobileControls;

    [SerializeField] private GameObject PcGameplay;
    [SerializeField] private GameObject MobileGameplay;

    private void Awake()
    {
        if (Application.isMobilePlatform)
        {
            desktop = false;
            PlayerPrefs.SetInt("bloomEffect", 0);
            GameObject.Find("PostProcessVolume").GetComponent<PostProcessingControler>().UpdateBloom(false);
        }
        else if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            desktop = true;
            PlayerPrefs.SetInt("bloomEffect", 1);
            GameObject.Find("PostProcessVolume").GetComponent<PostProcessingControler>().UpdateBloom(true);
        }

        //desktop = false;
    }

    public void SetVolume(float volume) {
        audioMixer.SetFloat("volume", volume);
    }

    public void SetQuality(int qualityIndex) {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void ActivateControlsScreen()
    {
        if (desktop)
        {
            PcControls.SetActive(true);
        }
        else
        {
            MobileControls.SetActive(true);
        }
    }

    public void DeactivateControlsScreen()
    {
        if (desktop)
        {
            PcControls.SetActive(false);
        }
        else
        {
            MobileControls.SetActive(false);
        }
    }

    public void ActivateGameplayScreen()
    {
        if (desktop)
        {
            PcGameplay.SetActive(true);
        }
        else
        {
            MobileGameplay.SetActive(true);
        }
    }

    public void DeactivateGameplayScreen()
    {
        if (desktop)
        {
            PcGameplay.SetActive(false);
        }
        else
        {
            MobileGameplay.SetActive(false);
        }
    }

    public void SetBloom(bool option) {
        if (option)
            PlayerPrefs.SetInt("bloomEffect", 1);
        else
            PlayerPrefs.SetInt("bloomEffect", 0);


            GameObject.Find("PostProcessVolume").GetComponent<PostProcessingControler>().UpdateBloom(option);

    }
}
