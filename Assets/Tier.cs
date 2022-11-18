using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Tier
{
    public int tierNumber;

    // Important! sum(Mele.probs + Ranged.probs + Special.probs) <= 1;
    [Header("Basic enemies:")]
    [Space]
    public float prob_melee;
    public GameObject[] melee;
    [Space]
    public float prob_ranged;
    public GameObject[] ranged;

    [Header("Special enemies:")]
    // Special.Probs is equal to: 1 - (Mele.probs + Ranged.probs)
    public GameObject[] special;

    // Returns an enemy using the probs setted
    public GameObject GetRandomEnemy() 
    {
        var value = Random.value;

        if (value < prob_melee)
            return melee[Random.Range(0, melee.Length)];
        else if (value < (prob_melee + prob_ranged))
            return ranged[Random.Range(0, melee.Length)];
        else
            return special[Random.Range(0, melee.Length)];

    }


}