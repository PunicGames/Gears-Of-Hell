using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Gate
{
    public Vector2Int position;
    public int orientation;

    // true = open, false = closed
    public bool state;
    private GameObject gameObject;


    public Gate(Vector2Int position, int orientation, Dictionary<Vector2Int, bool[]> blueprint)
    {
        this.position = position;
        this.orientation = orientation;
        this.state = Random.Range(0, 2) == 0;

        SetStateOnBlueprint(blueprint);
    }

    public void ChangeState(Dictionary<Vector2Int, bool[]> blueprint)
    {
        state = !state;

        SetStateOnBlueprint(blueprint);
    }

    public void SetGameObject(GameObject gameObject)
    {
        this.gameObject = gameObject;
        UpdateGameObject();
    }
    private void UpdateGameObject()
    {
        gameObject.GetComponent<WG_GateManager>().UpdateColor(state);
    }

    private void SetStateOnBlueprint(Dictionary<Vector2Int, bool[]> blueprint)
    {
        blueprint[this.position][orientation] = state;
        blueprint[position + WorldGenerator.moves[orientation]][WorldGenerator.InvertMovement(orientation)] = state;
    }
}

public class WorldGenerator : MonoBehaviour
{
    #region Generation Algorithm

    #region Variables
    // World's Blueprint
    public bool debug = false;

    private static Dictionary<Vector2Int, bool[]> blueprint;

    //  Algorithm params
    [SerializeField] private uint nCells = 100;
    [SerializeField] private uint nGates = 5;
    [SerializeField] public static readonly Vector2 cellScale = new Vector2( 10.0f, 10.0f);
    [SerializeField] private Vector2 wallSize;
    [SerializeField] private float obstacleRatio;

    [Header("Chunks")]
    public GameObject[] floors;
    public GameObject[] door_1;
    public GameObject[] door_2_1;
    public GameObject[] door_2_2;
    public GameObject[] door_3;
    public GameObject[] door_4;
    public GameObject[] door_Base;
    public GameObject[] gates;
    public GameObject[] obstacles;

    [Header("Shops")]
    public GameObject shop;
    public int numberOfShops = 3;
    [Range(3, 5)] public int shopInterSpace = 4;

    // Navmesh
    private NavMeshSurface surface;

    // 0 = Right, 1 = Up, 2 = Left, 3 = Down
    public static readonly Vector2Int[] moves = { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };

    // Needed for delete 
    private LinkedList<GameObject> spawnedCells = new LinkedList<GameObject>();
    [HideInInspector] public List<GameObject> shops = new List<GameObject>();

    // Property that returns copy
    public static Dictionary<Vector2Int, bool[]> GetBlueprint => new Dictionary<Vector2Int, bool[]>(blueprint);

    private List<Gate> gateList = new List<Gate>();

    #endregion

    #region Methods

    private Dictionary<Vector2Int, bool[]> GenerateBlueprint()
    {
        var pos = Vector2Int.zero;

        // 0 = Right, 1 = Up, 2 = Left, 3 = Down
        Dictionary<Vector2Int, bool[]> cells = new Dictionary<Vector2Int, bool[]>();

        cells.Add(pos, new bool[] { false, false, false, false });

        var lifeTime = nCells;
        // while instead of for because of probably want to increase or decrease lifeTime
        while (lifeTime > 1)
        {
            int moveIndex = Random.Range(0, moves.Length);

            // exit door
            cells[pos][moveIndex] = true;

            pos += moves[moveIndex];

            if (cells.ContainsKey(pos))
            {
                cells[pos][InvertMovement(moveIndex)] = true;
            }
            else
            {
                var arr = new bool[] { false, false, false, false };
                arr[InvertMovement(moveIndex)] = true;
                cells.Add(pos, arr);
                lifeTime--;
            }

        }

        return cells;
    }

    public static int InvertMovement(int move)
    {
        switch (move)
        {
            case 0:
                return 2;
            case 1:
                return 3;
            case 2:
                return 0;
            case 3:
                return 1;
            default:
                return -1;
        }
    }

    private void GenerateCells(Dictionary<Vector2Int, bool[]> blueprint)
    {
        // Gate Spawn Management
        //var gateCandidates = new Dictionary<Vector2Int, bool[]>();

        // Paralelizable
        foreach (var cell in blueprint)
        {
            int doors = 0;
            foreach (var door in cell.Value)
                if (door) doors++;
            /*
            // Gates
            if (doors == 2)
                gateCandidates.Add(cell.Key, cell.Value);
            */

            SpawnCell(cell.Key, doors, cell.Value);


        }

        //GenerateGates(gateCandidates);

        SpawnShops();
    }


    private Vector2Int[] GetShopsPositions(int n)
    { 
        var salida = new Vector2Int[n];
        int i = 0;
        int error = 0;
        int space = shopInterSpace;

        while (i < salida.Length)
        {
            var candidates = new List<Vector2Int>(blueprint.Keys);

            for (int j = 0; j < i; j++)
            {
                var b = salida[j];
                candidates.RemoveAll(a=>
                {
                    var dist = EnemySpawnController.ManhathanDistance(a, b);
                    return dist <= space;
                });
            }

            if (candidates.Count != 0) { 
                error++;
                if (error > 25)
                {
                    error = 0;
                    space--;
                }
                salida[i] = candidates[Random.Range(0, candidates.Count)];
                i++;
            }
            else
            {
                i--;
            }
            
        }

        return salida;
    }
    private void SpawnShops()
    {
        var shopPositions = GetShopsPositions(numberOfShops);

        foreach(var p in shopPositions)
        {
            var pos = p * cellScale;
            GameObject obs = Instantiate(shop, new Vector3(pos.x, 0.0f, pos.y), Quaternion.identity);
            obs.transform.parent = transform;
            obs.GetComponent<Shop>().active = false;
            shops.Add(obs);
        }
    }

    private bool flipCoin()
    {
        var n = Random.Range(0.0f, 100.0f);
        return n <= obstacleRatio;
    }

    private void SpawnCell(Vector2Int position, int doors, bool[] doorDistribution)
    {
        // method variables
        bool finded = false;
        int it = -1;
        GameObject cell = null;
        GameObject floor = Instantiate(floors[0], new Vector3(position.x * cellScale.x, 0, position.y * cellScale.y), Quaternion.identity);

        switch (doors)
        {
            case 1:
                while (!finded)
                {
                    it++;
                    finded = doorDistribution[it];
                }
                cell = Instantiate(door_1[Random.Range(0, door_1.Length)], new Vector3(position.x * cellScale.x, 0, position.y * cellScale.y), Quaternion.identity);
                cell.GetComponent<Transform>().Rotate(Vector3.up, -90 * it);
                break;
            case 2:
                if (doorDistribution[0])
                {
                    if (doorDistribution[1])
                    {
                        cell = Instantiate(door_2_1[Random.Range(0, door_2_1.Length)], new Vector3(position.x * cellScale.x, 0, position.y * cellScale.y), Quaternion.identity);
                    }
                    else if (doorDistribution[2])
                    {
                        cell = Instantiate(door_2_2[Random.Range(0, door_2_2.Length)], new Vector3(position.x * cellScale.x, 0, position.y * cellScale.y), Quaternion.identity);
                    }
                    else
                    {
                        cell = Instantiate(door_2_1[Random.Range(0, door_2_1.Length)], new Vector3(position.x * cellScale.x, 0, position.y * cellScale.y), Quaternion.identity);
                        cell.GetComponent<Transform>().Rotate(Vector3.up, 90);
                    }
                }
                else if (doorDistribution[1])
                {
                    if (doorDistribution[2])
                    {
                        cell = Instantiate(door_2_1[Random.Range(0, door_2_1.Length)], new Vector3(position.x * cellScale.x, 0, position.y * cellScale.y), Quaternion.identity);
                        cell.GetComponent<Transform>().Rotate(Vector3.up, -90);
                    }
                    else
                    {
                        cell = Instantiate(door_2_2[Random.Range(0, door_2_2.Length)], new Vector3(position.x * cellScale.x, 0, position.y * cellScale.y), Quaternion.identity);
                        cell.GetComponent<Transform>().Rotate(Vector3.up, -90);
                    }
                }
                else if (doorDistribution[2])
                {
                    if (doorDistribution[3])
                    {
                        cell = Instantiate(door_2_1[Random.Range(0, door_2_1.Length)], new Vector3(position.x * cellScale.x, 0, position.y * cellScale.y), Quaternion.identity);
                        cell.GetComponent<Transform>().Rotate(Vector3.up, 180);
                    }
                }
                break;
            case 3:
                finded = true;
                while (finded)
                {
                    it++;
                    finded = doorDistribution[it];
                }
                cell = Instantiate(door_3[Random.Range(0, door_3.Length)], new Vector3(position.x * cellScale.x, 0, position.y * cellScale.y), Quaternion.identity);
                cell.GetComponent<Transform>().Rotate(Vector3.up, -90 * it);
                break;
            case 4:
                cell = Instantiate(door_4[0], new Vector3(position.x * cellScale.x, 0, position.y * cellScale.y), Quaternion.identity);
                break;
        }
        floor.transform.parent = cell.transform;
        cell.transform.parent = transform;
        cell.name = "Cell: ( " + position.x.ToString() + ", "+ position.y.ToString() + ") -\t" + doors.ToString();

        if (flipCoin())
            SpawnObstacle(position).parent = cell.transform;
        

        spawnedCells.AddLast(cell);
    }

    private void GenerateGates(Dictionary<Vector2Int, bool[]> candidates)
    {
        var i = 0;
        while (candidates.Count > 0 && i < nGates)
        {
            var candidatesArr = new List<Vector2Int>(candidates.Keys);

            var n = Random.Range(0, candidatesArr.Count);


            var pos = candidatesArr[n];
            var holes = candidates[candidatesArr[n]];

            var holePos = new int[2];
            var it = 0;
            for (int j = 0; j < holes.Length; j++)
            {
                if (holes[j])
                {
                    holePos[it] = j;
                    it++;
                }
            }
            var gate = new Gate(pos, holePos[Random.Range(0, holePos.Length)], blueprint);
            var newPos = gate.position + moves[gate.orientation];

            candidates.Remove(pos);
            candidates.Remove(newPos);

            SpawnGate(gate);
            gateList.Add(gate);

            i++;
        }
    }

    private void SpawnGate(Gate gate)
    {
        GameObject obj = Instantiate(gates[0], new Vector3(gate.position.x * cellScale.x, 0, gate.position.y * cellScale.y), Quaternion.identity);
        obj.GetComponent<Transform>().Rotate(Vector3.up, gate.orientation * -90);
        gate.SetGameObject(obj);

        spawnedCells.AddLast(obj);
    }

    private Transform SpawnObstacle(Vector2Int position)
    {
        Vector2 pos = new Vector2(position.x * cellScale.x, position.y * cellScale.y);
        Vector2 rndOffset = new Vector2(Random.Range((-cellScale.x / 2) + wallSize.x, (cellScale.x / 2) - wallSize.y), Random.Range((-cellScale.y / 2) + wallSize.x, (cellScale.y / 2) - wallSize.y));
        GameObject obs = Instantiate(obstacles[Random.Range(0, obstacles.Length)], new Vector3(pos.x + rndOffset.x, 0.0f, pos.y + rndOffset.y), Quaternion.identity);
        obs.transform.Rotate(Vector3.up, Random.Range(0.0f, 359.9f));
        return obs.transform;

    }

    private void GenerateWorld()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        blueprint = GenerateBlueprint();
        GenerateCells(blueprint);

        sw.Stop();
        Debug_UI.Print("Mapa generado en: " + sw.Elapsed.ToString());

        surface.BuildNavMesh();

        /* Debug
        foreach (var cell in blueprint)
        {
            string dbgStr = "";
            for (int i = 0; i < cell.Value.Length; i++)
            {
                if (cell.Value[i])
                    dbgStr += i.ToString() + "-";
            }

            print(cell.Key + " " + dbgStr);
        }
        */

    }

    private void DeleteWorld()
    {
        foreach (var cell in spawnedCells)
        {
            Destroy(cell);
        }
        foreach (var shop in shops)
        {
            Destroy(shop);
        }
    }
    #endregion

    #endregion

    #region MonoBehavior

    public void Awake()
    {
        surface = gameObject.GetComponent<NavMeshSurface>();

        GenerateWorld();
    }

    
    public void Update()
    {
        if (debug && Input.GetKeyDown(KeyCode.R))
        {
            DeleteWorld();
            GenerateWorld();
        }
    }
    
    #endregion
}
