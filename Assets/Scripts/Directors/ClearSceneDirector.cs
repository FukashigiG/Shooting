using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class ClearSceneDirector : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI txt_ClearTime;
    [SerializeField] TextMeshProUGUI txt_Weapon;
    [SerializeField] TextMeshProUGUI txt_Stage;

    [SerializeField] GameObject fadePanel;

    private void Start()
    {
        txt_Weapon.text = "クリアステージ：ザ・ボス";

        txt_ClearTime.text = "クリアタイム：" + GameDirector.elapsedTime + " 秒";

        txt_Weapon.text = "使用ウェポン : シューター";

        fadePanel.SetActive(true);
        Image image = fadePanel.GetComponent<Image>();
        image.color = new Color(1f, 1f, 1f, 1f);
        image.DOFade(0f, 1.5f).OnComplete(() => fadePanel.SetActive(false));
    }

    public void GoToStartScene()
    {
        Sceneloader.Instance.LoadScene("StartScene");
    }
}
