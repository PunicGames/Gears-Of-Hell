using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    static GameManager instance;

    private void Start()
    {
        if (instance != null) { 
            Destroy(gameObject);
            Debug.Log("Creado.");
        }
        else { 
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("Creando...");
            OptionsInitilizer_DefaultValues();
        }

        DeviceDetection();
    }

    private void DeviceDetection() { 
        // DeviceType.Console || DeviceType.Desktop || DeviceType.Handheld) 
        if (Application.isMobilePlatform)
        {
            Debug.Log("Handheld");
        }
        else { 
            if (SystemInfo.deviceType == DeviceType.Desktop)
            {
                Debug.Log("Desktop");
            }
            else if (SystemInfo.deviceType == DeviceType.Console)
            {
                Debug.Log("Console");
            }
        }
    }


    public void OptionsInitilizer_DefaultValues()
    {
        if (Application.isMobilePlatform)
        {
            PlayerPrefs.SetInt("bloomEffect", 1);
            PlayerPrefs.SetInt("colorGrading", 1);
        }
        else if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            PlayerPrefs.SetInt("bloomEffect", 1);
            PlayerPrefs.SetInt("colorGrading", 1);
        }
    }
}
