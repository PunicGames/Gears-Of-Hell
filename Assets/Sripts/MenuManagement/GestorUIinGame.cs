using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestorUIinGame : MonoBehaviour
{

    public static GestorUIinGame guingame;

    [SerializeField]
    private GameObject mobileUI;
    [SerializeField]
    private GameObject shopUI;

    private bool desktop = true;

    public bool shooping;

    private void Awake()
    {
        // Deteccion de dispositivo
        if (Application.isMobilePlatform)
        {
            desktop = false;
        }
        else if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            desktop = true;
        }
    }

    void Start()
    {

        if (!desktop) {
            mobileUI.SetActive(true);
        }
    }

    public void ShowShop() { 
        shopUI.SetActive(true);
        Time.timeScale = 0.0f;
        shooping = true;
    }

    public void HideShop() { 
        shopUI.SetActive(false);
        Time.timeScale = 1.0f;
        shooping = false;
    }
}
