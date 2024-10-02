using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class Sceneloader : MonoBehaviour
{
    [SerializeField] GameObject fadeOutKun;

    public async void GoToTitleScene()
    {
        Time.timeScale = 1f;

        await SceneManager.LoadSceneAsync("StartScene");
    }

    public async void GoToGamaOverScene()
    {
        Image image = fadeOutKun.GetComponent<Image>();
        image.color = new Color(0, 0, 0, 0);
        await image.DOFade(1f, 2.5f * Time.timeScale).ToUniTask();

        Time.timeScale = 1f;

        await SceneManager.LoadSceneAsync("GameOverScene");
    }

    public async void GoToGameClearScene()
    {
        Image image = fadeOutKun.GetComponent<Image>();
        image.color = new Color(1f, 1f, 1f, 0);
        await image.DOFade(1f, 3.5f * Time.timeScale).ToUniTask();

        Time.timeScale = 1f;

        await SceneManager.LoadSceneAsync("ClearScene");
    }
}
