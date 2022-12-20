using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class BossFightManager : MonoBehaviour
{

    private Health h;
    [SerializeField] GameObject text;
    [SerializeField] GameObject scene;

    private void Start()
    {
        h = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        
    }

    private void Update()
    {
        if(h.currentHealth <= 0)
        {
            text.GetComponent<TMP_Text>().text = "DEFEAT";
            Time.timeScale = 0.2f;
            scene.SetActive(true);
        }
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene("BossFight");
    }

    public void WinLevel()
    {
        text.GetComponent<TMP_Text>().text = "VICTORY";
        Time.timeScale = 0f;
        scene.SetActive(true);
    }
}
