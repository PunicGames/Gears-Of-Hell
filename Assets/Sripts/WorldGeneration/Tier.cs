using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Enemy 
{
    public GameObject prefab;
    public float prob;
}

[System.Serializable]
public class Tier
{
    public string name;

    public Enemy[] enemies;
    
    public GameObject GetRandomEnemy()
    {
        var value = Random.value;
        var aux = 0.0f;

        for (int i = 0; i < (enemies.Length - 1); i++)
        {
            aux += enemies[i].prob;
            if (value < aux) 
            {
                return enemies[i].prefab;
            }
        }

        return enemies[enemies.Length - 1].prefab;

    }

}