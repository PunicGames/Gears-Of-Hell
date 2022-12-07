using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{

    // Bullets record
    [HideInInspector] public float precision;
    [HideInInspector] public int numBulletsHit;
    [HideInInspector] public int numBulletsMissed;

    // Gold record
    [HideInInspector] public int numGoldEarned;

    // Kills record
    [HideInInspector] public int numDefeatedEnemies;


    // Start is called before the first frame update
    void Start()
    {
        precision = 0f;
        numBulletsHit = 0;
        numBulletsMissed = 0;
        numDefeatedEnemies = 0;
        numGoldEarned = 0;
    }

}
