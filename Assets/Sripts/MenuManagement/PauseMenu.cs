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
    public void PausarDespausar() { 
        
    }

    public void Resume() {
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    private void Pause()
    {
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}
