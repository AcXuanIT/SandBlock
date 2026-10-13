using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SpawnSand : MonoBehaviour
{
    [SerializeField] private GameObject sandBlockPrefab;
    [SerializeField] private Transform sandBlockParent;

    public void SpawnSandBlock(Vector2 position)
    {
        if(Mathf.Abs(position.x) > GridSystem.Instance.gridWidth || Mathf.Abs(position.y) > GridSystem.Instance.gridLength)
        {
            return;
        }
        position.x = position.x + GridSystem.Instance.gridWidth;
        position.y = position.y + GridSystem.Instance.gridLength;

        float c = position.x / GridSystem.Instance.cellSize;
        float r = position.y / GridSystem.Instance.cellSize;

        int col = Mathf.RoundToInt(c);
        int row = Mathf.RoundToInt(r);

        if (GridSystem.Instance.grid[col, row].isEmpty == 0)
        {
            return;
        }

        SandBlock newSandBlock = PoolingManager.Spawn(sandBlockPrefab, position, Quaternion.identity, sandBlockParent).GetComponent<SandBlock>();
        GridSystem.Instance.grid[col, row].isEmpty = 0;
        newSandBlock.SetPosition(col, row);
    }
    public void SpawnSandBlockForItem(int col , int row, Vector2 pos, ColorDetail colorDetail, Transform item)
    {

        SandBlock newSandBlock = PoolingManager.Spawn(sandBlockPrefab, pos, Quaternion.identity, item).GetComponent<SandBlock>();

        newSandBlock.Init(colorDetail.colorType, colorDetail.colorValue, col, row, pos);
    }
}
