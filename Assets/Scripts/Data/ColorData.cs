using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

[CreateAssetMenu(fileName = "ColorData", menuName = "ScriptableObjects/ColorData")]
public class ColorData : ScriptableObject
{
    public ColorDetail Red = new ColorDetail { colorType = ColorType.Red, colorValue = Color.red , nameColor = "Red" };
    public ColorDetail Green = new ColorDetail { colorType = ColorType.Green, colorValue = Color.green, nameColor = "Green" };
    public ColorDetail Blue = new ColorDetail { colorType = ColorType.Blue, colorValue = Color.blue, nameColor = "Blue" };
    public ColorDetail Yellow = new ColorDetail { colorType = ColorType.Yellow, colorValue = Color.yellow, nameColor = "Yellow" };
    public ColorDetail Purple = new ColorDetail { colorType = ColorType.Purple, colorValue = new Color(0.5f, 0f, 0.5f), nameColor = "Purple" };
    public ColorDetail Orange = new ColorDetail { colorType = ColorType.Orange, colorValue = new Color(1f, 0.5f, 0f), nameColor = "Orange" };
    public ColorDetail Cyan = new ColorDetail { colorType = ColorType.Cyan, colorValue = Color.cyan, nameColor = "Cyan" };
    public ColorDetail Magenta = new ColorDetail { colorType = ColorType.Magenta, colorValue = Color.magenta, nameColor = "Magenta" };

    [System.NonSerialized] private ColorDetail[] cache;
    public ColorDetail[] All
    {
        get
        {
            if (cache == null)
                cache = new[] { Red, Green, Blue, Yellow, Purple, Orange, Cyan, Magenta };
            return cache;
        }
    }
    public ColorDetail GetRandom()
    {
        ColorDetail[] all = All;
        return all[Random.Range(0, GameManager.Instance.colors)];
    }
    public ColorDetail GetColorByIndex(int index)
    {
        ColorDetail[] all = All;
        if(index > 0 &&  index <= all.Length) 
            {
                return all[index - 1];
            }
            return null;
    }

    public int CountsColor()
    {
        ColorDetail[] all = All;
        return all.Length;
    }

    private void OnValidate()
    {
        cache = null;
    }
}

[System.Serializable]
public class ColorDetail
{
    public ColorType colorType;
    public Color colorValue;
    public string nameColor;
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
