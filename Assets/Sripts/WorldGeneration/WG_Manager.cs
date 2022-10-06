using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;

public class WG_Gate
{
    public Vector2Int position;
    public int orientation;

    // true = open, false = closed
    public bool state;
    private GameObject gameObject;


    public WG_Gate(Vector2Int position, int orientation, Dictionary<Vector2Int, bool[]> blueprint)
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
        blueprint[position + WG_Manager.moves[orientation]][WG_Manager.InvertMovement(orientation)] = state;
    }
}

public class WG_Manager : MonoBehaviour
{
    #region Generation Algorithm

    #region Variables
    //  Algorithm params
    [SerializeField] private uint nCells = 100;
    [SerializeField] private uint nGates = 5;
    [SerializeField] public static Vector2 cellScale = new Vector2( 10.0f, 10.0f);
    [SerializeField] private Vector2 wallSize;
    [SerializeField] private float obstacleRatio;
    [Space]
    public GameObject[] door_1;
    public GameObject[] door_2_1;
    public GameObject[] door_2_2;
    public GameObject[] door_3;
    public GameObject[] door_4;
    public GameObject[] door_Base;
    public GameObject[] gates;
    public GameObject[] obstacles;

    private NavMeshSurface surface;

    // 0 = Right, 1 = Up, 2 = Left, 3 = Down
    public static readonly Vector2Int[] moves = { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };

    // Needed for delete 
    private LinkedList<GameObject> spawnedCells = new LinkedList<GameObject>();

    // Property that returns copy
    public static Dictionary<Vector2Int, bool[]> GetBlueprint => new Dictionary<Vector2Int, bool[]>(blueprint);

    private static Dictionary<Vector2Int, bool[]> blueprint;

    private List<WG_Gate> gateList = new List<WG_Gate>();

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
        var gateCandidates = new Dictionary<Vector2Int, bool[]>();

        // Paralelizable
        foreach (var cell in blueprint)
        {
            int doors = 0;
            foreach (var door in cell.Value)
                if (door) doors++;

            // Gates
            if (doors == 2)
                gateCandidates.Add(cell.Key, cell.Value);


            SpawnCell(cell.Key, doors, cell.Value);

            if (flipCoin())
                SpawnObstacle(cell.Key);

        }

        GenerateGates(gateCandidates);
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

        switch (doors)
        {
            case 1:
                while (!finded)
                {
                    it++;
                    finded = doorDistribution[it];
                }
                cell = Instantiate(door_1[0], new Vector3(position.x * cellScale.x, 0, position.y * cellScale.y), Quaternion.identity);
                cell.GetComponent<Transform>().Rotate(Vector3.up, -90 * it);
                break;
            case 2:
                if (doorDistribution[0])
                {
                    if (doorDistribution[1])
                    {
                        cell = Instantiate(door_2_1[0], new Vector3(position.x * cellScale.x, 0, position.y * cellScale.y), Quaternion.identity);
                    }
                    else if (doorDistribution[2])
                    {
                        cell = Instantiate(door_2_2[0], new Vector3(position.x * cellScale.x, 0, position.y * cellScale.y), Quaternion.identity);
                    }
                    else
                    {
                        cell = Instantiate(door_2_1[0], new Vector3(position.x * cellScale.x, 0, position.y * cellScale.y), Quaternion.identity);
                        cell.GetComponent<Transform>().Rotate(Vector3.up, 90);
                    }
                }
                else if (doorDistribution[1])
                {
                    if (doorDistribution[2])
                    {
                        cell = Instantiate(door_2_1[0], new Vector3(position.x * cellScale.x, 0, position.y * cellScale.y), Quaternion.identity);
                        cell.GetComponent<Transform>().Rotate(Vector3.up, -90);
                    }
                    else
                    {
                        cell = Instantiate(door_2_2[0], new Vector3(position.x * cellScale.x, 0, position.y * cellScale.y), Quaternion.identity);
                        cell.GetComponent<Transform>().Rotate(Vector3.up, -90);
                    }
                }
                else if (doorDistribution[2])
                {
                    if (doorDistribution[3])
                    {
                        cell = Instantiate(door_2_1[0], new Vector3(position.x * cellScale.x, 0, position.y * cellScale.y), Quaternion.identity);
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
                cell = Instantiate(door_3[0], new Vector3(position.x * cellScale.x, 0, position.y * cellScale.y), Quaternion.identity);
                cell.GetComponent<Transform>().Rotate(Vector3.up, -90 * it);
                break;
            case 4:
                cell = Instantiate(door_4[0], new Vector3(position.x * cellScale.x, 0, position.y * cellScale.y), Quaternion.identity);
                break;
        }

        cell.transform.parent = transform;
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
            var gate = new WG_Gate(pos, holePos[Random.Range(0, holePos.Length)], blueprint);
            var newPos = gate.position + moves[gate.orientation];

            candidates.Remove(pos);
            candidates.Remove(newPos);

            SpawnGate(gate);
            gateList.Add(gate);

            i++;
        }
    }

    private void SpawnGate(WG_Gate gate)
    {
        GameObject obj = Instantiate(gates[0], new Vector3(gate.position.x * cellScale.x, 0, gate.position.y * cellScale.y), Quaternion.identity);
        obj.GetComponent<Transform>().Rotate(Vector3.up, gate.orientation * -90);
        gate.SetGameObject(obj);

        spawnedCells.AddLast(obj);
    }

    private void SpawnObstacle(Vector2Int position)
    {
        Vector2 pos = new Vector2(position.x * cellScale.x, position.y * cellScale.y);
        Vector2 rndOffset = new Vector2(Random.Range((-cellScale.x / 2) + wallSize.x, (cellScale.x / 2) - wallSize.y), Random.Range((-cellScale.y / 2) + wallSize.x, (cellScale.y / 2) - wallSize.y));
        GameObject obs = Instantiate(obstacles[0], new Vector3(pos.x + rndOffset.x, 0.0f, pos.y + rndOffset.y), Quaternion.identity);
        obs.GetComponent<Transform>().Rotate(Vector3.up, Random.Range(0.0f, 359.9f));
        obs.transform.parent = transform;
        spawnedCells.AddLast(obs);

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
        if (Input.GetKeyDown(KeyCode.R))
        {
            DeleteWorld();
            GenerateWorld();
        }
    }

    #endregion
}
