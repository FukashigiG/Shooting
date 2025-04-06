using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectDirector : SingletonMono<StageSelectDirector>
{
    [SerializeField] GameObject rightWindow;
    RectTransform windowRect;
    [SerializeField] Text txt_Title;

    private void Start()
    {
        rightWindow.TryGetComponent(out windowRect);
    }

    public void DisplayRightWindow(string title)
    {
        txt_Title.text = title;

        rightWindow.SetActive(true);

        windowRect.anchoredPosition = Vector3.zero;

        windowRect.DOAnchorPosX(-250, 0.5f);

        Debug.Log(Time.time);
    }
}
