using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GestorUIinGame : MonoBehaviour
{

    public static GestorUIinGame guingame;

    [SerializeField]
    private GameObject desktopUI;
    [SerializeField]
    private GameObject mobileUI;
    [SerializeField]
    private GameObject shopUI;
    [SerializeField]
    private GameObject finPartida;

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
        // Cursor
        cursorHotSpot = new Vector2(0, 0);
    }

    public void ShowShop() { 
        shopUI.SetActive(true);
        Time.timeScale = 0.0f;
        shooping = true;
        //if (desktop)
        //    Cursor.SetCursor(cursorSprite, cursorHotSpot, CursorMode.ForceSoftware);
    }

    public void HideShop() { 
        shopUI.SetActive(false);
        Time.timeScale = 1.0f;
        shooping = false;
        //if(desktop)
        //    GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<ShootSystem>().ChangeCursorBack();
    }

    public void FinishGame(int min, int secs) {
        Time.timeScale = 0.0f;

        if (desktop)
            mobileUI.SetActive(false);
        else
            desktopUI.SetActive(false);

        finPartida.SetActive(true);

        if(secs < 10)
            GameObject.Find("TotalTime").GetComponent<TextMeshProUGUI>().text = min + " : 0" + secs;
        else
            GameObject.Find("TotalTime").GetComponent<TextMeshProUGUI>().text = min + " : " + secs;
    }
}
