using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UnityEngine.Audio;

public class StartSceneDirector : SingletonMono<StartSceneDirector>
{
    [SerializeField] GameObject tranjitionPanel;

    [SerializeField] AudioMixer audioMixer;

    [SerializeField] Slider BGM_Slider;
    [SerializeField] Slider SE_Slider;

    [SerializeField] Button button_GoStageSelect;

    bool alreadyGoToGame;

    private void Awake()
    {
        button_GoStageSelect.onClick.AddListener(() => LoadMainScene().Forget());
    }

    private void Start()
    {
        audioMixer.GetFloat("BGM", out float bgmVol);
        BGM_Slider.value = bgmVol;

        audioMixer.GetFloat("SE", out float seVol);
        SE_Slider.value = seVol;

        alreadyGoToGame = false;
    }

    async UniTask LoadMainScene()
    {
        if (alreadyGoToGame) return;

        alreadyGoToGame = true;

        tranjitionPanel.SetActive(true);

        tranjitionPanel.TryGetComponent(out RectTransform _rect);

        await _rect.DOAnchorPos(Vector3.zero, 0.8f).ToUniTask();

        Sceneloader.Instance.LoadScene("StageSelectScene");
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
