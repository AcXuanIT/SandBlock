using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

[CreateAssetMenu(fileName = "ColorData", menuName = "ScriptableObjects/ColorData")]
public class ColorData : ScriptableObject
{
    public ColorDetail Red = new ColorDetail { colorType = ColorType.Red, colorValue = Color.red };
    public ColorDetail Green = new ColorDetail { colorType = ColorType.Green, colorValue = Color.green };
    public ColorDetail Blue = new ColorDetail { colorType = ColorType.Blue, colorValue = Color.blue };
    public ColorDetail Yellow = new ColorDetail { colorType = ColorType.Yellow, colorValue = Color.yellow };
    public ColorDetail Purple = new ColorDetail { colorType = ColorType.Purple, colorValue = new Color(0.5f, 0f, 0.5f) };
    public ColorDetail Orange = new ColorDetail { colorType = ColorType.Orange, colorValue = new Color(1f, 0.5f, 0f) };
    public ColorDetail Cyan = new ColorDetail { colorType = ColorType.Cyan, colorValue = Color.cyan };
    public ColorDetail Magenta = new ColorDetail { colorType = ColorType.Magenta, colorValue = Color.magenta };

    public ColorDetail GetRandom()
    {
        ColorDetail[] colorDetails = new ColorDetail[] { Red, Green, Blue, Yellow, Purple, Orange, Cyan, Magenta };
        int randomIndex = Random.Range(0, colorDetails.Length);
        return colorDetails[randomIndex];
    }
}

[System.Serializable]
public class ColorDetail
{
    public ColorType colorType;
    public Color colorValue;
}

public enum ColorType
{
    Red,
    Green,
    Blue,
    Yellow,
    Purple,
    Orange,
    Cyan,
    Magenta,
}
