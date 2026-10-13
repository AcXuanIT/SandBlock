using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell
{
    public byte isEmpty;
}

public class GridSystem : Singleton<GridSystem>
{
    [Header("Cell Settings")]
    public float cellSize = 0.1f;


    [Header("Grid Settings")]
    public GridCell[,] grid;

    [Header("Functional Settings")]
    public SpawnSand spawnSand;
    public InputSystem inputSystem;

    public float gridWidth;
    public float gridLength;

    private void Awake()
    {
        InitializeGrid(150,200);
    }

    public void InitializeGrid(int col , int row)
    {
        grid = new GridCell[col, row];

        for (int x = 0; x < col; x++)
        {
            for (int y = 0; y < row; y++)
            {
                grid[x, y] = new GridCell();
                grid[x, y].isEmpty = 1; 
            }
        }

        gridWidth = (col / 2) * cellSize;
        gridLength = (row / 2) * cellSize;
    }

}
