using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class GameOverSceneDirector : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI txt_remainingHP;
    [SerializeField] GameObject fade;
    [SerializeField] GameObject transition_Out;

    [SerializeField] Button button_ReTry;
    [SerializeField] Button button_GoTitle;

    private void Awake()
    {
        button_ReTry.onClick.AddListener(() => Retry().Forget());
        button_GoTitle.onClick.AddListener(GoToTitle);
    }

    private void Start()
    {
        slider.value = GameDirector.remainingHP_Boss;

        fade.SetActive(true);
        Image image = fade.GetComponent<Image>();
        image.DOFade(0, 1f);

        txt_remainingHP.text = "É{ÉXÇÃécÇËHP - " + GameDirector.remainingHP_Boss * 100 + "Åì";
    }

    public void GoToTitle()
    {
        Sceneloader.Instance.LoadScene("StartScene");
    }

    async UniTask Retry()
    {
        transition_Out.SetActive(true);

        await UniTask.Delay(1000);

        Sceneloader.Instance.LoadScene("MainScene");
    }
}
