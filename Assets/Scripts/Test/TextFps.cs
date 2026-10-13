using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextFps : MonoBehaviour
{
    [SerializeField] private TMP_Text fpsText;

    private float timer;
    private int frameCount;

    void Update()
    {
        timer += Time.unscaledDeltaTime;
        frameCount++;

        // Cập nhật FPS mỗi 0.5 giây
        if (timer >= 0.5f)
        {
            float fps = frameCount / timer;

            fpsText.text = $"FPS: {fps:F0}";

            timer = 0f;
            frameCount = 0;
        }
    }
}
