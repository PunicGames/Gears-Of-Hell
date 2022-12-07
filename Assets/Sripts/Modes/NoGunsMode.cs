using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoGunsMode : MonoBehaviour
{

    GameObject player;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponentInChildren<ShootSystem>().guns.getGuns()[0].maxTotalBullets = 0;
        player.GetComponentInChildren<ShootSystem>().guns.getGuns()[0].totalBullets = 0;
        player.GetComponentInChildren<ShootSystem>().guns.getGuns()[0].bulletsLeftInMagazine = 0;
        player.GetComponentInChildren<ShootSystem>().guns.getGuns()[0].magazineSize = 0;
        player.GetComponentInChildren<ShootSystem>().guns.getGuns()[0].bulletDamage = 0;
        Invoke("AddCoins", 10);
    }

    private void AddCoins() {
        player.GetComponent<CoinSystem>().AddCoin(10);
        Invoke("AddCoins", 10);
    }
}
