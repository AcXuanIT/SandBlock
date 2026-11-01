using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class SelelctColorButton : MonoBehaviour
{
    [SerializeField] public GameObject selectColorUI;
    [SerializeField] public GameObject colorUI;

    [SerializeField] public Button btnBack;

    public List<ColorButton> Buttons = new List<ColorButton>();

    public void InitSelect()
    {
        gameObject.SetActive(false);
        InitColorUI();
        btnBack.onClick.AddListener(() =>
        {
            FunctionSystem.Instance.EnterMode(FunctionSystem.BoosterMode.Recolor);
            gameObject.SetActive(false);
        });
    }
    private void OnEnable()
    {
        //InitColorUI();
        SetActiveColorButton();
        SetShowOutlineColor();
    }


    public void InitColorUI()
    {
        ColorDetail[] colors = GridSystem.Instance.colorData.All;
        ClearAll();
        for (int i = 0; i < colors.Length; i++)
        {
            ColorDetail color = colors[i];
            GameObject btnObj = PoolingManager.Spawn(colorUI, transform.position, quaternion.identity, selectColorUI.transform);
            ColorButton btn = btnObj.GetComponent<ColorButton>();
            btn.Init(color);
            btn.gameObject.SetActive(false);
            Buttons.Add(btn);
        }
    }

    public void SetActiveColorButton()
    {
        for (int i = 0; i < GameManager.Instance.colors; i++)
        {
            Buttons[i].gameObject.SetActive(true);
        }
    }
    public void ClearAll()
    {
        foreach(ColorButton btn in Buttons)
        {
            PoolingManager.Despawn(btn.gameObject);
        }
        Buttons.Clear();
    }

    public void SetShowOutlineColor()
    {
        foreach(ColorButton color in Buttons)
        {
            if(color.colorType == FunctionSystem.Instance.RecolorTarget)
            {
                color.SetActiveOutline(true);
            }
            else
            {
                color.SetActiveOutline(false);
            }
        }
    }

}
