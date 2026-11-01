using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private BoxCollider2D boxCollider;
    public float initScale = 0.5f;

    private SpriteRenderer visual;
    private Texture2D tex;
    private Sprite sprite;
    private Color32[] pixels;
    private int texW, texH;
    private int minX, minY;

    private ItemDetail itemCurrentData;
    private ColorDetail colorDetail;
    private int size;
    private Vector3 home;

    public Vector2 LocalCenter => boxCollider.offset;
    public Vector2 LocalHalfSize => boxCollider.size * 0.5f;

    public void SetupBlock(ItemDetail data, ColorDetail colorDetail, int size, Vector2 spawnCenter)
    {
        itemCurrentData = data;
        this.colorDetail = colorDetail;
        this.size = size;

        GridSystem grid = GridSystem.Instance;
        float cell = grid.cellSize;
        List<Vector2Int> shape = data.shape;

        // khung bao cua hinh (don vi: o hinh)
        int maxX, maxY;
        minX = int.MaxValue; minY = int.MaxValue;
        maxX = int.MinValue; maxY = int.MinValue;
        for (int i = 0; i < shape.Count; i++)
        {
            Vector2Int b = shape[i];
            if (b.x < minX) minX = b.x;
            if (b.y < minY) minY = b.y;
            if (b.x > maxX) maxX = b.x;
            if (b.y > maxY) maxY = b.y;
        }

        int w = (maxX - minX + 1) * size;
        int h = (maxY - minY + 1) * size;
        EnsureTexture(w, h, cell);

        // to mau tung hat (o khong thuoc hinh de trong suot)
        System.Array.Clear(pixels, 0, pixels.Length);
        for (int s = 0; s < shape.Count; s++)
        {
            int baseX = (shape[s].x - minX) * size;
            int baseY = (shape[s].y - minY) * size;
            for (int gy = 0; gy < size; gy++)
                for (int gx = 0; gx < size; gx++)
                    pixels[(baseY + gy) * w + baseX + gx] = grid.JitterColor(colorDetail.colorType);
        }
        tex.SetPixels32(pixels);
        tex.Apply(false);

        // goc duoi trai cua anh nam o (minX, minY) trong khong gian hinh
        visual.transform.localPosition = new Vector3(minX * size * cell, minY * size * cell, 0f);

        SetCollider(size * cell, maxX, maxY);

        // dat tam collider vao spawnCenter
        home = new Vector3(spawnCenter.x - boxCollider.offset.x * initScale,
                           spawnCenter.y - boxCollider.offset.y * initScale, 0f);
        transform.position = home;
        transform.localScale = Vector3.one * initScale;
    }

    private void EnsureTexture(int w, int h, float cell)
    {
        if (visual == null)
        {
            var go = new GameObject("Visual");
            go.transform.SetParent(transform, false);
            visual = go.AddComponent<SpriteRenderer>();
            visual.sortingOrder = 10;
        }

        if (tex != null && texW == w && texH == h) return;

        if (sprite != null) Destroy(sprite);
        if (tex != null) Destroy(tex);

        tex = new Texture2D(w, h, TextureFormat.RGBA32, false)
        {
            name = "ItemTexture",
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        sprite = Sprite.Create(tex, new Rect(0, 0, w, h), Vector2.zero, 1f / cell, 0, SpriteMeshType.FullRect);
        visual.sprite = sprite;

        pixels = new Color32[w * h];
        texW = w;
        texH = h;
    }

    private void SetCollider(float blockSize, int maxX, int maxY)
    {
        boxCollider.size = new Vector2((maxX - minX + 1) * blockSize, (maxY - minY + 1) * blockSize);
        boxCollider.offset = new Vector2((minX + maxX + 1) * blockSize * 0.5f,
                                         (minY + maxY + 1) * blockSize * 0.5f);
    }

    public void BeginDrag()
    {
        transform.localScale = Vector3.one;
    }

    public bool TryPlace()
    {
        GridSystem grid = GridSystem.Instance;

        List<Vector2Int> shape = itemCurrentData.shape;

        grid.WorldToCell(transform.position, out int col, out int row);

        for (int i = 0; i < shape.Count; i++)
        {
            if (!grid.CheckGrid(col + shape[i].x * size, row + shape[i].y * size, size, size))
            {
                transform.position = home;
                transform.localScale = Vector3.one * initScale;
                return false;
            }
        }
        int t = GameManager.Instance.scorePerBlock * shape.Count;
        GameManager.Instance.scoreUI.OnScore(t, transform.position);
        GameManager.Instance.SetScore(t);

        for (int s = 0; s < shape.Count; s++)
        {
            int baseX = (shape[s].x - minX) * size;
            int baseY = (shape[s].y - minY) * size;
            for (int gy = 0; gy < size; gy++)
            {
                for (int gx = 0; gx < size; gx++)
                {
                    Color32 c = pixels[(baseY + gy) * texW + baseX + gx];
                    grid.Place(col + shape[s].x * size + gx, row + shape[s].y * size + gy, colorDetail.colorType, c);
                }
            }
        }

        return true;
    }

    private void OnDestroy()
    {
        if (sprite != null) Destroy(sprite);
        if (tex != null) Destroy(tex);
    }
}
