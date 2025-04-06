using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectButtonCtrler : MonoBehaviour
{
    [SerializeField] string stageName;

    Button _button;
    Text txt_TitleName;

    void Start()
    {
        TryGetComponent(out _button);
        _button.onClick.AddListener(Selected);

        txt_TitleName = transform.GetChild(0).GetComponent<Text>();
        txt_TitleName.text = stageName;
    }

    void Selected()
    {
        StageSelectDirector.Instance.DisplayRightWindow(stageName);
    }
}
