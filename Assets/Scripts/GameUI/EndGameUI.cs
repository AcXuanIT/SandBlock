using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndGameUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button btnAgain;

    [Header("Panel")]
    [SerializeField] private GameObject panelEndGame;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI bestscore;

    private void Awake()
    {
        btnAgain.onClick.AddListener(() =>
        {
            GameManager.Instance.OnReplayGame();
            panelEndGame.SetActive(false);
        });
    }
    public void OnEndGame()
    {
        panelEndGame.SetActive(true);
        SetText();
    }

    public void SetText()
    {
        int s = GameManager.Instance.scoreInGame;
        int bs = PlayerPrefs.GetInt("BestScore", 0);

        score.text = s.ToString("N0", CultureInfo.InvariantCulture);
        bestscore.text = bs.ToString("N0", CultureInfo.InvariantCulture);
    }
}
