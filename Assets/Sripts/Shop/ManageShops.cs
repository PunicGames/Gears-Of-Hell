using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManageShops : MonoBehaviour
{
    // Prefab Tienda
    [SerializeField]
    private GameObject tienda;

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
    int numNewPerksGenerated = 0;
    int[] gunsAmmoGeneratedIndexes = new int[3];
    int[] perksNewOldGeneratedIndexes = new int[3];


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

        Debug.Log("Num Perks Has player" + numPerksHasPlayer);
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
            if (playerPerkLevels[idxExistentPerk] == 4)
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

        }
        else if (numPerksHasPlayer == 4) // Caso de que al jugador le quedan 1 ventaja por conseguir. Muestra 2 para mejorar y 1 para comprar nueva.
        {
            // Generación de nueva ventaja
            int numNewGeneratedPerk = 0;
            int generatedPerkIndex = 0;
            bool[] generatedInShopPerks = (bool[])playerPerks.Clone();

            while (numNewGeneratedPerk != 1)
            {
                int idx = Random.Range(0, playerPerks.Length);
                if (!generatedInShopPerks[idx])
                {
                    generatedPerkIndex = idx;
                    perksNewOldGeneratedIndexes[0] = idx;
                    numNewGeneratedPerk++;
                }
            }

            

            // Generacion de 2 perks existentes
            int[] idxExistentPerks = new int[2];
            int numPerksGenerated = 0;
            bool[] generatedExistenPerks = new bool[playerPerks.Length];
            while (numPerksGenerated != 2)
            {
                int idx = Random.Range(0, playerPerks.Length);
                if (playerPerks[idx] && !generatedExistenPerks[idx]) // (playerPerks[idx] && (playerPerkLevels[idx] != 4) && !generatedInShopPerks[idx])
                {
                    idxExistentPerks[numPerksGenerated] = idx;
                    generatedExistenPerks[idx] = true;
                    perksNewOldGeneratedIndexes[numPerksGenerated + 1] = idx;
                }
                numPerksGenerated++;
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
                if (playerPerkLevels[idxExistentPerks[i]] == 4)
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


        }
        else // Caso de que el jugador tiene todas las ventajas. Muestra 3 para mejorar o muestra ya mejoradas para ocupar huecos restantes
        {
            int numPerksNotMaxLevel = 0;
            List<int> indexes = new List<int>();

            for (int i = 0; i < playerPerkLevels.Length; i++)
            {
                if (playerPerkLevels[i] != 4 && playerPerks[i]) { // No maxeada y que tenga el jugador
                    numPerksNotMaxLevel++;
                }
            }

            // Se generan las ventajas que no estén al máximo nivel
            if (numPerksNotMaxLevel != 0) { 
                bool generated = false;
                for (int i = 0; i < numPerksNotMaxLevel; i++) {
                    generated = false;
                    while (!generated) {
                        int idx = Random.Range(0, playerPerks.Length);
                        if (playerPerks[idx] && (playerPerkLevels[idx] != 4)) { 
                            generated = true;
                            indexes.Add(idx);
                        }
                    }
                }
            }

            // Se generan las que están al máximo nivel
            if (indexes.Count != 3) {
                bool generated = false;
                for (int i = numPerksNotMaxLevel; i < 3; i++) {
                    generated = false;
                    while (!generated)
                    {
                        int idx = Random.Range(0, playerPerks.Length);
                        if (playerPerks[idx] && (playerPerkLevels[idx] == 4))
                        {
                            generated = true;
                            indexes.Add(idx);
                        }
                    }
                }
            }

            // Se prepara la UI
            for (int i = 0; i < 3; i++) {
                perksNewOldGeneratedIndexes[i] = indexes[i];

                // Display en UI
                titleText[i + 3].text = perksTexts[indexes[i]];
                displaysPerks[i].sprite = spritesPerks[indexes[i]];

                // Muestra el dinero de la mejora que le corresponde
                if (playerPerkLevels[indexes[i]] == 4)
                {
                    coinsText[i + 3].text = "MAX LVL";
                    continue;
                }
                else { 
                    // Si no está al máximo nivel
                    switch (indexes[i])
                    {
                        case 0:
                                coinsText[i + 3].text = (rapidFirePrices[playerPerkLevels[indexes[i]]]).ToString();
                            break;
                        case 1:
                                coinsText[i + 3].text = (laserShotPrices[playerPerkLevels[indexes[i]]]).ToString();
                            break;
                        case 2:
                                coinsText[i + 3].text = (bigShotPrices[playerPerkLevels[indexes[i]]]).ToString();
                            break;
                        case 3:
                                coinsText[i + 3].text = (tacticVestPrices[playerPerkLevels[indexes[i]]]).ToString();
                            break;
                        case 4:
                                coinsText[i + 3].text = (tacticalBootsPrices[playerPerkLevels[indexes[i]]]).ToString();
                            break;
                        case 5:
                                coinsText[i + 3].text = (medicPrices[playerPerkLevels[indexes[i]]]).ToString();
                            break;
                        case 6:
                                coinsText[i + 3].text = (electricalBarrierPrices[playerPerkLevels[indexes[i]]]).ToString();
                            break;
                        case 7:
                                coinsText[i + 3].text = (gunsmithPrices[playerPerkLevels[indexes[i]]]).ToString();
                            break;
                        default:
                            break;
                    }
                }

            }

            numNewPerksGenerated = 0;
        
        }
        
        
    }

    public void BuyGunAmmo1()
    {
        if (GameObject.FindGameObjectWithTag("Player").GetComponent<CoinSystem>().totalCoinsInGame < gunPrices[gunsAmmoGeneratedIndexes[0]]) return; // Si el jugador no tiene suficientes monedas no compra nada


        int numGunsHasAlreadyPlayer = 0;
        bool[] playerHasGuns = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<ShootSystem>().availableGuns;
        for (int i = 0; i < playerHasGuns.Length; i++)
        {
            if (playerHasGuns[i])
            {
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
        else if (numGunsGenerated == 0) // Se comprará munición
        {
            int ammoQ = ammoQuantity[gunsAmmoGeneratedIndexes[0]];
            GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<ShootSystem>().guns.getGuns()[gunsAmmoGeneratedIndexes[0]].AddAmmo(ammoQ);
            GameObject.FindGameObjectWithTag("Player").GetComponent<CoinSystem>().SpendCoin(ammoPrices[gunsAmmoGeneratedIndexes[0]]);
        }
    }
    public void BuyGunAmmo2()
    {
        if (GameObject.FindGameObjectWithTag("Player").GetComponent<CoinSystem>().totalCoinsInGame < gunPrices[gunsAmmoGeneratedIndexes[1]]) return;

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
        if (GameObject.FindGameObjectWithTag("Player").GetComponent<CoinSystem>().totalCoinsInGame < gunPrices[gunsAmmoGeneratedIndexes[2]]) return;

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
            if (GameObject.FindGameObjectWithTag("Player").GetComponent<CoinSystem>().totalCoinsInGame < perkInitPrices[perksNewOldGeneratedIndexes[0]]) return;
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
            if (GameObject.FindGameObjectWithTag("Player").GetComponent<CoinSystem>().totalCoinsInGame < perkInitPrices[perksNewOldGeneratedIndexes[1]]) return;
            BuyNewPerk(perksNewOldGeneratedIndexes[1], 1);
        }
        else
        { // Se compra una mejora
            UpgradePerk(perksNewOldGeneratedIndexes[1], 1);
        }
    }
    public void BuyNewOldPerk3()
    {
        if (numNewPerksGenerated > 2) // Se compra una nueva ventaja
        {
            if (GameObject.FindGameObjectWithTag("Player").GetComponent<CoinSystem>().totalCoinsInGame < perkInitPrices[perksNewOldGeneratedIndexes[2]]) return;
            BuyNewPerk(perksNewOldGeneratedIndexes[2], 2);
        }
        else // Se mejora la ventaja existente
        {
            UpgradePerk(perksNewOldGeneratedIndexes[2], 2);
        }
    }

    private void BuyNewPerk(int idx, int place)
    {
        int numPerks = 0;
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        bool[] perksHasPlayer = player.GetComponentInChildren<PerksManager>().availablePerks;
        for (int i = 0; i < perksHasPlayer.Length; i++) {
            if (perksHasPlayer[i])
                numPerks++;
        }

        if (numPerks == 5) return; // Si el jugador ya tiene 5 ventajas no compra una nueva
        if (player.GetComponentInChildren<PerksManager>().availablePerks[idx]) return; // Si la ventaja la acabamos de comprar, ya no lo podremos hacer más
        switch (idx)
        {
            case 0:
                player.GetComponentInChildren<PerksManager>().ActivateRapidFire();
                player.GetComponent<CoinSystem>().SpendCoin(perkInitPrices[0]);
                break;
            case 1:
                player.GetComponentInChildren<PerksManager>().ActivateLaserShot();
                player.GetComponent<CoinSystem>().SpendCoin(perkInitPrices[1]);
                break;
            case 2:
                player.GetComponentInChildren<PerksManager>().ActivateBigShot();
                player.GetComponent<CoinSystem>().SpendCoin(perkInitPrices[2]);
                break;
            case 3:
                player.GetComponentInChildren<PerksManager>().ActivateTacticVest();
                player.GetComponent<CoinSystem>().SpendCoin(perkInitPrices[3]);
                break;
            case 4:
                player.GetComponentInChildren<PerksManager>().ActivateTacticalBoots();
                player.GetComponent<CoinSystem>().SpendCoin(perkInitPrices[4]);
                break;
            case 5:
                player.GetComponentInChildren<PerksManager>().ActivateMedic();
                player.GetComponent<CoinSystem>().SpendCoin(perkInitPrices[5]);
                break;
            case 6:
                player.GetComponentInChildren<PerksManager>().ActivateElectricalBarrier();
                player.GetComponent<CoinSystem>().SpendCoin(perkInitPrices[6]);
                break;
            case 7:
                player.GetComponentInChildren<PerksManager>().ActivateGunsmith();
                player.GetComponent<CoinSystem>().SpendCoin(perkInitPrices[7]);
                break;
            default:
                break;
        }
    }
    private void UpgradePerk(int idx, int place)
    {
        int lvl = 0;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        switch (idx)
        {
            case 0:
                lvl = player.GetComponentInChildren<PerksManager>().perkLevels[0];
                if (GameObject.FindGameObjectWithTag("Player").GetComponent<CoinSystem>().totalCoinsInGame < rapidFirePrices[lvl]) return;
                if (lvl == 4) return;
                player.GetComponentInChildren<PerksManager>().UpgradeRapidFire();
                player.GetComponent<CoinSystem>().SpendCoin(rapidFirePrices[lvl]);

                // Si con la subida de nivel alcanza el máximo. Eso es, cuando es nivel 3 y entramos en este método haciendo que suba al máximo.
                if(lvl == 3)
                    UpdateUi(place, perksTexts[idx], "MAX");
                else
                    UpdateUi(place, perksTexts[idx], (rapidFirePrices[lvl + 1]).ToString());
                break;
            case 1:
                lvl = player.GetComponentInChildren<PerksManager>().perkLevels[1];
                if (GameObject.FindGameObjectWithTag("Player").GetComponent<CoinSystem>().totalCoinsInGame < laserShotPrices[lvl]) return;
                if (lvl == 4) return; // Si ya esta al máximo nivel, salimos del método
                player.GetComponentInChildren<PerksManager>().UpgradeLaserShot();
                player.GetComponent<CoinSystem>().SpendCoin(laserShotPrices[lvl]);

                // Si con la subida de nivel alcanza el máximo
                if (lvl == 3)
                    UpdateUi(place, perksTexts[idx], "MAX");
                else
                    UpdateUi(place, perksTexts[idx], (rapidFirePrices[lvl + 1]).ToString());
                break;
            case 2:
                lvl = player.GetComponentInChildren<PerksManager>().perkLevels[2];
                if (GameObject.FindGameObjectWithTag("Player").GetComponent<CoinSystem>().totalCoinsInGame < bigShotPrices[lvl]) return;
                if (lvl == 4) return;// Si ya esta al máximo nivel, salimos del método
                player.GetComponentInChildren<PerksManager>().UpgradeBigShot();
                player.GetComponent<CoinSystem>().SpendCoin(bigShotPrices[lvl]);

                // Si con la subida de nivel alcanza el máximo
                if (lvl == 3)
                    UpdateUi(place, perksTexts[idx], "MAX");
                else
                    UpdateUi(place, perksTexts[idx], (rapidFirePrices[lvl + 1]).ToString());
                break;
            case 3:
                lvl = player.GetComponentInChildren<PerksManager>().perkLevels[3];
                if (GameObject.FindGameObjectWithTag("Player").GetComponent<CoinSystem>().totalCoinsInGame < tacticVestPrices[lvl]) return;
                if (lvl == 4) return;// Si ya esta al máximo nivel, salimos del método
                player.GetComponentInChildren<PerksManager>().UpgradeTacticVest();
                player.GetComponent<CoinSystem>().SpendCoin(tacticVestPrices[lvl]);

                // Si con la subida de nivel alcanza el máximo
                if (lvl == 3)
                    UpdateUi(place, perksTexts[idx], "MAX");
                else
                    UpdateUi(place, perksTexts[idx], (rapidFirePrices[lvl + 1]).ToString());
                break;
            case 4:
                lvl = player.GetComponentInChildren<PerksManager>().perkLevels[4];
                if (GameObject.FindGameObjectWithTag("Player").GetComponent<CoinSystem>().totalCoinsInGame < tacticalBootsPrices[lvl]) return;
                if (lvl == 4) return;// Si ya esta al máximo nivel, salimos del método
                player.GetComponentInChildren<PerksManager>().UpgradeTacticalBoots();
                player.GetComponent<CoinSystem>().SpendCoin(tacticalBootsPrices[lvl]);

                // Si con la subida de nivel alcanza el máximo
                if (lvl == 3)
                    UpdateUi(place, perksTexts[idx], "MAX");
                else
                    UpdateUi(place, perksTexts[idx], (rapidFirePrices[lvl + 1]).ToString());
                break;
            case 5:
                lvl = player.GetComponentInChildren<PerksManager>().perkLevels[5];
                if (GameObject.FindGameObjectWithTag("Player").GetComponent<CoinSystem>().totalCoinsInGame < medicPrices[lvl]) return;
                if (lvl == 4) return;// Si ya esta al máximo nivel, salimos del método
                player.GetComponentInChildren<PerksManager>().UpgradeMedic();
                player.GetComponent<CoinSystem>().SpendCoin(medicPrices[lvl]);

                // Si con la subida de nivel alcanza el máximo
                if (lvl == 3)
                    UpdateUi(place, perksTexts[idx], "MAX");
                else
                    UpdateUi(place, perksTexts[idx], (rapidFirePrices[lvl + 1]).ToString());
                break;
            case 6:
                lvl = player.GetComponentInChildren<PerksManager>().perkLevels[6];
                if (GameObject.FindGameObjectWithTag("Player").GetComponent<CoinSystem>().totalCoinsInGame < electricalBarrierPrices[lvl]) return;
                if (lvl == 4) return;// Si ya esta al máximo nivel, salimos del método
                player.GetComponentInChildren<PerksManager>().UpgradeElectricalBarrier();
                player.GetComponent<CoinSystem>().SpendCoin(electricalBarrierPrices[lvl]);

                // Si con la subida de nivel alcanza el máximo
                if (lvl == 3)
                    UpdateUi(place, perksTexts[idx], "MAX");
                else
                    UpdateUi(place, perksTexts[idx], (rapidFirePrices[lvl + 1]).ToString());
                break;
            case 7:
                lvl = player.GetComponentInChildren<PerksManager>().perkLevels[7];
                if (GameObject.FindGameObjectWithTag("Player").GetComponent<CoinSystem>().totalCoinsInGame < gunsmithPrices[lvl]) return;
                if (lvl == 4) return;// Si ya esta al máximo nivel, salimos del método
                player.GetComponentInChildren<PerksManager>().UpgradeGunsmith();
                player.GetComponent<CoinSystem>().SpendCoin(gunsmithPrices[lvl]);

                // Si con la subida de nivel alcanza el máximo
                if (lvl == 3)
                    UpdateUi(place, perksTexts[idx], "MAX");
                else
                    UpdateUi(place, perksTexts[idx], (rapidFirePrices[lvl + 1]).ToString());
                break;
            default:
                break;
        }
    }

    private void UpdateUi(int place, string tT, string cT) {
        titleText[place + 3].text = tT;
        displaysPerks[place].sprite = spritesPerks[perksNewOldGeneratedIndexes[place]];
        coinsText[place + 3].text = cT;
    }
}
