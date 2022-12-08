using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    // params
    private GameObject player;

    public int EnemiesOnScene => GameObject.FindGameObjectsWithTag("Enemy").Length;

    [SerializeField] private GameObject gameRegistry;


    private int tierCounter= 0;
    private Tier tierAtUse;
    public Tier[] tierList;
    [SerializeField] private int[] maxEnemiesAtTier;

    private float spawnTime;
    private bool spawning;

    [Header("Parameters")]
    [SerializeField] private int spawnDistance;
    [SerializeField] private int maxEnemiesAtSameTime;
    [SerializeField] private int minEnemiesAtSameTime;
    //time registry
    private GameRegistry timeScript;
    [SerializeField] private uint nTicks;

    public void Awake()
    {
        spawning = false;
        tierAtUse = tierList[0];

        spawnTime = Time.time + 5f;//primer respawn de enemigo
        //tier2_spawnTime = Time.time + 60f;
        //tier3_spawnTime = Time.time + 120f;
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
        if (spawning)
        {
            var max = MaxEnemiesOnScene();
            print("TIER "+ tierCounter+"\nmax: " + max + "\nnEnemies: " + EnemiesOnScene);
            if (EnemiesOnScene < max)
                SpawnEnemies(max - EnemiesOnScene);
            ResetTimer();
        }
        else
            if (Time.time > spawnTime)
            spawning = true;
    }

    private void ResetTimer()
    {
        //timeZero = Time.time;

        float aux = timeScript.minutes + 1;

        aux = 10 / aux;

        var minSpawnDelay = aux * 0.8f;
        var maxSpawnDelay = aux * 1.2f;

        spawnTime = Time.time + Random.Range(minSpawnDelay, maxSpawnDelay);
        spawning = false;
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
            //var enemy = GetEnemyToInstantiate();
            var enemy = tierAtUse.GetRandomEnemy();

            Instantiate(enemy, new Vector3(pick.x * WorldGenerator.cellScale.x, 0.0f, pick.y * WorldGenerator.cellScale.y), Quaternion.identity);
            
            candidates.Remove(pick);
            n--;
        }
    }

    // Every time we want to increase the Tier, we will add enemies of higher tiers. And as we progress we will remove those enemies that are at very low levels.
    public void TierIncrement()
    {
        var aux = tierCounter + 1;

        // Add new Tier
        if (aux <= tierList.Length - 1)
            tierCounter = aux;

        tierAtUse = tierList[tierCounter];
        maxEnemiesAtSameTime = maxEnemiesAtTier[tierCounter];
    }

    // Return the cell where the player is
    private Vector2Int GetPlayerV2IntPosition()
    {
        var pl_pos = player.transform.position;
        return new Vector2Int(Mathf.RoundToInt(pl_pos.x / WorldGenerator.cellScale.x), Mathf.RoundToInt(pl_pos.z / WorldGenerator.cellScale.y));
    }
    

    // Return the number of max enemies that could be spawned
    private int MaxEnemiesOnScene()
    {
        var x = nTicks * 0.025f;
        //f: y = (((-(x^2)/2)+1 )/( (-(x^2)/2)-1))+1)/2

        //var num = ((-1.0f * x * x) / 2.0f) + 1.0f;
        //var den = ((-1.0f * x * x) / 2.0f) - 1.0f;

        //f: y = ((((-x + 1) / (-x - 1)) + 1) / (2))
        
        var num = -1.0f * x + 1.0f;
        var den = -1.0f * x - 1.0f;

        var res = ((num / den) + 1.0f )/ 2.0f;

        return minEnemiesAtSameTime + Mathf.CeilToInt(res * maxEnemiesAtSameTime);
    }

    public static int ManhathanDistance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

}
