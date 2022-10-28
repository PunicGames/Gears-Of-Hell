using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
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
}
