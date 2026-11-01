using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Outline outline;
    [SerializeField] private Image image;
    public ColorType colorType;

    public void Init(ColorDetail color)
    {
        image.color = color.colorValue;
        colorType = color.colorType;

        button.onClick.AddListener(() =>
        {
            FunctionSystem.Instance.SetRecolorColor(color.colorType);
        });
    }

    public void SetActiveOutline(bool active)
    {
        outline.enabled = active;
    }
}
