using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameRegistry : MonoBehaviour
{

    private float elapsedTime;
    private int minutes, seconds;

    // Display time
    private Text timeDisplay;

    // Shop
    [SerializeField]
    private GameObject shopManager;
    private bool generated;

    void Start()
    {
        elapsedTime = 0f;
        minutes = 0;
        seconds = 0;
        timeDisplay = GameObject.Find("GameTimer").GetComponent<Text>();
    }


    void Update()
    {
        // Temporizador
        elapsedTime += Time.deltaTime;
        minutes = (int)(elapsedTime * 0.0167); // Dividir entre 60 es lo mismo que multiplicar por 0.0167
        seconds = (int)(elapsedTime - (minutes * 60));

        // Minutos
        if (minutes >= 10)
            timeDisplay.text = minutes.ToString();
        else
            timeDisplay.text = "0" + minutes.ToString();
        // Segundos
        if (seconds >= 10)
            timeDisplay.text += " : " + seconds.ToString();
        else
            timeDisplay.text += " : 0" + seconds.ToString();



        // HAY QUE HACER PARA QUE SOLO ACTUALICE LA TIENDA UNA ÚNICA VEZ
        if (minutes == 0 && seconds == 5 && !generated) {
            Debug.Log("Nuevos objetos en tienda");
            shopManager.GetComponent<ManageShops>().RefreshShop();
            generated = true;
        }
        if (minutes == 0 && seconds == 6)
        {
            generated = false;
        }
            if (minutes == 0 && seconds == 10 && !generated)
        {
            Debug.Log("Nuevos objetos en tienda");
            shopManager.GetComponent<ManageShops>().RefreshShop();
            generated = true;
        }

    }
}
