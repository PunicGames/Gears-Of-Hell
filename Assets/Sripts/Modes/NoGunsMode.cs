using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoGunsMode : MonoBehaviour
{

    GameObject player;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<Health>().SetInvincibilityTime(0.5f);
        ShootSystem sS = player.GetComponentInChildren<ShootSystem>();

        for (int i = 0; i < sS.guns.getGuns().Length; i++)
        { 
            sS.guns.getGuns()[i].maxTotalBullets = 0;
            sS.guns.getGuns()[i].totalBullets = 0;
            sS.guns.getGuns()[i].bulletsLeftInMagazine = 0;
            sS.guns.getGuns()[i].magazineSize = 0;
            sS.guns.getGuns()[i].bulletDamage = 0;
        }

        Invoke("AddCoins", 10);
    }

    private void AddCoins() {
        player.GetComponent<CoinSystem>().AddCoin(10);
        Invoke("AddCoins", 10);
    }
}
