using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReviveUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] public Button btnRevive;

    [Header("Text")]
    [SerializeField] public TextMeshProUGUI txtTime;

    [Header("Panel")]
    [SerializeField] public GameObject panelRevive;

    public float timeDelay = 10f;
    public float time = 0f;

    private void Awake()
    {
        btnRevive.onClick.AddListener(() =>
        {

        });
    }

    public void OnRevive()
    {
        panelRevive.SetActive(true);
        time = 0f;
        SetTextTime(timeDelay);

        DelayRevive(timeDelay);
    }


    public void ReviveGame()
    {
        panelRevive.SetActive(false);
        GameManager.Instance.OnReviveGame();
    }

    public void DelayRevive(float time)
    {
        int lastSecond = -1;
        DOVirtual.Float(time, 0f,time, value =>
        {
            int second = Mathf.CeilToInt(value);
            if (second != lastSecond)         
            {
                lastSecond = second;
                SetTextTime(lastSecond);
            }
        })
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                GameManager.Instance.OnOverGame();
                panelRevive.SetActive(false);
            });
    }

    public void SetTextTime(float timer)
    {
        int t = (int)timer;
        txtTime.text = t.ToString();
    }
    
}
