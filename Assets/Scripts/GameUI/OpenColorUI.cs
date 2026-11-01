using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OpenColorUI : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI textNameTitle;
    [SerializeField] public Image imageColor;
    [SerializeField] public TextMeshProUGUI textNameColor;
    [SerializeField] public Button btnConfirm;
    [SerializeField] public GameObject panel;

    [SerializeField] public RectTransform rec;
    public float width = 1080f;
    public float duration = 2f;

    private void Awake()
    {
        btnConfirm.onClick.AddListener(() =>
        {
            OffShow();
            GameManager.Instance.isUpColor = false;
        });
    }

    public void OpenColor(ColorDetail color)
    {
        imageColor.color = color.colorValue;
        textNameColor.color = color.colorValue;
        textNameColor.text = color.nameColor;
        panel.SetActive(true);

        DOVirtual.Float(-width, 0f, duration, value =>
        {
            rec.anchoredPosition = new Vector3(value, 0f, 0f);
        });
    }

    public void OffShow()
    {
        DOVirtual.Float(0f, width, duration, value =>
        {
            rec.anchoredPosition = new Vector3(value, 0f, 0f);
        }).OnComplete(() =>
        {
            panel.SetActive(false);
        });
    }
}
