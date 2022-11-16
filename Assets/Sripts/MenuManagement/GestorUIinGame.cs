using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestorUIinGame : MonoBehaviour
{

    public static GestorUIinGame guingame;

    [SerializeField]
    private GameObject desktopUI;
    [SerializeField]
    private GameObject mobileUI;
    [SerializeField]
    private GameObject shopUI;

    private bool desktop = true;

    public bool shooping;

    // Cursor
    [SerializeField] private Texture2D cursorSprite;
    private Vector2 cursorHotSpot;

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

        //desktop = false;

        
        if (!desktop)
        {
            mobileUI.SetActive(true);
            desktopUI.SetActive(false);
        }
        else
        {
            mobileUI.SetActive(false);
            desktopUI.SetActive(true);
        }
        
    }

    void Start()
    {
        /*
        if (!desktop) {
            mobileUI.SetActive(true);
        }
        */

        // Cursor
        cursorHotSpot = new Vector2(0, 0);
    }

    public void ShowShop() { 
        shopUI.SetActive(true);
        Time.timeScale = 0.0f;
        shooping = true;
        Cursor.SetCursor(cursorSprite, cursorHotSpot, CursorMode.Auto);
    }

    public void HideShop() { 
        shopUI.SetActive(false);
        Time.timeScale = 1.0f;
        shooping = false;
        GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<ShootSystem>().ChangeCursorBack();
    }
}
