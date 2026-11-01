using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] public Button btnHome;
    [SerializeField] public Button btnReplay;
    [SerializeField] public Button btnBack;
    [SerializeField] public Button btnMusic;
    [SerializeField] public Button btnVFX;

    [Header("Sprite")]
    [SerializeField] public Sprite spriteMusic;
    [SerializeField] public Sprite spriteNotMusic;
    [SerializeField] public Sprite spriteVFX;
    [SerializeField] public Sprite spriteNotVFX;

    [Header("Image")]
    [SerializeField] public Image iconMusic;
    [SerializeField] public Image iconVFX;

    [Header("Panel")]
    [SerializeField] public GameObject panelPause;

    private int music;
    private int vfx;

    private void Awake()
    {
        InitValue();

        btnHome.onClick.AddListener(() =>
        {
            GameManager.Instance.OnHome();
            panelPause.SetActive(false);
        });
        btnReplay.onClick.AddListener(() =>
        {
            GameManager.Instance.OnReplayGame();
            panelPause.SetActive(false);
        });
        btnBack.onClick.AddListener(() =>
        {
            panelPause.SetActive(false);
        });
        btnMusic.onClick.AddListener(() =>
        {
            if (music == 0) music = 1;
            else music = 0;

            PlayerPrefs.SetInt("MusicValue", music);

            MusicUI(music);
        });
        btnVFX.onClick.AddListener(() =>
        {
            if (vfx == 0) vfx = 1;
            else vfx = 0;

            PlayerPrefs.SetInt("VFXValue", vfx);

            VFXUI(vfx);
        });
    }
    private void OnEnable()
    {
        InitValue();
    }
    public void InitValue()
    {
        music = PlayerPrefs.GetInt("MusicValue", 1);
        vfx = PlayerPrefs.GetInt("VFXValue", 1);

        MusicUI(music);
        VFXUI(vfx);
    }
    public void OnPause()
    {
        panelPause.SetActive(true);
    }

    public void OnHome()
    {
        GameManager.Instance.OnHome();
        panelPause.SetActive(false);
    }
    public void OnReplay()
    {
        GameManager.Instance.OnReplayGame();
        panelPause.SetActive(false);
    }
    
    public void MusicUI(int music)
    {
        if(music == 0)
        {
            iconMusic.sprite = spriteMusic;
        }
        else
        {
            iconMusic.sprite = spriteNotMusic;
        }
    }
    public void VFXUI(int vfx)
    {
        if(vfx == 0)
        {
            iconVFX.sprite = spriteVFX;
        }
        else
        {
            iconVFX.sprite = spriteNotVFX;
        }
    }
}
