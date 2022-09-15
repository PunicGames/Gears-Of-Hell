using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{

    public Transform[] spawnPoints;

    public GameObject[] enemies;

    public float spawnSpeed;

    private void Start()
    {
        InvokeRepeating("instantiateEnemy", 3.0f, spawnSpeed);


        //CancelInvoke("instantiateEnemy") para parar este metodo concreto
    }

    private void instantiateEnemy()
    {
        Debug.Log("Spawned");
        int spawnIndex = Random.Range(0, spawnPoints.Length);
        int enemyIndex = Random.Range(0, enemies.Length);

        Instantiate(enemies[enemyIndex], spawnPoints[spawnIndex].position, Quaternion.identity);
    }
}
