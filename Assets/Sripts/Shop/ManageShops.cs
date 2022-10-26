using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageShops : MonoBehaviour
{

    [SerializeField]
    private GameObject tienda;

    [SerializeField]
    private Transform[] posicionesIniciales;

    private GameObject player;

    private float[] attackSpeedUpgrades;
    void Start()
    {
        for (int i = 0; i < posicionesIniciales.Length; i++)
        {
            GameObject t = Instantiate(tienda, posicionesIniciales[i].position, Quaternion.identity);
        }


        // Inicializacion de ventajas
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void RefreshShop()
    {

    }



}
