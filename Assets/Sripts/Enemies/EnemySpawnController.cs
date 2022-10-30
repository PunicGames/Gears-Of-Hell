using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    // params
    private GameObject player;
    [SerializeField] private GameObject[] tier1_Enemies;
    [SerializeField] private GameObject[] tier2_Enemies;
    [SerializeField] private GameObject[] tier3_Enemies;
    [SerializeField] private List<GameObject> spawnList;
    [SerializeField] private GameObject gameRegistry;

    [SerializeField] private int spawnCount;
    private float minSpawnDelay = 1;
    private float maxSpawnDelay = 4;

    private float tier1_spawnTime;
    private float tier2_spawnTime;
    private float tier3_spawnTime;
    private float timeZero;
    private bool tier1_spawning;
    private bool tier2_spawning;
    private bool tier3_spawning;

    [SerializeField] private int minSpawnSteps;
    [SerializeField] private int maxSpawnSteps;

    //time registry
    private GameRegistry timeScript;

    public void Awake()
    {
        
        spawnList = new List<GameObject>();

        tier1_spawning = false;
        tier2_spawning = false;
        tier3_spawning = false;

        tier1_spawnTime = Time.time + 5f;//primer respawn de enemigo
        tier2_spawnTime = Time.time + 60f;
        tier3_spawnTime = Time.time + 120f;
        timeScript = gameRegistry.GetComponent<GameRegistry>();
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void Update()
    {
        SpawnLoop();
    }

    private void ResetTimer()
    {
        //timeZero = Time.time;

        float aux = timeScript.minutes + 1;

        aux = 10 / aux;

        minSpawnDelay = aux * 0.8f;
        maxSpawnDelay = aux * 1.2f;

        tier1_spawnTime = Time.time + Random.Range(minSpawnDelay, maxSpawnDelay);
        tier1_spawning = false;
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

        Vector2Int index = new Vector2Int(Mathf.RoundToInt(pl_pos.x / WorldGenerator.cellScale.x), Mathf.RoundToInt(pl_pos.z / WorldGenerator.cellScale.y));

        var position = GetSpawnPosition(index);
        var enemy1 = tier1_Enemies[Random.Range(0, tier1_Enemies.Length)];


        print("Spawning enemy at: " + position.ToString());
         
        Instantiate(enemy1, new Vector3(position.x * WorldGenerator.cellScale.x, 0.0f, position.z * WorldGenerator.cellScale.y), Quaternion.identity);

    }

    private void SpawnLoop()
    {
        if (tier1_spawning)
        {
            SpawnEnemy();
            ResetTimer();
        } else
            if (Time.time > tier1_spawnTime)
                tier1_spawning = true;
    }

}
