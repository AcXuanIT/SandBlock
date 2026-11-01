using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] public TextMeshProUGUI txtScore;

    [Header("Buttons")]
    [SerializeField] public Button btnPause;

    private void Awake()
    {
        btnPause.onClick.AddListener(() =>
        {
            GameManager.Instance.OnPauseGame();
        });
    }
    public void SetScore(int score)
    {
        txtScore.text = score.ToString("N0", CultureInfo.InvariantCulture);
    }
}
