using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UnityEngine.Audio;

public class StartSceneDirector : MonoBehaviour
{
    public enum weaponEnum
    {
        weapon0, weapon1, weapon2
    }
    public static weaponEnum weapon { get; private set; }


    public enum stageEnum
    {
        stage0, stage1
    }
    public static stageEnum stage { get; private set;}

    [SerializeField] TextMeshProUGUI[] txt_Title_Stage;
    [SerializeField] TextMeshProUGUI txt_Explanation_Stage;
    [SerializeField] Image[] icon_Stage;
    [Serializable] class info_Stage
    {
        public string title;
        public string explanation;
        public Sprite icon;
    }
    [SerializeField] info_Stage[] infos_St;


    [SerializeField] TextMeshProUGUI[] txt_Title_Weapon;
    [SerializeField] TextMeshProUGUI txt_Explanation_Weapon;
    [SerializeField] Image[] icon_Weapon;
    [Serializable] class info_Weapon
    {
        public string title;
        public string explanation;
        public Sprite icon;
    }
    [SerializeField] info_Weapon[] infos_Wp;

    weaponEnum weapon_WatchedNow;
    stageEnum stage_WatchedNow;

    [SerializeField] GameObject tranjitionPanel;

    bool alreadyGoToGame;

    [SerializeField] AudioMixer audioMixer;

    [SerializeField] Slider BGM_Slider;
    [SerializeField] Slider SE_Slider;

    private void Start()
    {
        switch (stage)
        {
            case stageEnum.stage0:
                ShowDescription_Stage(0);
                stage_WatchedNow = stageEnum.stage0;
                break;

            case stageEnum.stage1:
                ShowDescription_Stage(1);
                stage_WatchedNow = stageEnum.stage1;
                break;
        }

        switch(weapon)
        {
            case weaponEnum.weapon0:
                ShowDescription_Weapon(0);
                weapon_WatchedNow= weaponEnum.weapon0;
                break;

            case weaponEnum.weapon1:
                ShowDescription_Weapon(1);
                weapon_WatchedNow = weaponEnum.weapon1;
                break;

            case weaponEnum.weapon2:
                ShowDescription_Weapon(2);
                weapon_WatchedNow = weaponEnum.weapon2;
                break;
        }

        audioMixer.GetFloat("BGM", out float bgmVol);
        BGM_Slider.value = bgmVol;

        audioMixer.GetFloat("SE", out float seVol);
        SE_Slider.value = seVol;

        alreadyGoToGame = false;
    }

    public void SelectWeapon()
    {
        weapon = weapon_WatchedNow;
    }

    public void SelectStage()
    {
        stage = stage_WatchedNow;
    }

    public void GoToMainScene()
    {
        LoadMainScene().Forget();
    }

    async UniTask LoadMainScene()
    {
        if (alreadyGoToGame) return;

        alreadyGoToGame = true;

        tranjitionPanel.SetActive(true);

        tranjitionPanel.TryGetComponent(out RectTransform _rect);

        await _rect.DOAnchorPos(Vector3.zero, 0.8f).ToUniTask();

        await SceneManager.LoadSceneAsync("MainScene");
    }

    public void ShowDescription_Stage(int x)
    {
        foreach(TextMeshProUGUI y in txt_Title_Stage)
        {
            y.text = infos_St[x].title;
        }

        txt_Explanation_Stage.text = infos_St[x].explanation;

        foreach(Image z in icon_Stage)
        {
            z.overrideSprite = infos_St[x].icon;
        }

        switch (x)
        {
            case 0:
                stage_WatchedNow = stageEnum.stage0;
                break;

            case 1:
                stage_WatchedNow = stageEnum.stage1;
                break;

            default:
                stage_WatchedNow = stageEnum.stage0;
                break;

        }
    }

    public void ShowDescription_Weapon(int x)
    {
        foreach (TextMeshProUGUI y in txt_Title_Weapon)
        {
            y.text = infos_Wp[x].title;
        }

        txt_Explanation_Weapon.text = infos_Wp[x].explanation;

        foreach (Image z in icon_Weapon)
        {
            z.overrideSprite = infos_Wp[x].icon;
        }

        switch(x)
        {
            case 0:
                weapon_WatchedNow = weaponEnum.weapon0;
                break;

            case 1:
                weapon_WatchedNow = weaponEnum.weapon1;
                break;

            case 2:
                weapon_WatchedNow= weaponEnum.weapon2;
                break;
        }
    }

    public void SetBGMvol(float x)
    {
        audioMixer.SetFloat("BGM", x);
    }

    public void SetSEvol(float x)
    {
        audioMixer.SetFloat("SE", x);
    }
}
