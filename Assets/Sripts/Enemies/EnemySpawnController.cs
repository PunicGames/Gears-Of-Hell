using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    // params
    private GameObject player;

    public int EnemiesOnScene => GameObject.FindGameObjectsWithTag("Enemy").Length;

    [SerializeField] private GameObject[] tier1_Enemies;
    [SerializeField] private GameObject[] tier2_Enemies;
    [SerializeField] private GameObject[] tier3_Enemies;
    [SerializeField] private GameObject[] tier4_Enemies;
    [SerializeField] private GameObject[] tier5_Enemies;
    [SerializeField] private GameObject gameRegistry;

    private float tier1_spawnTime;
    private float tier2_spawnTime;
    private float tier3_spawnTime;
    private float timeZero;
    private bool tier1_spawning;
    private bool tier2_spawning;
    private bool tier3_spawning;

    [SerializeField] private int spawnDistance;
    [SerializeField] private int maxEnemiesAtSameTime;
    [SerializeField] private int minEnemiesAtSameTime;
    //time registry
    private GameRegistry timeScript;
    [SerializeField] private uint nTicks;

    public void Awake()
    {
        tier1_spawning = false;
        tier2_spawning = false;
        tier3_spawning = false;

        tier1_spawnTime = Time.time + 5f;//primer respawn de enemigo
        tier2_spawnTime = Time.time + 60f;
        tier3_spawnTime = Time.time + 120f;
        timeScript = gameRegistry.GetComponent<GameRegistry>();
        nTicks = 0;
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void Update()
    {
        SpawnLoop();
    }

    private void SpawnLoop()
    {
        if (tier1_spawning)
        {
            var max = MaxEnemiesOnScene();
            print("max: " + max + "\nnEnemies: " + EnemiesOnScene);
            if (EnemiesOnScene < max)
                SpawnEnemies(max - EnemiesOnScene);
            ResetTimer();
        }
        else
            if (Time.time > tier1_spawnTime)
            tier1_spawning = true;
    }

    private void ResetTimer()
    {
        //timeZero = Time.time;

        float aux = timeScript.minutes + 1;

        aux = 10 / aux;

        var minSpawnDelay = aux * 0.8f;
        var maxSpawnDelay = aux * 1.2f;

        tier1_spawnTime = Time.time + Random.Range(minSpawnDelay, maxSpawnDelay);
        tier1_spawning = false;
        nTicks++;
    }


    private List<Vector2Int> GetPossiblesSpawnPositions(Vector2Int key)
    {
        var keyList = new List<Vector2Int>(WorldGenerator.GetBlueprint.Keys);

        keyList.RemoveAll(x => {
            var dist = ManhathanDistance(key, x);
            //return (dist <= spawDistance - 2) || (dist >= spawDistance + 1);
            return dist != spawnDistance;
        });

        return keyList;
    }

    /* DEPRECATED SPAWN POSITION SOLVER
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
    */


    private void SpawnEnemies(int n)
    {
        var index = GetPlayerV2IntPosition();
        var candidates = GetPossiblesSpawnPositions(index);
        while ((candidates.Count > 0) && (n > 0))
        {
            var pick = candidates[Random.Range(0, candidates.Count)];

            //var enemy = tier1_Enemies[Random.Range(0, tier1_Enemies.Length)];
            var enemy = GetEnemyToInstantiate();

            Instantiate(enemy, new Vector3(pick.x * WorldGenerator.cellScale.x, 0.0f, pick.y * WorldGenerator.cellScale.y), Quaternion.identity);
            
            candidates.Remove(pick);
            n--;
        }
    }

    private Vector2Int GetPlayerV2IntPosition()
    {
        var pl_pos = player.transform.position;
        return new Vector2Int(Mathf.RoundToInt(pl_pos.x / WorldGenerator.cellScale.x), Mathf.RoundToInt(pl_pos.z / WorldGenerator.cellScale.y));
    }

    private int MaxEnemiesOnScene()
    {
        //y = (((-(x^2)/2)+1 )/( (-(x^2)/2)-1))+1)/2

        var x = nTicks * 0.025f;
        var num = ((-1.0f * x * x) / 2.0f) + 1.0f;
        var den = ((-1.0f * x * x) / 2.0f) - 1.0f;
        var res = ((num / den) + 1.0f )/ 2.0f;

        return minEnemiesAtSameTime + Mathf.CeilToInt(res * maxEnemiesAtSameTime);
    }

    public static int ManhathanDistance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private GameObject GetEnemyToInstantiate()
    {
        var n = Random.Range(0.0f, 1.0f);

        if (n < 0.6f)
        {
            return tier1_Enemies[0];
        } else if (n >= 0.6f && n < 0.8f)
        {
            return tier1_Enemies[1];
        } else
        {
            return tier1_Enemies[2];
        }
    }

}
