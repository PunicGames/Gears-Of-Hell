using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameRegistry : MonoBehaviour
{

    private float elapsedTime;
    [HideInInspector] public int minutes, seconds;

    // Display time
    private Text timeDisplay;

    // Shop
    [SerializeField]
    private GameObject shopManager;
    private bool generated;
    public float firstShopTime;
    public float shopActivationTime;

    private WorldGenerator wgScript;

    private void Awake()
    {
        wgScript = GameObject.Find("WorldGenerator").GetComponent<WorldGenerator>();
    }

    void Start()
    {
        elapsedTime = 0f;
        minutes = 0;
        seconds = 0;
        timeDisplay = GameObject.Find("GameTimer").GetComponent<Text>();

        //shopManager.GetComponent<ManageShops>().RefreshShop();
        InvokeRepeating("ChangeShop", firstShopTime, shopActivationTime);
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

    }

    void ChangeShop()
    {

        int lastIndexOfActiveShop = 0;

        for (int i = 0; i < wgScript.shops.Count; i++) {
            if (wgScript.shops[i].GetComponent<Shop>().active) { 
                lastIndexOfActiveShop = i;
                wgScript.shops[i].GetComponent<Shop>().active = false;
            }
        }

        //calculamos el indice de la nueva a activar
        bool generated = false;
        int idx = Random.Range(0, wgScript.shops.Count);
        while (!generated) {
            if (idx != lastIndexOfActiveShop)
            {
                generated = true;
                wgScript.shops[idx].GetComponent<Shop>().active = true;
            }
            else {
                idx = Random.Range(0, wgScript.shops.Count);
            }
        }

        shopManager.GetComponent<ManageShops>().RefreshShop();
    }
}
