using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuGameUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] public Button btnPlay;
    [SerializeField] public Button btnSettings;

    [Header("Panel")]
    [SerializeField] public GameObject panelMenu;

    [Header("Score")]
    [SerializeField] public TextMeshProUGUI txtBestScore;

    private void Awake()
    {
        panelMenu.SetActive(true);
        InitScore();

        btnPlay.onClick.AddListener(() =>
        {
            GameManager.Instance.OnStartGame();
            OnPlay();
        });
        btnSettings.onClick.AddListener(() =>
        {
            OnSettings();
        });
    }

    private void OnEnable()
    {
       InitScore();
    }

    public void InitScore()
    {
        int score = PlayerPrefs.GetInt("BestScore", 0);

        txtBestScore.text = score.ToString("N0", CultureInfo.InvariantCulture);
    }

    public void OnPlay()
    {
        panelMenu.SetActive(false);
    }

    public void OnHome()
    {
        panelMenu.SetActive(true);
    }

    public void OnSettings()
    {
        GameManager.Instance.settingsUI.Settings();
    }
}
