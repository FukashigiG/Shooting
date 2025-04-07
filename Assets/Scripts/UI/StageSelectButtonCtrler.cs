using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StageSelectButtonCtrler : MonoBehaviour, ISelectHandler
{

    RectTransform _rectTransform;
    RectTransform _parent;

    Button _button;
    Text txt_TitleName;

    int pointingID;

    void Awake()
    {
        TryGetComponent(out _button);
        _button.onClick.AddListener(Clicked);

        txt_TitleName = transform.GetChild(0).GetComponent<Text>();

        TryGetComponent(out _rectTransform);
        _parent = this.transform.parent.GetComponent<RectTransform>();
    }

    public void SetInfo(int id, string name)
    {
        pointingID = id;

        txt_TitleName.text = name;
    }

    public void OnSelect(BaseEventData x)
    {
        StageSelectDirector.Instance.DisplayRightWindow(pointingID, this.gameObject);

        _parent.DOAnchorPosX(0 - (_rectTransform.localPosition.x - 410f) , 0.5f);
    }

    public void Clicked()
    {
        StageSelectDirector.Instance.ReadyToFight();
    }
}
