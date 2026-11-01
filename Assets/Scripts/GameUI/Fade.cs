using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
    [Header("Sprite")]
    public SpriteRenderer sr;
    public Sprite fadeSprite;
    [SerializeField] public Color tint;
    public int sortingOrderOffset = 1;

    [Header("Canh bao")]
    [Range(0.1f, 1f)] public float warningRatio = 0.7f;

    [Header("Kich thuoc")]
    [Range(0f, 1f)] public float fadeStartRatio = 0.8f;
    [Range(0.05f, 1f)] public float lengthRatio = 0.2f;
    [Range(0.1f, 1f)] public float fadeFractionOfSprite = 0.5f;

    [Header("Nhay")]
    public float blinkInterval = 2f;
    public float pulseDuration = 0.6f;
    [Range(0f, 1f)] public float maxAlpha = 1f;

    private GridSystem grid;
    private float timer;
    private bool wasWarning;

    public void InitFade()
    {
        grid = GridSystem.Instance;
        sr.sprite = fadeSprite;
        sr.sortingOrder = grid.sortingOrder + sortingOrderOffset;
        sr.enabled = false;

        Layout();
    }

    private void Update()
    {
        if (grid == null) return;
        if (fadeSprite == null) return;

        sr.sprite = fadeSprite;

        int warningRow = Mathf.RoundToInt(grid.Row * warningRatio);
        bool warning = !grid.IsGameOver && grid.HighestRow >= warningRow;

        if (warning)
        {
            if (!wasWarning) timer = 0f;         
            timer += Time.deltaTime;
        }
        wasWarning = warning;

        float alpha = warning ? PulseAlpha(timer) * maxAlpha : 0f;
        sr.enabled = alpha > 0.001f;
        if (!sr.enabled) return;

        //Layout();
        sr.color = new Color(tint.r, tint.g, tint.b, tint.a * alpha);
    }

    public float PulseAlpha(float t)
    {
        float interval = Mathf.Max(0.01f, blinkInterval);
        float duration = Mathf.Clamp(pulseDuration, 0.01f, interval);
        float local = t % interval;
        return local < duration ? Mathf.Sin(local / duration * Mathf.PI) : 0f;
    }

    private void Layout()
    {
        float gridH = grid.GridLength * 2f;

        float ppu = fadeSprite.pixelsPerUnit;
        Rect rect = fadeSprite.rect;

        float sx = (grid.GridWidth * 2f) / (rect.width / ppu);
        float sy = (lengthRatio * gridH) / ((rect.height / ppu) * fadeFractionOfSprite);

        float startPx = rect.height * (1f - fadeFractionOfSprite);
        float startOffsetY = (startPx - fadeSprite.pivot.y) / ppu * sy;
        float centerOffsetX = (rect.width * 0.5f - fadeSprite.pivot.x) / ppu * sx;

        float targetY = -grid.GridLength + fadeStartRatio * gridH;

        Transform t = sr.transform;
        t.localScale = new Vector3(sx, sy, 1f);
        t.position = new Vector3(-centerOffsetX, targetY - startOffsetY, 0f);
    }
}
