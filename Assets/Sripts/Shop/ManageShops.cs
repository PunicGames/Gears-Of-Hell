using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManageShops : MonoBehaviour
{
    // Prefab Tienda
    [SerializeField]
    private GameObject tienda;

    // Posiciones de prueba
    [SerializeField]
    private Transform[] posicionesIniciales;

    // Imagenes de los objetos
    [SerializeField]
    private Sprite[] spritesAmmo;
    [SerializeField]
    private Sprite[] spritesPerks;

    // Display de título
    [SerializeField]
    private Text[] titleText;
    // Display de Sprites
    [SerializeField]
    private Image[] displaysAmmo;
    [SerializeField]
    private Image[] displaysPerks;
    // Display de precio
    [SerializeField]
    private Text[] coinsText;


    // Precios
    [SerializeField]
    private int[] ammoPrices; // En orden: Subfusil, Rifle, Francotirador

    void Start()
    {
        for (int i = 0; i < posicionesIniciales.Length; i++)
        {
            GameObject t = Instantiate(tienda, posicionesIniciales[i].position, Quaternion.identity);
        }

        // Prueba
        displaysAmmo[0].sprite = spritesAmmo[0];
        titleText[0].text = "PISTOLAA";
        coinsText[0].text = ammoPrices[0].ToString();

        // Al iniciar el juego genera una tienda inicial
        RefreshShop();
    }

    public void RefreshShop()
    {
        Debug.Log("NUEVOS OBJETOS!");

        // Municion



        // Perks
    }

    public void BuyAmmo1()
    {
        Debug.Log("Has comprado Ammo1");

        // Prueba
        GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<ShootSystem>().guns.AddAmmo(0, 300);
    }
    public void BuyAmmo2()
    {
        Debug.Log("Has comprado Ammo2");
    }
    public void BuyAmmo3()
    {
        Debug.Log("Has comprado Ammo3");
    }
    public void BuyPerk1()
    {
        Debug.Log("Has comprado Perk1");
    }
    public void BuyPerk2()
    {
        Debug.Log("Has comprado Perk2");
    }
    public void BuyPerk3()
    {
        Debug.Log("Has comprado Perk3");
    }
}
