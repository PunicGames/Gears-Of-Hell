using System.Collections.Generic;
using UnityEngine;


public class WG_Manager : MonoBehaviour
{
    #region Generation Algorithm
    
    //  Algorithm params
    [SerializeField] private uint lifeTime = 7;


    // 0 = Right, 1 = Up, 2 = Left, 3 = Down
    private readonly Vector2Int[] moves = { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };

    private Dictionary<Vector2Int, bool[]> GenerateBlueprint()
    {
        var pos = Vector2Int.zero;

        // 0 = Right, 1 = Up, 2 = Left, 3 = Down
        Dictionary<Vector2Int, bool[]> cells = new Dictionary<Vector2Int, bool[]>();

        cells.TryAdd(pos, new bool[] { false, false, false, false });

        // while instead of for because of probably want to increase or decrease lifeTime
        while (lifeTime > 0)
        {
            int moveIndex = Random.Range(0, moves.Length);

            // exit door
            cells[pos][moveIndex] = true;

            pos += moves[moveIndex];

            bool[] arr;
            if (!cells.TryGetValue(pos, out arr))
            {
                arr = new bool[] { false, false, false, false };   
            }
            arr[InvertMovement(moveIndex)] = true;
            cells.TryAdd(pos, arr);

            lifeTime--;
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
    #endregion

    #region MonoBehavior

    public void Start()
    {
        Dictionary<Vector2Int, bool[]> cells = GenerateBlueprint();

        //debug
        foreach (var cell in cells)
        {
            int doors = 0;
            foreach (var door in cell.Value)
                if (door) doors++;       

            print(cell.Key + " " + doors);
        }
    }


    #endregion
}
