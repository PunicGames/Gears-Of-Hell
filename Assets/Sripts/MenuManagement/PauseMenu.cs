using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    
    // Variable a usar en otros scripts como para bajar el volumen o algo asi. Accesible desde otros scripts con PauseMenu.GameIsPaused;
    public static bool GameIsPaused = false; 
    public static bool TriggerPause = false; 

    public GameObject PauseMenuUI;
    public GameObject OptionsMenu;

    // Cursor
    [SerializeField] private Texture2D cursorSprite;
    private Vector2 cursorHotSpot;

    // Platform control
    private bool desktop;

    private void Awake()
    {
        // Platform
        if (Application.isMobilePlatform)
        {
            desktop = false;
        }
        else if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            desktop = true;
        }
    }

    private void Start()
    {
        cursorHotSpot = new Vector2(0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        // Poner con el Input System si se pulsa una tecla o no para lanzar el menu
        if (TriggerPause) { 
            TriggerPause = false;
            if (GameIsPaused)
            {
                Resume();
            }
            else {
                Pause();
            }
        }
    }

    public void Resume() {
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;

        //if(desktop)
        //    GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<ShootSystem>().ChangeCursorBack();
    }

    private void Pause()
    {
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        //if (desktop)
        //    Cursor.SetCursor(cursorSprite, cursorHotSpot, CursorMode.ForceSoftware);
    }


    public void LoadMenu()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        SceneManager.LoadScene("Menu");
    }
}
