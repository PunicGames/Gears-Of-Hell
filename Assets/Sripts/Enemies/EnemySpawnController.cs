using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    // params
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject[] enemyList;
    [SerializeField] private List<GameObject> spawnList;

    [SerializeField] private int spawnCount;
    [SerializeField] private float minSpawnDelay;
    [SerializeField] private float maxSpawnDelay;

    private float spawnTime;
    private float timeZero;
    private bool spawning;

    [SerializeField] private int minSpawnSteps;
    [SerializeField] private int maxSpawnSteps;

    public void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        spawnList = new List<GameObject>();
        spawning = true;
    }

    public void Update()
    {
        SpawnLoop();
    }

    private void ResetTimer()
    {
        //timeZero = Time.time;
        spawnTime = Time.time + Random.Range(minSpawnDelay, maxSpawnDelay);
        spawning = false;
    }

    private Vector3 GetSpawnPosition(Vector2Int key)
    {
        var bp = WorldGenerator.GetBlueprint;
        var pos = key;
        var steps = Random.Range(minSpawnSteps, maxSpawnSteps + 1);
        
        while (steps > 0)
        {
            // node expansion
            var doors = bp[pos];
            List<int> options = new List<int>();

            for (int i = 0; i < doors.Length; i++)
                if (doors[i])
                    options.Add(i);

            // pick random option
            pos += WorldGenerator.moves[options[Random.Range(0, options.Count)]];

            steps--;
        }

        if (pos == key)
            return GetSpawnPosition(key);
        else
            return new Vector3(pos.x, 0.0f, pos.y);
    }

    private void SpawnEnemy()
    {
        var pl_pos = player.transform.position;
        var cellScale = WorldGenerator.cellScale;

        Vector2Int index = new Vector2Int(Mathf.RoundToInt(pl_pos.x / cellScale.x), Mathf.RoundToInt(pl_pos.z / cellScale.y));

        var position = GetSpawnPosition(index);
        var enemy = enemyList[Random.Range(0, enemyList.Length)];

        Debug_UI.Print("Spawning enemy at: " + position.ToString());
         
        Instantiate(enemy, position * cellScale, Quaternion.identity);

    }

    private void SpawnLoop()
    {
        if (spawning)
        {
            SpawnEnemy();
            ResetTimer();
        } else
            if (Time.time > spawnTime)
                spawning = true;
    }

}
