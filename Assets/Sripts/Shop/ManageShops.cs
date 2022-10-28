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
    [SerializeField]
    private Sprite[] spritesGuns;

    // Display de título
    [SerializeField]
    private Text[] titleText;
    // Display de Sprites
    [SerializeField]
    private Image[] displaysAmmoAndGuns;
    [SerializeField]
    private Image[] displaysPerks;
    // Display de precio
    [SerializeField]
    private Text[] coinsText;


    // Precios
    [SerializeField]
    private int[] ammoPrices;    // En orden: Pistola, Subfusil, Rifle, Francotirador
    [SerializeField]                           
    private int[] gunPrices;     // En orden: Pistola, Subfusil, Rifle, Francotirador
    [SerializeField]                          
    private int[] ammoQuantity;  // En orden: Pistola, Subfusil, Rifle, Francotirador
    [SerializeField]
    private int[] perkInitPrices;     // En orden: rapid fire, laser shot, big shot, tactic vest, tactical boots, medic, electrical barrier, gunsmith
    [SerializeField]
    private int[] rapidFirePrices;
    [SerializeField]
    private int[] laserShotPrices;
    [SerializeField]
    private int[] bigShotPrices;
    [SerializeField]
    private int[] tacticVestPrices;
    [SerializeField]
    private int[] tacticalBootsPrices;
    [SerializeField]
    private int[] medicPrices;
    [SerializeField]
    private int[] electricalBarrierPrices;
    [SerializeField]
    private int[] gunsmithPrices;

    // Textos
    [SerializeField]
    private string[] gunTexts;
    [SerializeField]
    private string[] ammoTexts;
    [SerializeField]
    private string[] perksTexts;

    // Controladores de tienda
    int numGunsGenerated = 0;
    int numAmmoGenerated = 0;
    int numNewPerksGenerated = 0;
    int numOldPerksGenerated = 0;
    int[] gunsAmmoGeneratedIndexes = new int[3];
    int[] perksNewOldGeneratedIndexes = new int[3];

    void Start()
    {
        for (int i = 0; i < posicionesIniciales.Length; i++)
        {
            GameObject t = Instantiate(tienda, posicionesIniciales[i].position, Quaternion.identity);
        }
    }

    public void RefreshShop()
    {
        Debug.Log("NUEVOS OBJETOS!");

        int numGunsHasPlayer = 0;

        // -------------------------------------------------------------------//
        // MUNICION Y ARMAS
        // -------------------------------------------------------------------//
        bool[] playerGuns = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<ShootSystem>().availableGuns;

        for (int i = 0; i < playerGuns.Length; i++)
        {
            if (playerGuns[i])
            {
                numGunsHasPlayer++;
            }
        }

        // En caso de que el jugador no tenga compradas todas las armas, se ofrece alguna opción
        if (numGunsHasPlayer == 1) // Solo tiene la pistola, por lo que la tienda ofrece 3 armas ya que no podrá comprar munición (la de la pistola es infinita)
        {
            int numGeneratedGuns = 0;
            int[] generatedGunsIndexes = new int[3];
            bool[] generatedInShopGuns = (bool[])playerGuns.Clone();
            while (numGeneratedGuns != 3)
            {
                int idx = Random.Range(1, playerGuns.Length); // Se genera a partir del 1 ya que el indice 0 es la propia pistola
                if (!generatedInShopGuns[idx])
                { // Si el arma no ha sido ya generada en la tienda se genera dicha arma
                    generatedGunsIndexes[numGeneratedGuns] = idx;
                    generatedInShopGuns[idx] = true;
                    gunsAmmoGeneratedIndexes[numGeneratedGuns] = idx;
                    numGeneratedGuns++;
                }
            }

            // En este punto tenemos los indices de las armas a mostrar en tienda por lo que actualizamos la UI
            for (int i = 0; i < generatedGunsIndexes.Length; i++)
            {
                titleText[i].text = gunTexts[generatedGunsIndexes[i]];
                displaysAmmoAndGuns[i].sprite = spritesGuns[generatedGunsIndexes[i]];
                coinsText[i].text = gunPrices[generatedGunsIndexes[i]].ToString();
            }

            numGunsGenerated = 3;
            numAmmoGenerated = 0;
        }   
        else if (numGunsHasPlayer == 2) // Solo tiene la pistola y un arma más, por lo que se ofrecen 2 armas y munición de la que ya se tiene
        {
            // Generacion de armas
            int numGeneratedGuns = 0;
            int[] generatedGunsIndexes = new int[2];
            bool[] generatedInShopGuns = (bool[])playerGuns.Clone();
            while (numGeneratedGuns != 2)
            {
                int idx = Random.Range(1, playerGuns.Length);
                if (!generatedInShopGuns[idx])
                {
                    generatedGunsIndexes[numGeneratedGuns] = idx;
                    generatedInShopGuns[idx] = true;
                    gunsAmmoGeneratedIndexes[numGeneratedGuns] = idx;
                    numGeneratedGuns++;
                }
            }

            // Generación de munición
            int ammoIdx = 0;
            for (int i = 1; i < playerGuns.Length; i++)
            {
                if (playerGuns[i])
                {
                    ammoIdx = i;
                    gunsAmmoGeneratedIndexes[2] = i;
                    break;
                }
            }

            // Actualizacion de UI de armas
            for (int i = 0; i < generatedGunsIndexes.Length; i++)
            {
                titleText[i].text = gunTexts[generatedGunsIndexes[i]];
                displaysAmmoAndGuns[i].sprite = spritesGuns[generatedGunsIndexes[i]];
                coinsText[i].text = gunPrices[generatedGunsIndexes[i]].ToString();
            }

            // Actualizacion de UI de municion
            titleText[2].text = ammoTexts[ammoIdx];
            displaysAmmoAndGuns[2].sprite = spritesAmmo[ammoIdx];
            coinsText[2].text = ammoPrices[ammoIdx].ToString();

            numGunsGenerated = 2;
            numAmmoGenerated = 1;
        }
        else if (numGunsHasPlayer == 3 && playerGuns[0]) // Se tienen todas las armas y una de ellas es la pistola, por lo que se ofrece un arma para cambiar por la pistola
        {
            // Generacion de armas
            int numGeneratedGuns = 0;
            int generatedGunIndex = 0;
            bool[] generatedInShopGuns = (bool[])playerGuns.Clone();
            while (numGeneratedGuns != 1)
            {
                int idx = Random.Range(1, playerGuns.Length);
                if (!generatedInShopGuns[idx])
                {
                    generatedGunIndex = idx;
                    gunsAmmoGeneratedIndexes[0] = idx;
                    numGeneratedGuns++;
                }
            }

            // Generacion de municion
            int numGeneratedAmmo = 0;
            int[] generatedAmmoIndexes = new int[2];
            bool[] generatedInShopAmmo = new bool[playerGuns.Length];
            while (numGeneratedAmmo != 2)
            {
                int idx = Random.Range(1, playerGuns.Length);
                if (!generatedInShopAmmo[idx] && playerGuns[idx]) // Si la municion no ha sido generada y el arma la posee el jugador
                {
                    generatedAmmoIndexes[numGeneratedAmmo] = idx;
                    generatedInShopAmmo[idx] = true;
                    gunsAmmoGeneratedIndexes[numGeneratedGuns + 1] = idx;
                    numGeneratedAmmo++;
                }
            }


            // Actualizacion de UI de armas
            titleText[0].text = gunTexts[generatedGunIndex];
            displaysAmmoAndGuns[0].sprite = spritesGuns[generatedGunIndex];
            coinsText[0].text = gunPrices[generatedGunIndex].ToString();

            // Actualizacion de UI de municion
            for (int i = 1; i < 3; i++)
            {
                titleText[i].text = ammoTexts[generatedAmmoIndexes[i - 1]];
                displaysAmmoAndGuns[i].sprite = spritesAmmo[generatedAmmoIndexes[i - 1]];
                coinsText[i].text = ammoPrices[generatedAmmoIndexes[i - 1]].ToString();
            }

            numGunsGenerated = 1;
            numAmmoGenerated = 2;
        }
        else if (numGunsHasPlayer == 3 && !playerGuns[0]) // El jugador tiene ya 3 armas, sienda una de ellas distinta a la pistola por lo que se ofrecen los 3 tipos de munición
        {
            // Se muestran las municiones de las armas que tiene el jugador
            int ammoGenerated = 0;
            for (int i = 0; i < playerGuns.Length; i++)
            {
                if (playerGuns[i])
                {
                    titleText[ammoGenerated].text = ammoTexts[i];
                    displaysAmmoAndGuns[ammoGenerated].sprite = spritesAmmo[i];
                    coinsText[ammoGenerated].text = ammoPrices[i].ToString();

                    gunsAmmoGeneratedIndexes[ammoGenerated] = i;
                    ammoGenerated++;
                }
            }

            numGunsGenerated = 0;
            numAmmoGenerated = 3;
        }



        // -------------------------------------------------------------------//
        // PERKS/VENTAJAS
        // -------------------------------------------------------------------//
        int numPerksHasPlayer = 0;
        bool[] playerPerks = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PerksManager>().availablePerks;
        int[] playerPerkLevels = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PerksManager>().perkLevels;
        for (int i = 0; i < playerPerks.Length; i++)
        {
            if (playerPerks[i])
            {
                numPerksHasPlayer++;
            }
        }

        // Se mostrarán únicamente ventajas nuevas en caso de que el jugador no tenga 3 por el momento
        if (numPerksHasPlayer < 3)
        {
            // Generacion de perks a mostrar en tienda
            int numNewGeneratedPerks = 0;
            int[] generatedPerksIndexes = new int[3];
            bool[] generatedInShopPerks = (bool[])playerPerks.Clone();

            while (numNewGeneratedPerks != 3)
            {
                int idx = Random.Range(0, playerPerks.Length);
                if (!generatedInShopPerks[idx])
                {
                    generatedPerksIndexes[numNewGeneratedPerks] = idx;
                    generatedInShopPerks[idx] = true;
                    perksNewOldGeneratedIndexes[numNewGeneratedPerks] = idx;
                    numNewGeneratedPerks++;
                }
            }

            // Mostrar en tienda ventajas nuevas
            for (int i = 0; i < generatedPerksIndexes.Length; i++)
            {
                titleText[i + 3].text = perksTexts[generatedPerksIndexes[i]];
                displaysPerks[i].sprite = spritesPerks[generatedPerksIndexes[i]];
                coinsText[i + 3].text = perkInitPrices[generatedPerksIndexes[i]].ToString();
            }

            numNewPerksGenerated = 3;
            numOldPerksGenerated = 0;
        }
        else if (numPerksHasPlayer == 3) // Caso de que al jugador le quedan 2 ventajas por conseguir. Muestra 1 para mejorar y 2 para comprar nuevas.
        {
            // Generación de nuevas ventajas
            int numNewGeneratedPerks = 0;
            int[] generatedPerksIndexes = new int[2];
            bool[] generatedInShopPerks = (bool[])playerPerks.Clone();

            while (numNewGeneratedPerks != 2)
            {
                int idx = Random.Range(0, playerPerks.Length);
                if (!generatedInShopPerks[idx])
                {
                    generatedPerksIndexes[numNewGeneratedPerks] = idx;
                    generatedInShopPerks[idx] = true;
                    perksNewOldGeneratedIndexes[numNewGeneratedPerks] = idx;
                    numNewGeneratedPerks++;
                }
            }

            // Generacion de perk existente (que no esté a nivel máximo)
            int idxExistentPerk = 0;
            bool generated = false;
            while (!generated)
            {
                int idx = Random.Range(0, playerPerks.Length);
                if (playerPerks[idx] && (playerPerkLevels[idx] != 5))
                {
                    idxExistentPerk = idx;
                    generated = true;
                    perksNewOldGeneratedIndexes[2] = idx;
                }
            }

            // Mostrar en tienda ventajas nuevas
            for (int i = 0; i < 2; i++)
            {
                titleText[i + 3].text = perksTexts[generatedPerksIndexes[i]];
                displaysPerks[i].sprite = spritesPerks[generatedPerksIndexes[i]];
                coinsText[i + 3].text = perkInitPrices[generatedPerksIndexes[i]].ToString();
            }

            // Mostrar en tienda la ya existente
            titleText[5].text = perksTexts[idxExistentPerk];
            displaysPerks[2].sprite = spritesPerks[idxExistentPerk];
            // Muestra el dinero de la mejora que le corresponde
            if (playerPerkLevels[idxExistentPerk] == 5)
            {
                coinsText[5].text = "MAX.";
            }
            else
            {
                switch (idxExistentPerk)
                {
                    case 0:
                        coinsText[5].text = (rapidFirePrices[playerPerkLevels[idxExistentPerk]]).ToString();
                        break;
                    case 1:
                        coinsText[5].text = (laserShotPrices[playerPerkLevels[idxExistentPerk]]).ToString();
                        break;
                    case 2:
                        coinsText[5].text = (bigShotPrices[playerPerkLevels[idxExistentPerk]]).ToString();
                        break;
                    case 3:
                        coinsText[5].text = (tacticVestPrices[playerPerkLevels[idxExistentPerk]]).ToString();
                        break;
                    case 4:
                        coinsText[5].text = (tacticalBootsPrices[playerPerkLevels[idxExistentPerk]]).ToString();
                        break;
                    case 5:
                        coinsText[5].text = (medicPrices[playerPerkLevels[idxExistentPerk]]).ToString();
                        break;
                    case 6:
                        coinsText[5].text = (electricalBarrierPrices[playerPerkLevels[idxExistentPerk]]).ToString();
                        break;
                    case 7:
                        coinsText[5].text = (gunsmithPrices[playerPerkLevels[idxExistentPerk]]).ToString();
                        break;
                    default:
                        break;
                }
            }

            numNewPerksGenerated = 2;
            numOldPerksGenerated = 1;
        }
        else if (numPerksHasPlayer == 4) // Caso de que al jugador le quedan 1 ventaja por conseguir. Muestra 2 para mejorar y 1 para comprar nueva.
        {
            // Generación de nueva ventaja
            int numNewGeneratedPerks = 0;
            int generatedPerkIndex = 0;
            bool[] generatedInShopPerks = (bool[])playerPerks.Clone();

            while (numNewGeneratedPerks != 1)
            {
                int idx = Random.Range(0, playerPerks.Length);
                if (!generatedInShopPerks[idx])
                {
                    generatedPerkIndex = idx;
                    perksNewOldGeneratedIndexes[0] = idx;
                    numNewGeneratedPerks++;
                }
            }

            // Generacion de 2 perks existentes
            int[] idxExistentPerks = new int[2];
            int numPerksGenerated = 0;
            while (numPerksGenerated != 2)
            {
                int idx = Random.Range(0, playerPerks.Length);
                if (playerPerks[idx] && (playerPerkLevels[idx] != 5) && !generatedInShopPerks[idx])
                {
                    idxExistentPerks[numPerksGenerated] = idx;
                    generatedInShopPerks[idx] = true;
                    perksNewOldGeneratedIndexes[numPerksGenerated + 1] = idx;
                    numPerksGenerated++;
                }
            }

            // Mostrar en tienda ventajas nuevas
            titleText[3].text = perksTexts[generatedPerkIndex];
            displaysPerks[0].sprite = spritesPerks[generatedPerkIndex];
            coinsText[3].text = perkInitPrices[generatedPerkIndex].ToString();

            // Mostrar en tienda las ya existentes
            for (int i = 0; i < 2; i++)
            {
                titleText[i + 4].text = perksTexts[idxExistentPerks[i]];
                displaysPerks[i + 1].sprite = spritesPerks[idxExistentPerks[i]];
                // Muestra el dinero de la mejora que le corresponde
                if (playerPerkLevels[idxExistentPerks[i]] == 5)
                {
                    coinsText[i + 4].text = "MAX.";
                }
                else
                {
                    switch (idxExistentPerks[i])
                    {
                        case 0:
                            coinsText[i + 4].text = (rapidFirePrices[playerPerkLevels[idxExistentPerks[i]]]).ToString();
                            break;
                        case 1:
                            coinsText[i + 4].text = (laserShotPrices[playerPerkLevels[idxExistentPerks[i]]]).ToString();
                            break;
                        case 2:
                            coinsText[i + 4].text = (bigShotPrices[playerPerkLevels[idxExistentPerks[i]]]).ToString();
                            break;
                        case 3:
                            coinsText[i + 4].text = (tacticVestPrices[playerPerkLevels[idxExistentPerks[i]]]).ToString();
                            break;
                        case 4:
                            coinsText[i + 4].text = (tacticalBootsPrices[playerPerkLevels[idxExistentPerks[i]]]).ToString();
                            break;
                        case 5:
                            coinsText[i + 4].text = (medicPrices[playerPerkLevels[idxExistentPerks[i]]]).ToString();
                            break;
                        case 6:
                            coinsText[i + 4].text = (electricalBarrierPrices[playerPerkLevels[idxExistentPerks[i]]]).ToString();
                            break;
                        case 7:
                            coinsText[i + 4].text = (gunsmithPrices[playerPerkLevels[idxExistentPerks[i]]]).ToString();
                            break;
                        default:
                            break;
                    }
                }
            }

            numNewPerksGenerated = 1;
            numOldPerksGenerated = 2;
        }
        else // Caso de que el jugador tiene todas las ventajas. Muestra 3 para mejorar.
        {
            int numPerksNotMaxLevel = 0;
            for (int i = 0; i < playerPerkLevels.Length; i++)
            {
                numPerksNotMaxLevel++;
            }

            // Se generan las que se puedan, como máximo 3, de ventajas que no estén a máximo nivel
            bool[] generatedInShopPerks = (bool[])playerPerks.Clone();
            bool generated;
            int numPerksGenerated = 0;
            for (int i = 0; i < numPerksNotMaxLevel; i++)
            {
                generated = false;
                while (!generated)
                {
                    int idx = Random.Range(0, playerPerks.Length);
                    if (playerPerks[idx] && (playerPerkLevels[idx] != 5) && !generatedInShopPerks[idx])
                    {
                        generated = true;
                        generatedInShopPerks[idx] = true;

                        perksNewOldGeneratedIndexes[numPerksGenerated] = idx;
                        numPerksGenerated++;

                        // Display en UI
                        titleText[i + 4].text = perksTexts[idx];
                        displaysPerks[i + 1].sprite = spritesPerks[idx];
                        // Muestra el dinero de la mejora que le corresponde
                        switch (idx)
                        {
                            case 0:
                                coinsText[i + 4].text = (rapidFirePrices[playerPerkLevels[idx]]).ToString();
                                break;
                            case 1:
                                coinsText[i + 4].text = (laserShotPrices[playerPerkLevels[idx]]).ToString();
                                break;
                            case 2:
                                coinsText[i + 4].text = (bigShotPrices[playerPerkLevels[idx]]).ToString();
                                break;
                            case 3:
                                coinsText[i + 4].text = (tacticVestPrices[playerPerkLevels[idx]]).ToString();
                                break;
                            case 4:
                                coinsText[i + 4].text = (tacticalBootsPrices[playerPerkLevels[idx]]).ToString();
                                break;
                            case 5:
                                coinsText[i + 4].text = (medicPrices[playerPerkLevels[idx]]).ToString();
                                break;
                            case 6:
                                coinsText[i + 4].text = (electricalBarrierPrices[playerPerkLevels[idx]]).ToString();
                                break;
                            case 7:
                                coinsText[i + 4].text = (gunsmithPrices[playerPerkLevels[idx]]).ToString();
                                break;
                            default:
                                break;
                        }

                    }
                }

            }


            // Se generan el resto de ventajas ya subidas al máximo nivel para completar la UI
            for (int i = numPerksNotMaxLevel; i < 3; i++)
            {
                generated = false;
                while (!generated)
                {
                    int idx = Random.Range(0, playerPerks.Length);
                    if (playerPerks[idx] && (playerPerkLevels[idx] == 5))
                    {
                        titleText[i + 4].text = perksTexts[idx];
                        displaysPerks[i + 1].sprite = spritesPerks[idx];
                        coinsText[i + 4].text = "MAX.";
                    }
                }

            }

            numNewPerksGenerated = 0;
            numOldPerksGenerated = 3;
        }
    }

    public void BuyGunAmmo1()
    {
        int numGunsHasAlreadyPlayer = 0;
        bool[] playerHasGuns = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<ShootSystem>().availableGuns;
        for (int i = 0; i < playerHasGuns.Length; i++) {
            if (playerHasGuns[i]) { 
                numGunsHasAlreadyPlayer++;
            }
        }


        if (numGunsGenerated > 0)  // Se comprará un arma
        {
            if (playerHasGuns[gunsAmmoGeneratedIndexes[0]]) return; // Si hemos comprado el arma y la tienda todavia no se ha actualizado, salimos del método
            // Se compra el arma
            GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<ShootSystem>().ActivateGun(gunsAmmoGeneratedIndexes[0]);
            if (numGunsHasAlreadyPlayer == 3 && playerHasGuns[0]) // Si el jugador tiene 3 armas y una de ellas es la pistola, se sustituye la pistola por la nueva
            {
                GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<ShootSystem>().DeactivateGun(0);
            }

            // Le quitamos el dinero
            GameObject.FindGameObjectWithTag("Player").GetComponent<CoinSystem>().SpendCoin(gunPrices[gunsAmmoGeneratedIndexes[0]]);
        }
        else if(numGunsGenerated == 0) // Se comprará munición
        {
            int ammoQ = ammoQuantity[gunsAmmoGeneratedIndexes[0]];
            GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<ShootSystem>().guns.getGuns()[gunsAmmoGeneratedIndexes[0]].AddAmmo(ammoQ);
            GameObject.FindGameObjectWithTag("Player").GetComponent<CoinSystem>().SpendCoin(ammoPrices[gunsAmmoGeneratedIndexes[0]]);
        }
    }
    public void BuyGunAmmo2()
    {
        int numGunsHasAlreadyPlayer = 0;
        bool[] playerHasGuns = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<ShootSystem>().availableGuns;
        for (int i = 0; i < playerHasGuns.Length; i++)
        {
            if (playerHasGuns[i])
            {
                numGunsHasAlreadyPlayer++;
            }
        }

        if (numGunsGenerated > 1) // Se compra un arma
        {
            if (playerHasGuns[gunsAmmoGeneratedIndexes[1]]) return; // Si hemos comprado el arma y la tienda todavia no se ha actualizado, salimos del método

            if (numGunsHasAlreadyPlayer == 3 && playerHasGuns[0]) // Si el jugador tiene 3 armas y una de ellas es la pistola, se sustituye la pistola por la nueva
            {
                GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<ShootSystem>().DeactivateGun(0);
            }

            GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<ShootSystem>().ActivateGun(gunsAmmoGeneratedIndexes[1]);
            GameObject.FindGameObjectWithTag("Player").GetComponent<CoinSystem>().SpendCoin(gunPrices[gunsAmmoGeneratedIndexes[1]]);
        }
        else // Se compra munición 
        {
            int ammoQ = ammoQuantity[gunsAmmoGeneratedIndexes[1]];
            GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<ShootSystem>().guns.getGuns()[gunsAmmoGeneratedIndexes[1]].AddAmmo(ammoQ);
            GameObject.FindGameObjectWithTag("Player").GetComponent<CoinSystem>().SpendCoin(ammoPrices[gunsAmmoGeneratedIndexes[1]]);
        }
    }
    public void BuyGunAmmo3()
    {
        int numGunsHasAlreadyPlayer = 0;
        bool[] playerHasGuns = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<ShootSystem>().availableGuns;
        for (int i = 0; i < playerHasGuns.Length; i++)
        {
            if (playerHasGuns[i])
            {
                numGunsHasAlreadyPlayer++;
            }
        }

        if (numGunsGenerated > 2) // Se compra un arma
        {

            if (playerHasGuns[gunsAmmoGeneratedIndexes[2]]) return; // Si hemos comprado el arma y la tienda todavia no se ha actualizado, salimos del método
            if (numGunsHasAlreadyPlayer == 3 && playerHasGuns[0]) // Si el jugador tiene 3 armas y una de ellas es la pistola, se sustituye la pistola por la nueva
            {
                GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<ShootSystem>().DeactivateGun(0);
            }

            GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<ShootSystem>().ActivateGun(gunsAmmoGeneratedIndexes[2]);
            GameObject.FindGameObjectWithTag("Player").GetComponent<CoinSystem>().SpendCoin(gunPrices[gunsAmmoGeneratedIndexes[2]]);
        }
        else // Se compra munición
        {
            Debug.Log("Se compra munición");
            int ammoQ = ammoQuantity[gunsAmmoGeneratedIndexes[2]];
            GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<ShootSystem>().guns.getGuns()[gunsAmmoGeneratedIndexes[2]].AddAmmo(ammoQ);
            GameObject.FindGameObjectWithTag("Player").GetComponent<CoinSystem>().SpendCoin(ammoPrices[gunsAmmoGeneratedIndexes[2]]);
        }
    }
    public void BuyNewOldPerk1()
    {

        if (numNewPerksGenerated > 0)  // Se comprará una nueva ventaja
        {
            BuyNewPerk(perksNewOldGeneratedIndexes[0], 0);
        }
        else if (numNewPerksGenerated == 0) // Se mejorará una ventaja
        {
            UpgradePerk(perksNewOldGeneratedIndexes[0], 0);
        }

    }
    public void BuyNewOldPerk2()
    {
        if (numNewPerksGenerated > 1)
        {
            BuyNewPerk(perksNewOldGeneratedIndexes[1], 1);
        }
        else { // Se compra una mejora
            UpgradePerk(perksNewOldGeneratedIndexes[1], 1);
        }
    }
    public void BuyNewOldPerk3()
    {
        if (numNewPerksGenerated > 2) // Se compra una nueva ventaja
        {
            BuyNewPerk(perksNewOldGeneratedIndexes[2], 2);
        }
        else // Se mejora la ventaja existente
        {
            UpgradePerk(perksNewOldGeneratedIndexes[2], 2);
        }
    }

    private void BuyNewPerk(int idx, int place) {
        switch (idx)
        {
            case 0:
                GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PerksManager>().ActivateRapidFire();
                GameObject.FindGameObjectWithTag("Player").GetComponent<CoinSystem>().SpendCoin(perkInitPrices[0]);
                break;
            case 1:
                GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PerksManager>().ActivateLaserShot();
                GameObject.FindGameObjectWithTag("Player").GetComponent<CoinSystem>().SpendCoin(perkInitPrices[1]);
                break;
            case 2:
                GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PerksManager>().ActivateBigShot();
                GameObject.FindGameObjectWithTag("Player").GetComponent<CoinSystem>().SpendCoin(perkInitPrices[2]);
                break;
            case 3:
                GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PerksManager>().ActivateTacticVest();
                GameObject.FindGameObjectWithTag("Player").GetComponent<CoinSystem>().SpendCoin(perkInitPrices[3]);
                break;
            case 4:
                GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PerksManager>().ActivateTacticalBoots();
                GameObject.FindGameObjectWithTag("Player").GetComponent<CoinSystem>().SpendCoin(perkInitPrices[4]);
                break;
            case 5:
                GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PerksManager>().ActivateMedic();
                GameObject.FindGameObjectWithTag("Player").GetComponent<CoinSystem>().SpendCoin(perkInitPrices[5]);
                break;
            case 6:
                GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PerksManager>().ActivateElectricalBarrier();
                GameObject.FindGameObjectWithTag("Player").GetComponent<CoinSystem>().SpendCoin(perkInitPrices[6]);
                break;
            case 7:
                GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PerksManager>().ActivateGunsmith();
                GameObject.FindGameObjectWithTag("Player").GetComponent<CoinSystem>().SpendCoin(perkInitPrices[7]);
                break;
            default:
                break;
        }
    }
    private void UpgradePerk(int idx, int place) {
        int lvl = 0;
        switch (idx)
        {
            case 0:
                lvl = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PerksManager>().perkLevels[0];
                if (lvl == 4) return;
                GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PerksManager>().UpgradeRapidFire();
                GameObject.FindGameObjectWithTag("Player").GetComponent<CoinSystem>().SpendCoin(rapidFirePrices[lvl]);
                break;
            case 1:
                lvl = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PerksManager>().perkLevels[1];
                if (lvl == 4) return;
                GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PerksManager>().UpgradeLaserShot();
                GameObject.FindGameObjectWithTag("Player").GetComponent<CoinSystem>().SpendCoin(laserShotPrices[lvl]);
                break;
            case 2:
                lvl = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PerksManager>().perkLevels[2];
                if (lvl == 4) return;
                GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PerksManager>().UpgradeBigShot();
                GameObject.FindGameObjectWithTag("Player").GetComponent<CoinSystem>().SpendCoin(bigShotPrices[lvl]);
                break;
            case 3:
                lvl = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PerksManager>().perkLevels[3];
                if (lvl == 4) return;
                GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PerksManager>().UpgradeTacticVest();
                GameObject.FindGameObjectWithTag("Player").GetComponent<CoinSystem>().SpendCoin(tacticVestPrices[lvl]);
                break;
            case 4:
                lvl = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PerksManager>().perkLevels[4];
                if (lvl == 4) return;
                GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PerksManager>().UpgradeTacticalBoots();
                GameObject.FindGameObjectWithTag("Player").GetComponent<CoinSystem>().SpendCoin(tacticalBootsPrices[lvl]);
                break;
            case 5:
                lvl = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PerksManager>().perkLevels[5];
                if (lvl == 4) return;
                GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PerksManager>().UpgradeMedic();
                GameObject.FindGameObjectWithTag("Player").GetComponent<CoinSystem>().SpendCoin(medicPrices[lvl]);
                break;
            case 6:
                lvl = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PerksManager>().perkLevels[6];
                if (lvl == 4) return;
                GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PerksManager>().UpgradeElectricalBarrier();
                GameObject.FindGameObjectWithTag("Player").GetComponent<CoinSystem>().SpendCoin(electricalBarrierPrices[lvl]);
                break;
            case 7:
                lvl = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PerksManager>().perkLevels[7];
                if (lvl == 4) return;
                GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PerksManager>().UpgradeGunsmith();
                GameObject.FindGameObjectWithTag("Player").GetComponent<CoinSystem>().SpendCoin(gunsmithPrices[lvl]);
                break;
            default:
                break;
        }
    }
}
