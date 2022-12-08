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

    // Shop display
    [SerializeField]
    private TextMeshProUGUI shopCoins;

    // Resume game displays
    [SerializeField] private TextMeshProUGUI bulletsHit;
    [SerializeField] private TextMeshProUGUI bulletsMissed;
    [SerializeField] private TextMeshProUGUI accuracy;
    [SerializeField] private TextMeshProUGUI totalTime;
    [SerializeField] private TextMeshProUGUI goldEarnedT;
    [SerializeField] private TextMeshProUGUI defeatedEnemiesT;

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

        if(desktop)
            desktopUI.SetActive(false);
        else
            mobileUI.SetActive(false);

        shopCoins.text = GameObject.FindGameObjectWithTag("Player").GetComponent<CoinSystem>().totalCoinsInGame.ToString();

        // Cursor
        //if (desktop)
        //    Cursor.SetCursor(cursorSprite, cursorHotSpot, CursorMode.ForceSoftware);
    }

    public void HideShop() { 
        shopUI.SetActive(false);
        Time.timeScale = 1.0f;
        shooping = false;

        if(desktop)
            desktopUI.SetActive(true);
        else
            mobileUI.SetActive(true);

        // Cursor
        //if(desktop)
        //    GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<ShootSystem>().ChangeCursorBack();
    }

    public void FinishGame(int min, int secs, int bHit, int bMissed, int goldEarned, int defeatedEnemies) {
        Time.timeScale = 0.0f;

        if (desktop)
            mobileUI.SetActive(false);
        else
            desktopUI.SetActive(false);

        finPartida.SetActive(true);

        // Display resume values
        if(secs < 10)
            totalTime.text = min + " : 0" + secs;
        else
            totalTime.text = min + " : " + secs;

        bulletsHit.text = bHit.ToString();
        bulletsMissed.text = bMissed.ToString();
        if (bHit + bMissed == 0)
        {
            accuracy.text = "00.00%";
        }
        else { 
            float acc = ((float)bHit/((float)bHit + (float)bMissed))*100;
            accuracy.text = System.Math.Round(acc, 2) + "%";
        }
        goldEarnedT.text = goldEarned.ToString();
        defeatedEnemiesT.text = defeatedEnemies.ToString();
    }
}
