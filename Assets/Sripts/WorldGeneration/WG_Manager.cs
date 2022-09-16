using System.Collections.Generic;
using UnityEngine;


public class WG_Manager : MonoBehaviour
{
    #region Generation Algorithm

    //  Algorithm params
    [SerializeField] private uint lifeTime = 7;
    [SerializeField] private Vector2 cellScale;

    public GameObject[] door_1;
    public GameObject[] door_2_1;
    public GameObject[] door_2_2;
    public GameObject[] door_3;
    public GameObject[] door_4;
    public GameObject[] door_Base;

    // 0 = Right, 1 = Up, 2 = Left, 3 = Down
    private readonly Vector2Int[] moves = { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };

    private Dictionary<Vector2Int, bool[]> GenerateBlueprint()
    {
        var pos = Vector2Int.zero;

        // 0 = Right, 1 = Up, 2 = Left, 3 = Down
        Dictionary<Vector2Int, bool[]> cells = new Dictionary<Vector2Int, bool[]>();

        cells.Add(pos, new bool[] { false, false, false, false });

        // while instead of for because of probably want to increase or decrease lifeTime
        while (lifeTime > 1)
        {
            int moveIndex = Random.Range(0, moves.Length);

            // exit door
            cells[pos][moveIndex] = true;

            pos += moves[moveIndex];

            bool[] arr;
            if (cells.ContainsKey(pos))
            {
                cells[pos][InvertMovement(moveIndex)] = true;
            }
            else
            {
                arr = new bool[] { false, false, false, false };
                arr[InvertMovement(moveIndex)] = true;
                cells.TryAdd(pos, arr);
                lifeTime--;
            }
           
        }

        return cells;
    }

    private int InvertMovement(int move)
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
        // Paralelizable
        foreach (var cell in blueprint)
        {
            int doors = 0;
            foreach (var door in cell.Value)
                if (door) doors++;

            SpawnCell(cell.Key, doors, cell.Value);
        }
    }

    private void SpawnCell(Vector2Int position, int doors, bool[] doorDistribution)
    {
        bool finded = false;
        int it = -1;
        GameObject cell;
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
                        Instantiate(door_2_1[0], new Vector3(position.x * cellScale.x, 0, position.y * cellScale.y), Quaternion.identity);
                    }
                    else if (doorDistribution[2])
                    {
                        Instantiate(door_2_2[0], new Vector3(position.x * cellScale.x, 0, position.y * cellScale.y), Quaternion.identity);
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
                Instantiate(door_4[0], new Vector3(position.x * cellScale.x, 0, position.y * cellScale.y), Quaternion.identity);
                break;
        }
    }
    #endregion

    #region MonoBehavior

    public void Start()
    {
        Dictionary<Vector2Int, bool[]> cells = GenerateBlueprint();
        GenerateCells(cells);
        //debug

        foreach (var cell in cells)
        {
            string dbgStr = "";
            for (int i = 0; i < cell.Value.Length; i++)
            {
                if (cell.Value[i])
                    dbgStr += i.ToString() + "-";
            }

            print(cell.Key + " " + dbgStr);
        }

    }


    #endregion
}
