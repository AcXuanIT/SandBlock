using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FunctionSystem : Singleton<FunctionSystem>
{
    public enum BoosterMode { None, Bomb, Recolor, ClearColor }

    [Header("Bomb")]
    public int bombRadius = 20;

    [Header("Overlay (xem truoc)")]
    public Color bombAreaTint = new Color(1f, 0.15f, 0.15f, 0.18f);
    public Color bombSandTint = new Color(1f, 0.15f, 0.15f, 0.65f);
    public Color highlightTint = new Color(1f, 1f, 1f, 0.5f);
    [Range(0.02f, 0.5f)] public float overlayRefreshInterval = 0.05f;

    [Header("Button Functions")]
    [SerializeField] public Button btnDeleteAllOneColor;
    [SerializeField] public Button btnChangeColor;
    [SerializeField] public Button btnUseBoom;
    [SerializeField] public Image iconUseBoom;
    [SerializeField] public Image iconChangeColor;
    [SerializeField] public Image iconDeleteAllOneColor;
    [SerializeField] public Sprite imgIconDeleteAllOneColor;
    [SerializeField] public Sprite imgIconChangeColor;
    [SerializeField] public Sprite imgIconBoom;
    [SerializeField] public Sprite imgBack;
    public SelelctColorButton selectColor;

    public BoosterMode Mode { get; private set; }
    public bool IsActive => Mode != BoosterMode.None;
    public ColorType RecolorTarget { get; private set; }
    public bool HasHoverColor { get; private set; }
    public ColorType HoverColor { get; private set; }
    public bool HasRecolorRegion => grid != null && grid.HasRecolorPreview;

    private GridSystem grid;
    private Camera cam;
    private int[] buffer;

    private Texture2D overlayTex;
    private Color32[] overlayPx;
    private int[] dirty;
    private int dirtyCount;
    private bool overlayDirty;

    private int lastCol = -1, lastRow = -1;
    private float lastDraw;

    private void Awake()
    {
        btnDeleteAllOneColor.onClick.AddListener(() =>
        {
            FunctionSystem.Instance.EnterMode(FunctionSystem.BoosterMode.ClearColor);
        });

        btnChangeColor.onClick.AddListener(() =>
        {
            RecolorTarget = grid.colorData.All[0].colorType;
            FunctionSystem.Instance.EnterMode(FunctionSystem.BoosterMode.Recolor);
            selectColor.gameObject.SetActive(true);
        });

        btnUseBoom.onClick.AddListener(() =>
        {
            FunctionSystem.Instance.EnterMode(FunctionSystem.BoosterMode.Bomb);
        });
    }
    public void InitBooster()
    {
        grid = GridSystem.Instance;
        cam = Camera.main;
        int n = grid.Col * grid.Row;
        buffer = new int[n];
        dirty = new int[n];
        overlayPx = new Color32[n];

        overlayTex = new Texture2D(grid.Col, grid.Row, TextureFormat.RGBA32, false)
        {
            name = "BoosterOverlay",
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        overlayTex.SetPixels32(overlayPx);
        overlayTex.Apply(false);

        Sprite sprite = Sprite.Create(overlayTex, new Rect(0, 0, grid.Col, grid.Row), Vector2.zero,
                                      1f / grid.cellSize, 0, SpriteMeshType.FullRect);
        var go = new GameObject("BoosterOverlay");
        go.transform.SetParent(transform, false);
        go.transform.position = new Vector3(-grid.GridWidth, -grid.GridLength, 0f);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = grid.sortingOrder + 1;
    }

    /// <summary>Bam lai dung nut dang chon = huy che do.</summary>
    public void EnterMode(BoosterMode m)
    { 

        if (m == Mode)
        {
            Cancel();
            return;
        }
        CancelInternal();
        Mode = m;
        Notify();
    }


    public void Cancel()
    {
        CancelInternal();
        Mode = BoosterMode.None;
        Notify();
    }

    private void CancelInternal()
    {
        if (grid != null && grid.HasRecolorPreview) grid.CancelRecolor();
        ClearOverlayPixels();
        HasHoverColor = false;
        lastCol = lastRow = -1;
    }

    private void Finish()
    {
        ClearOverlayPixels();
        HasHoverColor = false;
        lastCol = lastRow = -1;
        Mode = BoosterMode.None;
        Notify();
    }

    private void Notify()
    {
        switch(Mode)
        {
            case BoosterMode.None:
                iconChangeColor.sprite = imgIconChangeColor;
                iconDeleteAllOneColor.sprite = imgIconDeleteAllOneColor;
                iconUseBoom.sprite = imgIconBoom;
                selectColor.gameObject.SetActive(false);
                break;
            case BoosterMode.Bomb:
                iconUseBoom.sprite = imgBack;
                break;
            case BoosterMode.Recolor:
                iconChangeColor.sprite = imgBack;
                break;
            case BoosterMode.ClearColor:
                iconDeleteAllOneColor.sprite = imgBack;
                break;
        }
    }

    public void BombPreview(int col, int row)
    {
        ClearOverlayPixels();

        int r = bombRadius;
        int r2 = r * r;
        Color32 sand = bombSandTint;
        Color32 area = bombAreaTint;

        for (int dy = -r; dy <= r; dy++)
        {
            int y = row + dy;
            if (y < 0 || y >= grid.Row) continue;
            for (int dx = -r; dx <= r; dx++)
            {
                if (dx * dx + dy * dy > r2) continue;
                int x = col + dx;
                if (x < 0 || x >= grid.Col) continue;
                PaintOverlay(y * grid.Col + x, grid.SelectableValue(x, y) != 0 ? sand : area);
            }
        }
    }

    public bool BombConfirm(int col, int row)
    {
        int n = grid.CollectCircle(col, row, bombRadius, buffer);
        if (n == 0) return false;

        grid.StartClearFlash(buffer, n);
        Finish();
        return true;
    }

    // ----------------------------------------------------- ClearColor

    public void ColorHover(int col, int row)
    {
        ClearOverlayPixels();

        byte v = grid.SelectableValue(col, row);
        if (v == 0)
        {
            if (HasHoverColor)
            {
                HasHoverColor = false;
                Notify();
            }
            return;
        }

        ColorType t = (ColorType)(v - 1);
        if (!HasHoverColor || HoverColor != t)
        {
            HasHoverColor = true;
            HoverColor = t;
            Notify();
        }

        int n = grid.CollectColor(v, buffer);
        Color32 c = highlightTint;
        for (int k = 0; k < n; k++) PaintOverlay(buffer[k], c);
    }

    public bool ColorConfirm()
    {
        if (!HasHoverColor) return false;

        byte id = (byte)((int)HoverColor + 1);
        int n = grid.CollectColor(id, buffer);
        if (n == 0) return false;

        grid.StartClearFlash(buffer, n);
        Finish();
        return true;
    }

    // -------------------------------------------------------- Recolor

    public void SetRecolorColor(ColorType t)
    {
        RecolorTarget = t;
        grid.SetRecolorTarget(t);
        selectColor.SetShowOutlineColor();
        Notify();
    }

    public bool RecolorPick(int col, int row)
    {
        if (grid.SelectableValue(col, row) == 0) return false;

        int n = grid.CollectRegion(col, row, buffer);

        if (!grid.BeginRecolorPreview(buffer, n, RecolorTarget)) return false;

        Notify();

        return true;
    }

    public void RecolorConfirm()
    {
        if (Mode != BoosterMode.Recolor || !grid.HasRecolorPreview) return;

        grid.ConfirmRecolor();
        Finish();
    }

    // -------------------------------------------------------- Input

    private void Update()
    {
        if (!IsActive) return;

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
        {
            Cancel();
            return;
        }

        bool overUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        Vector2 world = cam.ScreenToWorldPoint(Input.mousePosition);
        grid.WorldToCellFloor(world, out int col, out int row);
        bool inside = col >= 0 && col < grid.Col && row >= 0 && row < grid.Row;
        bool hovering = inside && !overUI;
        bool click = hovering && Input.GetMouseButtonDown(0);
        bool redraw = hovering && (col != lastCol || row != lastRow ||
                                   Time.unscaledTime - lastDraw >= overlayRefreshInterval);

        switch (Mode)
        {
            case BoosterMode.Bomb:
                if (click && BombConfirm(col, row)) return;
                if (redraw) { BombPreview(col, row); MarkDrawn(col, row); }
                else if (!hovering) HideHover();
                break;

            case BoosterMode.ClearColor:
                if (click && ColorHover(col, row, true) && ColorConfirm()) return;
                if (redraw) { ColorHover(col, row); MarkDrawn(col, row); }
                else if (!hovering) HideHover();
                break;

            case BoosterMode.Recolor:
                if (click)
                {
                    if (grid.HasRecolorPreview && grid.IsInRecolorRegion(col, row))
                    {
                        RecolorConfirm();
                        return;
                    }

                    RecolorPick(col, row);
                }
                break;
        }
    }

    // lam moi lua chon tai o truoc khi xac nhan (cat co the vua di chuyen)
    private bool ColorHover(int col, int row, bool refreshOnly)
    {
        ColorHover(col, row);
        return HasHoverColor;
    }

    private void MarkDrawn(int col, int row)
    {
        lastCol = col;
        lastRow = row;
        lastDraw = Time.unscaledTime;
    }

    private void HideHover()
    {
        if (lastCol == -1 && !HasHoverColor) return;

        ClearOverlayPixels();
        lastCol = lastRow = -1;
        if (HasHoverColor)
        {
            HasHoverColor = false;
            Notify();
        }
    }

    // ------------------------------------------------------ Overlay

    private void ClearOverlayPixels()
    {
        if (dirtyCount == 0) return;
        for (int k = 0; k < dirtyCount; k++) overlayPx[dirty[k]] = default;
        dirtyCount = 0;
        overlayDirty = true;
    }

    private void PaintOverlay(int idx, Color32 c)
    {
        overlayPx[idx] = c;
        dirty[dirtyCount++] = idx;
        overlayDirty = true;
    }

    private void LateUpdate()
    {
        if (!overlayDirty) return;
        overlayTex.SetPixels32(overlayPx);
        overlayTex.Apply(false);
        overlayDirty = false;
    }
}
