using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


//jsonからデータを持ってくる際List型や配列型の変数じゃダメっぽいのでこれを用意してるよ
[Serializable] class StageDataColections
{
    public List<StageData> collections_S;
    public List<WeaponData> collections_W;
}

public class StageSelectDirector : SingletonMono<StageSelectDirector>
{
    StageDataColections stageDatas;

    [SerializeField] TextAsset jsonFile;

    [SerializeField] GameObject rightWindow;
    RectTransform windowRect;
    [SerializeField] Text txt_Title;
    [SerializeField] Text txt_Description;

    [SerializeField] Transform parent_selectButtons;

    int cullentPointingStageID = 0;
    int cullentPointingWeaponID = 0;
    GameObject pointingStageButtonObj;

    [SerializeField] Button button_LoadBattleScene;
    [SerializeField] Button button_GoBack;

    [SerializeField] GameObject window_WeaponSelect;
    [SerializeField] GameObject transitionPanel;

    [SerializeField] GameObject IDHolder;

    [SerializeField] GameObject weaponWindow;

    [SerializeField] Button button_OpenWeaponWindow;



    private void Awake()
    {
        button_LoadBattleScene.onClick.AddListener(() => GoToMainScene().Forget());
        button_GoBack.onClick.AddListener(GoBackToStageSelect);

        button_OpenWeaponWindow.onClick.AddListener(OpenWeaponWindow);
    }

    private void Start()
    {
        string jsonStr = jsonFile.ToString();
        stageDatas = JsonUtility.FromJson<StageDataColections>(jsonStr);

        rightWindow.TryGetComponent(out windowRect);

        Transform buttonTransform;

        for (int i = 0; i < parent_selectButtons.childCount; i++)
        {
            buttonTransform = parent_selectButtons.GetChild(i);

            buttonTransform.TryGetComponent(out StageSelectButtonCtrler ctrler);

            ctrler.SetInfo(stageDatas.collections_S[i].ID, stageDatas.collections_S[i].stageName);
        }

    }

    public void DisplayRightWindow(int id, GameObject stageButton)
    {
        cullentPointingStageID = id;
        pointingStageButtonObj = stageButton;

        StageData _stageData = stageDatas.collections_S.Find(x => x.ID == id);

        txt_Title.text = _stageData.stageName;
        txt_Description.text = _stageData.description;

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

    async UniTask GoToMainScene()
    {
        if(StageInfoHolder.Instance != null)
        {
            Instantiate(IDHolder);

            Debug.Log("w");
        }

        StageInfoHolder.Instance.GetComponent<StageInfoHolder>().SetID(cullentPointingStageID, cullentPointingWeaponID);

        transitionPanel.SetActive(true);

        await UniTask.Delay(1400);

        Sceneloader.Instance.LoadScene("MainScene");
    }


    void OpenWeaponWindow()
    {
        weaponWindow.SetActive(true);

        EventSystem.current.SetSelectedGameObject(pointingStageButtonObj);
    }

    public void setWeapon(int x)
    {
        cullentPointingWeaponID = x;
    }
}
