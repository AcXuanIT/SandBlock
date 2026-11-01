using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    [SerializeField] public GameObject line;

    public Color lineColor = new Color(1f, 0.15f, 0.15f, 1f);
    public float thickness = 0.08f;
    public int sortingOrderOffset = 2;

    private GridSystem grid;
    private SpriteRenderer sr;
    private Texture2D tex;
    private Sprite sprite;

    private void Awake()
    {
        grid = GridSystem.Instance;
        sr = line.GetComponent<SpriteRenderer>();
    }


    public void InitLine()
    {
        tex = new Texture2D(1, 1, TextureFormat.RGBA32, false) { name = "LimitLineTexture", filterMode = FilterMode.Point };
        tex.SetPixel(0, 0, Color.white);
        tex.Apply(false);
        sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);

        if(sr != null)
        {
            sr.sprite = sprite;
            sr.sortingOrder = grid.sortingOrder + sortingOrderOffset;
        }
        Apply();
    }

    private void LateUpdate()
    {
        Apply();   
    }

    private void Apply()
    {
        line.gameObject.transform.position = new Vector3(0f, grid.LimitY, 0f);
        line.gameObject.transform.localScale = new Vector3(grid.GridWidth * 2f, thickness, 1f);
        sr.color = lineColor;
    }

    private void OnDestroy()
    {
        if (sprite != null) Destroy(sprite);
        if (tex != null) Destroy(tex);
    }
}
