using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

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

        txt_remainingHP.text = "ボスの残りHP - " + GameDirector.remainingHP_Boss * 100 + "％";
    }

    public void GoToTitle()
    {
        SceneManager.LoadScene("StartScene");
    }

    public void Retry()
    {
        SceneManager.LoadScene("MainScene");
    }
}
