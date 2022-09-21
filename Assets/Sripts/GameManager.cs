using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public Image deviceColor;

    private void Start()
    {
        // DeviceType.Console || DeviceType.Desktop || DeviceType.Handheld) 
        if (Application.isMobilePlatform)
        {
            Debug.Log("Handheld");
            deviceColor.color = Color.yellow;
        }
        else { 
            if (SystemInfo.deviceType == DeviceType.Desktop)
            {
                Debug.Log("Desktop");
                deviceColor.color = Color.green;
            }
            else if (SystemInfo.deviceType == DeviceType.Console)
            {
                Debug.Log("Console");
                deviceColor.color = Color.red;
            }
        }
    }
}
