using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("UI")]
    [SerializeField] public MenuGameUI menuGameUI;
    [SerializeField] public SettingsUI settingsUI;
    [SerializeField] public GameUI gameUI;
    [SerializeField] public PauseUI pauseUI;
    [SerializeField] public EndGameUI endGameUI;
    [SerializeField] public ReviveUI reviveUI;
    [SerializeField] public OpenColorUI openColorUI;

    [Header("Grid")]
    [SerializeField] public Line line;
    [SerializeField] public Fade fade;

    [Header("Score")]
    public int scoreInGame;
    public int bestScore;
    public Score scoreUI;
    public int scorePerBlock = 10;
    public int checkScore = 5000;

    [Header("Color")]
    public int colors;
    public bool isUpColor = false;

    public bool isPause = false;

    public void OnStartGame()
    {
        isUpColor = false;
        isPause = false;
        scoreInGame = 0;
        colors = 3;
        checkScore = 5000;
        SetScore(0);
        GridSystem.Instance.InitializeGrid();
        FunctionSystem.Instance.InitBooster();
        FunctionSystem.Instance.selectColor.InitSelect();
        ItemSystem.Instance.SpawnItems();

        line.InitLine();
        fade.InitFade();
    }
    public void OnPauseGame()
    {
        isPause = true;
        pauseUI.OnPause();
    }
    public void OnContinue()
    {
        isPause = false;
    }
    public void OnHome()
    {
        menuGameUI.OnHome();

        GridSystem.Instance.ClearAll();
        ItemSystem.Instance.OnClearAll();
    }
    public void OnReplayGame()
    {
        isUpColor = false;
        isPause = false;
        scoreInGame = 0;
        colors = 3;
        checkScore=5000;
        SetScore(0);
        GridSystem.Instance.Restart();

        ItemSystem.Instance.OnRestartItem();
    }
    public void OnReviveGame()
    {
        
    }
    public void OnKeepPlaying()
    {
        reviveUI.OnRevive();
    }
    public void OnOverGame()
    {
        endGameUI.OnEndGame();
    }
    
    public void SetScore(int score)
    {
        scoreInGame += score;
        gameUI.SetScore(scoreInGame);
        bestScore = PlayerPrefs.GetInt("BestScore", 0);
        if(score > bestScore)
        {
            bestScore = score;
            PlayerPrefs.SetInt("BestScore", bestScore);
        }
        if(scoreInGame > checkScore)
        {
            UpColor();
            checkScore = checkScore * 2;
        }
    }

    public void UpColor()
    {
        int t = ItemSystem.Instance.colorData.CountsColor();

        if (colors + 1 > t)
        {
            colors = t;
        }
        else

        {
            colors++;
            isUpColor = true;
            openColorUI.OpenColor(ItemSystem.Instance.colorData.GetColorByIndex(colors));
            ItemSystem.Instance.SpawnItemByColor(ItemSystem.Instance.colorData.GetColorByIndex(colors));
        }
    }
}
