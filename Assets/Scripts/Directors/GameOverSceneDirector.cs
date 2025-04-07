using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameOverSceneDirector : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI txt_remainingHP;
    [SerializeField] GameObject fade;

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

    public void Retry()
    {
        Sceneloader.Instance.LoadScene("MainScene");
    }
}
