using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private BoxCollider2D boxCollider;
    public float detailScale = 1f;
    public float initScale = 0.5f;
    public void SetupBlock(ItemDetail data, ColorDetail colorDetail ,int size, Vector2 posItemSpawn)
    {
        List<Vector2Int> shape = data.shape;

        for(int i = 0; i < shape.Count; i++)
        {
            Vector2Int pos = shape[i];
            Vector2 spawnPos = new Vector2(pos.x*size*GridSystem.Instance.cellSize, pos.y*size*GridSystem.Instance.cellSize);
            spawnPos += posItemSpawn;

            SpawnBlockItem(colorDetail,size, spawnPos);
        }

        SetCollider(shape, size);

        transform.localScale = Vector3.one * initScale;
    }

    public void SpawnBlockItem(ColorDetail colorDetail, int size, Vector2 pos)
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                GridSystem.Instance.spawnSand.SpawnSandBlockForItem(x, y, pos, colorDetail, transform);
            }
        }
    }
    public void SetCollider(List<Vector2Int> shape, int size)
    {
        if (boxCollider == null)
            return;

        float cellSize = GridSystem.Instance.cellSize;

        int minX = int.MaxValue;
        int minY = int.MaxValue;
        int maxX = int.MinValue;
        int maxY = int.MinValue;

        foreach (Vector2Int block in shape)
        {
            minX = Mathf.Min(minX, block.x);
            minY = Mathf.Min(minY, block.y);
            maxX = Mathf.Max(maxX, block.x);
            maxY = Mathf.Max(maxY, block.y);
        }

        float blockSize = size * cellSize;

        boxCollider.size = new Vector2(
            (maxX - minX + 1) * blockSize,
            (maxY - minY + 1) * blockSize
        );

        boxCollider.offset = new Vector2(
            (minX + maxX + 1) * blockSize * 0.5f,
            (minY + maxY + 1) * blockSize * 0.5f
        );
    }
}
