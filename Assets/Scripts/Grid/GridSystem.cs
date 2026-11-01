using DG.Tweening;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GridSystem : Singleton<GridSystem>
{
    [Header("BackGround")]
    [SerializeField] public GameObject backgroundGrid;

    [Header("Grid")]
    public int Col = 150;
    public int Row = 200;
    public float cellSize = 0.1f;

    [Header("Rendering")]
    public ColorData colorData;
    [Range(0f, 0.3f)] public float colorJitter = 0.08f;
    public int sortingOrder = 0;

    [Header("Simulation")]
    [Range(10, 120)] public int ticksPerSecond = 60;
    [Range(1, 4)] public int maxTicksPerFrame = 2;
    [Range(1, 8)] public int maxFallPerTick = 2;

    [Header("Clear Effect")]
    public float flashDuration = 1f;
    [Range(1f, 30f)] public float flashStepsPerSecond = 9f;
    public Color[] flashTints = { new Color(0.90f, 0.90f, 0.90f), new Color(0.58f, 0.58f, 0.58f), new Color(0.26f, 0.26f, 0.26f) };

    [Header("Booster")]
    public float recolorBlinkHz = 2.5f;

    [Header("Game Over")]
    [Range(0.5f, 1f)] public float limitRatio = 0.85f;

    public float GridWidth { get; private set; }
    public float GridLength { get; private set; }
    public float UploadMs { get; private set; }
    public bool IsSimulating => active;
    public int FlashingGroups => flashGroups.Count;
    public bool IsGameOver { get; private set; }
    public int HighestRow { get; private set; } = -1;
    public int LimitRow => Mathf.Clamp(Mathf.RoundToInt(Row * limitRatio), 1, Row - 1);
    public float LimitY => LimitRow * cellSize - GridLength;

    private class FlashGroup
    {
        public int[] cells;
        public float start;
        public int lastPhase = -1;
        public float[] jitter;
    }

    private byte[] cells;
    private Color32[] pixels;
    private Color32[] palette;

    private bool[] flashing;      
    private readonly List<FlashGroup> flashGroups = new List<FlashGroup>();


    public class RecolorGroup
    {
        public int[] cells;
        public Color32[] original;
        public float[] jitter;
        public ColorType target;
        public int lastPhase = -1;
    }
    public RecolorGroup recolor;
    private Texture2D tex;
    private bool texDirty;

    private int[] visitStamp;
    private int[] queue;
    private int stamp;

    private bool active;
    private float accumulator;
    private int tick;

    private void OnDestroy()
    {
        if (tex != null)
        {
            Destroy(tex);
            tex = null;
        }
    }

    private void Update()
    {
        if (IsGameOver)
        {
            UpdateFlashes();     
            return;
        }
        UpdateSim();
    }
    private void UpdateSim()
    {
        UpdateFlashes();      

        if (!active)
        {
            return;
        }

        accumulator += Time.deltaTime;
        float step = 1f / ticksPerSecond;
        int ticks = 0;

        while (accumulator >= step && ticks < maxTicksPerFrame)
        {
            accumulator -= step;
            ticks++;

            if (!Step())
            {
                active = false;
                accumulator = 0f;
                ResolveMatches();
                CheckGameOver();
                UpdateHighestRow();
                break;
            }
        }

        if (accumulator > step * maxTicksPerFrame) accumulator = 0f;
    }

    private void LateUpdate()
    {
        if (!texDirty) return;

        tex.SetPixels32(pixels);
        tex.Apply(false);
        texDirty = false;
    }

    public void InitializeGrid()
    {
        int col = this.Col;
        int row = this.Row;
        int n = col * row;

        cells = new byte[n];
        pixels = new Color32[n];
        flashing = new bool[n];
        visitStamp = new int[n];
        queue = new int[n];

        GridWidth = col * 0.5f * cellSize;
        GridLength = row * 0.5f * cellSize;

        palette = new Color32[16];
        foreach (ColorDetail d in colorData.All)
            palette[(int)d.colorType] = d.colorValue;

        tex = new Texture2D(col, row, TextureFormat.RGBA32, false)
        {
            name = "SandTexture",
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        tex.SetPixels32(pixels);
        tex.Apply(false);

        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, col, row), Vector2.zero, 1f / cellSize, 0, SpriteMeshType.FullRect);

        GameObject go = new GameObject("SandTexture");
        go.transform.SetParent(transform, false);
        go.transform.position = new Vector3(-GridWidth, -GridLength, 0f);

        backgroundGrid.transform.localScale = new Vector3(Col * cellSize, Row * cellSize, 1f);

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = sortingOrder;
    }

    private void UpdateHighestRow()
    {
        for (int i = cells.Length - 1; i >= 0; i--)
        {
            if (cells[i] != 0 && !flashing[i])
            {
                HighestRow = i / Col;
                return;
            }
        }
        HighestRow = -1;
    }
    private void CheckGameOver()
    {
        if (IsGameOver) return;

        for (int i = LimitRow * Col; i < cells.Length; i++)
        {
            if (cells[i] != 0 && !flashing[i])
            {
                GameManager.Instance.OnKeepPlaying();
                return;
            }
        }
    }
    public void Restart()
    {
        StartGameOverWipe();

        DOVirtual.DelayedCall(1f, () =>
        {
            ClearAll();
            IsGameOver = false;
            accumulator = 0f;
            HighestRow = -1;
        });
    }

    public void StartGameOverWipe()
    {
        IsGameOver = true;
        active = false;
        CancelRecolor();     

        int n = 0;
        for (int i = 0; i < cells.Length; i++)
            if (cells[i] != 0 && !flashing[i]) queue[n++] = i;
        if (n > 0) StartFlash(n);
    }

    public void WorldToCell(Vector2 world, out int col, out int row)
    {
        col = Mathf.RoundToInt((world.x + GridWidth) / cellSize);
        row = Mathf.RoundToInt((world.y + GridLength) / cellSize);
    }
    public void WorldToCellFloor(Vector2 world, out int col, out int row)
    {
        col = Mathf.FloorToInt((world.x + GridWidth) / cellSize);
        row = Mathf.FloorToInt((world.y + GridLength) / cellSize);
    }
    public bool CheckGrid(int col, int row, int sizeX, int sizeY)
    {
        if (col < 0 || row < 0 || col + sizeX > Col || row + sizeY > Row) return false;

        for (int y = row; y < row + sizeY; y++)
        {
            int rs = y * Col;
            for (int x = col; x < col + sizeX; x++)
                if (cells[rs + x] != 0) return false;
        }
        return true;
    }

    public Color32 JitterColor(ColorType type)
    {
        Color32 b = palette[(int)type];
        float j = 1f - Random.value * colorJitter;
        return new Color32((byte)(b.r * j), (byte)(b.g * j), (byte)(b.b * j), 255);
    }

    public bool Place(int col, int row, ColorType type, Color32 color)
    {
        int ci = row * Col + col;
        if (cells[ci] != 0) return false;

        cells[ci] = (byte)((int)type + 1);

        pixels[ci] = color;

        texDirty = true;
        active = true;
        return true;
    }

    private bool Step()
    {
        bool moved = false;
        bool leftFirst = (++tick & 1) == 0;
        int dir = leftFirst ? -1 : 1;
        int c = Col;
        int maxFall = maxFallPerTick;

        for (int y = 1; y < Row; y++)
        {
            int rs = y * c;
            for (int k = 0; k < c; k++)
            {
                int x = leftFirst ? k : c - 1 - k;
                int i = rs + x;
                byte v = cells[i];
                if (v == 0) continue;

                int below = i - c;

                if (cells[below] == 0)
                {
                    int ty = y - 1;
                    int fallen = 1;
                    while (fallen < maxFall && ty > 0 && cells[(ty - 1) * c + x] == 0)
                    {
                        ty--;
                        fallen++;
                    }

                    MoveCell(i, ty * c + x, v);
                    moved = true;
                    continue;
                }

                int tx = -1;
                int xa = x + dir;
                if (xa >= 0 && xa < c && cells[below + dir] == 0)
                {
                    tx = xa;
                }
                else
                {
                    int xb = x - dir;
                    if (xb >= 0 && xb < c && cells[below - dir] == 0)
                        tx = xb;
                }

                if (tx >= 0)
                {
                    MoveCell(i, below - x + tx, v);
                    moved = true;
                }
            }
        }

        if (moved) texDirty = true;
        return moved;
    }

    private void MoveCell(int from, int to, byte v)
    {
        cells[to] = v;
        cells[from] = 0;
        pixels[to] = pixels[from];
        pixels[from] = default;
    }

    private void ResolveMatches()
    {
        stamp++;

        for (int y = 0; y < Row; y++)
        {
            int start = y * Col;
            byte color = cells[start];
            if (color == 0 || visitStamp[start] == stamp) continue;

            int head = 0, tail = 0;
            bool reachedRight = false;

            queue[tail++] = start;
            visitStamp[start] = stamp;

            while (head < tail)
            {
                int cur = queue[head++];
                int cx = cur % Col;
                int cy = cur / Col;
                if (cx == Col - 1) reachedRight = true;

                for (int dy = -1; dy <= 1; dy++)
                {
                    int ny = cy + dy;
                    if (ny < 0 || ny >= Row) continue;

                    for (int dx = -1; dx <= 1; dx++)
                    {
                        if (dx == 0 && dy == 0) continue;
                        int nx = cx + dx;
                        if (nx < 0 || nx >= Col) continue;

                        int ni = ny * Col + nx;
                        if (cells[ni] != color || visitStamp[ni] == stamp) continue;

                        visitStamp[ni] = stamp;
                        queue[tail++] = ni;
                    }
                }
            }
            if (reachedRight)
                StartFlash(tail);
        }
    }
    private void StartFlash(int count)
    {
        var group = new FlashGroup { cells = new int[count], start = Time.time };
        System.Array.Copy(queue, group.cells, count);
        group.jitter = new float[count];
        for (int k = 0; k < count; k++)
        {
            int idx = group.cells[k];
            flashing[idx] = true;

            
            Color32 p = pixels[idx];
            Color32 b = palette[cells[idx] - 1];
            int bm = Mathf.Max(b.r, Mathf.Max(b.g, b.b));
            int pm = Mathf.Max(p.r, Mathf.Max(p.g, p.b));
            group.jitter[k] = bm > 0 ? Mathf.Clamp01(pm / (float)bm) : 1f;
        }
        flashGroups.Add(group);
    }

    private void UpdateFlashes()
    {
        UpdateRecolorPreview();
        if (flashGroups.Count == 0) return;

        float now = Time.time;
        int cleared = 0;
        int colorCount = flashTints.Length;

        for (int g = flashGroups.Count - 1; g >= 0; g--)
        {
            FlashGroup fg = flashGroups[g];
            float elapsed = now - fg.start;

            if (elapsed >= flashDuration)
            {
                int[] list = fg.cells;

                if (!IsGameOver)
                {
                    int s = list.Length;
                    GameManager.Instance.scoreUI.OnScore(s, transform.position);
                    GameManager.Instance.SetScore(s);
                }

                for (int k = 0; k < list.Length; k++)
                {
                    int idx = list[k];
                    cells[idx] = 0;
                    pixels[idx] = default;
                    flashing[idx] = false;
                }
                cleared += list.Length;
                flashGroups.RemoveAt(g);
                continue;
            }

            if (colorCount == 0) continue;

            int phase = (int)(elapsed * flashStepsPerSecond) % colorCount;
            if (phase != fg.lastPhase)
            {
                fg.lastPhase = phase;
                Color32 t = flashTints[phase];
                int[] list = fg.cells;
                for (int k = 0; k < list.Length; k++)
                {
                    float j = fg.jitter[k];
                    pixels[list[k]] = new Color32((byte)(t.r * j), (byte)(t.g * j), (byte)(t.b * j), 255);
                }
                texDirty = true;
            }
        }

        if (cleared > 0)
        {
            texDirty = true;
            active = true;         
        }
    }

    public bool HasRecolorPreview => recolor != null;

    public byte SelectableValue(int col, int row)
    {
        if (col < 0 || col >= Col || row < 0 || row >= Row) return 0;
        int i = row * Col + col;
        return flashing[i] ? (byte)0 : cells[i];
    }

    public bool IsInRecolorRegion(int col, int row)
    {
        if (recolor == null) return false;
        if (col < 0 || col >= Col || row < 0 || row >= Row) return false;

        int target = row * Col + col;
        int[] list = recolor.cells;
        for (int k = 0; k < list.Length; k++)
            if (list[k] == target) return true;
        return false;
    }

    public int CollectCircle(int cx, int cy, int radius, int[] buffer)
    {
        int n = 0;
        int r2 = radius * radius;
        for (int dy = -radius; dy <= radius; dy++)
        {
            int y = cy + dy;
            if (y < 0 || y >= Row) continue;
            for (int dx = -radius; dx <= radius; dx++)
            {
                if (dx * dx + dy * dy > r2) continue;
                int x = cx + dx;
                if (x < 0 || x >= Col) continue;
                int i = y * Col + x;
                if (cells[i] != 0 && !flashing[i]) buffer[n++] = i;
            }
        }
        return n;
    }

    public int CollectColor(byte id, int[] buffer)
    {
        int n = 0;
        for (int i = 0; i < cells.Length; i++)
            if (cells[i] == id && !flashing[i]) buffer[n++] = i;
        return n;
    }

    public int CollectRegion(int col, int row, int[] buffer)
    {
        if (SelectableValue(col, row) == 0) return 0;

        stamp++;
        int start = row * Col + col;
        byte color = cells[start];
        int head = 0, tail = 0;
        queue[tail++] = start;
        visitStamp[start] = stamp;

        while (head < tail)
        {
            int cur = queue[head++];
            int cx = cur % Col;
            int cy = cur / Col;

            for (int dy = -1; dy <= 1; dy++)
            {
                int ny = cy + dy;
                if (ny < 0 || ny >= Row) continue;
                for (int dx = -1; dx <= 1; dx++)
                {
                    if (dx == 0 && dy == 0) continue;
                    int nx = cx + dx;
                    if (nx < 0 || nx >= Col) continue;

                    int ni = ny * Col + nx;
                    if (cells[ni] != color || flashing[ni] || visitStamp[ni] == stamp) continue;

                    visitStamp[ni] = stamp;
                    queue[tail++] = ni;
                }
            }
        }

        System.Array.Copy(queue, buffer, tail);
        return tail;
    }

    public int StartClearFlash(int[] list, int count)
    {
        int n = 0;
        for (int k = 0; k < count; k++)
        {
            int idx = list[k];
            if (cells[idx] == 0 || flashing[idx]) continue;
            queue[n++] = idx;
        }
        if (n == 0) return 0;
        StartFlash(n);
        return n;
    }

    public bool BeginRecolorPreview(int[] list, int count, ColorType target)
    {
        CancelRecolor();

        if (count <= 0) return false;

        var g = new RecolorGroup
        {
            cells = new int[count],
            original = new Color32[count],
            jitter = new float[count],
            target = target
        };

        for (int k = 0; k < count; k++)
        {
            int idx = list[k];
            g.cells[k] = idx;
            Color32 p = pixels[idx];
            g.original[k] = p;

            Color32 b = palette[cells[idx] - 1];
            int bm = Mathf.Max(b.r, Mathf.Max(b.g, b.b));
            int pm = Mathf.Max(p.r, Mathf.Max(p.g, p.b));
            g.jitter[k] = bm > 0 ? Mathf.Clamp01(pm / (float)bm) : 1f;

            flashing[idx] = true;     
        }

        recolor = g;
        return true;
    }

    public void SetRecolorTarget(ColorType t)
    {
        if (recolor == null) return;
        recolor.target = t;
        recolor.lastPhase = -1;
    }

    public void CancelRecolor()
    {
        if (recolor == null) return;
        for (int k = 0; k < recolor.cells.Length; k++)
        {
            int idx = recolor.cells[k];
            pixels[idx] = recolor.original[k];
            flashing[idx] = false;
        }
        recolor = null;
        texDirty = true;
    }

    public void ConfirmRecolor()
    {
        if (recolor == null) return;

        Color32 b = palette[(int)recolor.target];
        byte id = (byte)((int)recolor.target + 1);
        for (int k = 0; k < recolor.cells.Length; k++)
        {
            int idx = recolor.cells[k];
            float j = recolor.jitter[k];
            cells[idx] = id;
            pixels[idx] = new Color32((byte)(b.r * j), (byte)(b.g * j), (byte)(b.b * j), 255);
            flashing[idx] = false;
        }

        recolor = null;
        texDirty = true;
        active = true;      
    }

    private void UpdateRecolorPreview()
    {
        if (recolor == null) return;

        int phase = ((int)(Time.time * recolorBlinkHz * 2f)) & 1;
        if (phase == recolor.lastPhase) return;
        recolor.lastPhase = phase;

        Color32 b = palette[(int)recolor.target];
        for (int k = 0; k < recolor.cells.Length; k++)
        {
            if (phase == 0)
            {
                pixels[recolor.cells[k]] = recolor.original[k];
            }
            else
            {
                float j = recolor.jitter[k];
                pixels[recolor.cells[k]] = new Color32((byte)(b.r * j), (byte)(b.g * j), (byte)(b.b * j), 255);
            }
        }
        texDirty = true;
    }

    public void ClearAll()
    {
        System.Array.Clear(cells, 0, cells.Length);
        System.Array.Clear(pixels, 0, pixels.Length);
        System.Array.Clear(flashing, 0, flashing.Length);
        recolor = null;
        flashGroups.Clear();
        active = false;
        texDirty = true;
    }
}
