using LootLocker.Requests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
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
            StartCoroutine(InitializeLootLocker());
        }

        
        //DeviceDetection();
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

    public void SetAntialiassing(int option)
    {

        var pb = Camera.main.GetComponent<PostProcessLayer>();
        switch (option)
        {
            case 0:
                pb.antialiasingMode = PostProcessLayer.Antialiasing.None;
                break;
            case 1:
                pb.antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
                pb.fastApproximateAntialiasing.fastMode = true;
                break;
            case 2:
                pb.antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
                pb.fastApproximateAntialiasing.fastMode = false;
                break;
            case 3:
                pb.antialiasingMode = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;
                pb.subpixelMorphologicalAntialiasing.quality = SubpixelMorphologicalAntialiasing.Quality.Medium;
                break;

        }


    }
    public void OptionsInitilizer_DefaultValues()
    {
        //Modify prederences
        if (Application.isMobilePlatform)
        {
            PlayerPrefs.SetInt("bloomEffect", 1);
            PlayerPrefs.SetInt("colorGrading", 1);
            PlayerPrefs.SetInt("antialiasing", 1);
        }
        else if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            PlayerPrefs.SetInt("bloomEffect", 1);
            PlayerPrefs.SetInt("colorGrading", 1);
            PlayerPrefs.SetInt("antialiasing", 2);
            QualitySettings.SetQualityLevel(2);
        }

        
        SetAntialiassing(PlayerPrefs.GetInt("antialiasing"));
    }

    public IEnumerator InitializeLootLocker()
    {
        bool done = false;
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (response.success)
            {
                print("Setting up LootLocker");
                done = true;
            }
            else
            {
                print(response.Error);
                done = true;
            }
        });
        yield return new WaitWhile(() => done == false);
    }
}
