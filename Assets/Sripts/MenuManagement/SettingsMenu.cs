using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    private bool desktop = true;
    [SerializeField]GameManager gm;

    public AudioMixer audioMixer;

    // Toggles
    [SerializeField] private Toggle bloomToggle;
    [SerializeField] private Toggle colorGradingToggle;

    [SerializeField] private PostProcessingControler ppController;

    private void Awake()
    {
        if (Application.isMobilePlatform)
        {
            desktop = false;

        }
        else if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            desktop = true;

        }

        //desktop = false;
    }

    private void Start()
    {
        UpdateToggles();
    }

    public void SetVolume(float volume) {
        audioMixer.SetFloat("volume", volume);
    }

    public void SetQuality(int qualityIndex) {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

   

    public void SetBloom(bool option) {
        if (option)
            PlayerPrefs.SetInt("bloomEffect", 1);
        else
            PlayerPrefs.SetInt("bloomEffect", 0);


            ppController.UpdateBloom(option);

    }

    public void SetColorGrading(bool option)
    {
        if (option)
            PlayerPrefs.SetInt("colorGrading", 1);
        else
            PlayerPrefs.SetInt("colorGrading", 0);


        ppController.UpdateColorGrading(option);

    }
    public void SetAntialiassing(int option)
    {
        PlayerPrefs.SetInt("antialiasing", option);
        gm.SetAntialiassing(option);

    }

    private void UpdateToggles() {
        bloomToggle.isOn = PlayerPrefs.GetInt("bloomEffect") == 0 ? false : true;
        colorGradingToggle.isOn = PlayerPrefs.GetInt("colorGrading") == 0 ? false : true;
    }
}
