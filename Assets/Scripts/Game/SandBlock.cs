using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandBlock : MonoBehaviour
{
    [SerializeField] public SpriteRenderer spriteRenderer;

    public int col;

    public int row; 

    public ColorType sandColorType;

    public void Init(ColorType colorType , Color color , int col , int row , Vector2 pos)
    {
        this.col = 0;
        this.row = 0;

        this.sandColorType = colorType;
        spriteRenderer.color = color;

        InitScale();   
        InitPosition(col, row, pos);
    }

    public void InitScale()
    {
        float scale = GridSystem.Instance.cellSize;
        transform.localScale = new Vector3(scale, scale, 1f);
    }
    public void InitPosition(int col , int row, Vector2 pos)
    {
        Vector2 newPos = new Vector2((col+1) * GridSystem.Instance.cellSize - GridSystem.Instance.cellSize * 0.5f, (row+1) * GridSystem.Instance.cellSize - GridSystem.Instance.cellSize * 0.5f);
        newPos.x = newPos.x + pos.x;
        newPos.y = newPos.y + pos.y;
        transform.position = newPos;
    }

    public void SetPosition(int col, int row)
    {
        this.col = col;
        this.row = row;

        Vector2 newPos = new Vector2(col * GridSystem.Instance.cellSize - GridSystem.Instance.cellSize * 0.5f, row * GridSystem.Instance.cellSize - GridSystem.Instance.cellSize * 0.5f);
        newPos.x = newPos.x - GridSystem.Instance.gridWidth;
        newPos.y = newPos.y - GridSystem.Instance.gridLength;
        transform.position = newPos;
    }
    private void Update()
    {
        //FallSand();
    }
    public void FallSand()
    {
        if(row > 0 && GridSystem.Instance.grid[col,row - 1].isEmpty == 1)
        {
            GridSystem.Instance.grid[col, row].isEmpty = 1;
            row--;
            GridSystem.Instance.grid[col, row].isEmpty = 0;
            SetPosition(col, row);
        }
        else if(row > 0 && col > 0 && GridSystem.Instance.grid[col - 1, row - 1].isEmpty == 1)
        {
            GridSystem.Instance.grid[col, row].isEmpty = 1;
            row--;
            col--;
            GridSystem.Instance.grid[col, row].isEmpty = 0;
            SetPosition(col, row);
        }
        else if(row > 0 && col < GridSystem.Instance.grid.GetLength(1) - 1 && GridSystem.Instance.grid[col + 1, row - 1].isEmpty == 1)
        {
            GridSystem.Instance.grid[col, row].isEmpty = 1;
            row--;
            col++;
            GridSystem.Instance.grid[col, row].isEmpty = 0;
            SetPosition(col, row);
        }
    }
}
