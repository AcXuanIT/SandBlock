using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] public Button btnMusic;
    [SerializeField] public Button btnVFX;
    [SerializeField] public Button btnRung;
    [SerializeField] public Button btnBack;

    [Header("Panel")]
    [SerializeField] public GameObject panelSettings;

    [Header("Image")]
    [SerializeField] public Image iconMusic;
    [SerializeField] public Image iconVFX;
    [SerializeField] public Image iconRung;

    [SerializeField] public Image iconBGMusic;
    [SerializeField] public Image iconBGVFX;
    [SerializeField] public Image iconBGRung;

    [Header("Sprites")]
    [SerializeField] public Sprite spriteMusic;
    [SerializeField] public Sprite spriteNotMusic;
    [SerializeField] public Sprite spriteVFX;
    [SerializeField] public Sprite spriteNotVFX;
    [SerializeField] public Sprite spriteRung;
    [SerializeField] public Sprite spriteNotRung;
    [SerializeField] public Sprite spriteBackGroundOn;
    [SerializeField] public Sprite spriteBackGroundOff;

    [Header("Music Value")]
    [SerializeField] public int music;
    [SerializeField] public int vfx;
    [SerializeField] public int rung;


    private void Awake()
    {
        InitMusicValue();

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
        btnRung.onClick.AddListener(() =>
        {
            if (rung == 0) rung = 1;
            else rung = 0;

            PlayerPrefs.SetInt("RungValue", rung);

            RungUI(rung);
        });
        btnBack.onClick.AddListener(() =>
        {
            OnBackSetting();
        });

    }
    private void OnEnable()
    {
        InitMusicValue();
    }
    public void InitMusicValue()
    {
        music = PlayerPrefs.GetInt("MusicValue", 1);
        vfx = PlayerPrefs.GetInt("VFXValue", 1);
        rung = PlayerPrefs.GetInt("RungValue", 1);

        MusicUI(music);
        VFXUI(vfx);
        RungUI(rung);
    }

    public void Settings()
    {
        panelSettings.SetActive(true);
    }

    public void OnBackSetting()
    {
        panelSettings.SetActive(false);
    }

    public void MusicUI(int m)
    {
        if(m == 1)
        {
            iconBGMusic.sprite = spriteBackGroundOn;
            iconMusic.sprite = spriteMusic;
        }
        else
        {
            iconBGMusic.sprite = spriteBackGroundOff;
            iconMusic.sprite = spriteNotMusic;
        }
    }
    public void VFXUI(int v)
    {
        if(v == 1)
        {
            iconBGVFX.sprite = spriteBackGroundOn;
            iconVFX.sprite = spriteVFX;
        }
        else
        {
            iconBGVFX.sprite = spriteBackGroundOff;
            iconVFX.sprite = spriteNotVFX;
        }
    }
    public void RungUI(int r)
    {
        if(r ==1)
        {
            iconBGRung.sprite = spriteBackGroundOn;
            iconRung.sprite = spriteRung;
        }
        else
        {
            iconBGRung.sprite = spriteBackGroundOff;
            iconRung.sprite = spriteNotRung;
        }
    }
}
