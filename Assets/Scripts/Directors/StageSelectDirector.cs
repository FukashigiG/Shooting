using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable] class StageData
{
    public int ID;
    public string stageName;
}


//jsonからデータを持ってくる際List型や配列型の変数じゃダメっぽいのでこれを用意してるよ
[Serializable] class DataColections
{
    public List<StageData> collections;
}

public class StageSelectDirector : SingletonMono<StageSelectDirector>
{
    DataColections stageDatas;

    [SerializeField] TextAsset jsonFile;

    [SerializeField] GameObject rightWindow;
    RectTransform windowRect;
    [SerializeField] Text txt_Title;

    [SerializeField] Transform parent_selectButtons;

    int cullentPointingID = 0;
    GameObject pointingStageButtonObj;

    [SerializeField] Button button_LoadBattleScene;
    [SerializeField] Button button_GoBack;

    private void Start()
    {
        string jsonStr = jsonFile.ToString();
        stageDatas = JsonUtility.FromJson<DataColections>(jsonStr);

        rightWindow.TryGetComponent(out windowRect);

        Transform buttonTransform;

        for (int i = 0; i < parent_selectButtons.childCount; i++)
        {
            buttonTransform = parent_selectButtons.GetChild(i);

            buttonTransform.TryGetComponent(out StageSelectButtonCtrler ctrler);

            ctrler.SetInfo(stageDatas.collections[i].ID, stageDatas.collections[i].stageName);
        }

        button_LoadBattleScene.onClick.AddListener(GoToMainScene);
        button_GoBack.onClick.AddListener(GoBackToStageSelect);
    }

    public void DisplayRightWindow(int id, GameObject stageButton)
    {
        cullentPointingID = id;
        pointingStageButtonObj = stageButton;

        StageData _stageData = stageDatas.collections.Find(x => x.ID == id);

        txt_Title.text = _stageData.stageName;

        rightWindow.SetActive(true);

        windowRect.anchoredPosition = Vector3.zero;

        windowRect.DOAnchorPosX(-250, 0.5f);
    }

    public void ReadyToFight()
    {
        EventSystem.current.SetSelectedGameObject(button_LoadBattleScene.gameObject);
    }

    void GoBackToStageSelect()
    {
        EventSystem.current.SetSelectedGameObject(pointingStageButtonObj);
    }

    void GoToMainScene()
    {
        Sceneloader.Instance.LoadScene("MainScene");
    }

}
