using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.UI;
using DG.Tweening;

public class GameDirector : MonoBehaviour, IObserver<GameObject>
{
    List<IDisposable> _disposable = new List<IDisposable>();

    float randomAngle;

    [SerializeField] GameObject[] bossEnemy;
    [SerializeField] HPGaugeController HPBar_Boss;

    GameObject theBoss;
    MobStatus bossStatus;
    Base_BossController _bossController;

    Sceneloader sceneloader;

    [SerializeField] GameObject player;
    MobStatus playerStatus;

    [NonSerialized] public UnityEvent onFinish = new UnityEvent();

    CinemachineImpulseSource impulseSource;

    [SerializeField] CinemachineTargetGroup targetGroup;

    [SerializeField] Cam_Director _camDirector;
    public static float remainingHP_Boss {  get; private set; }

    [SerializeField] Sprite[] icon_Player;
    [SerializeField] Image image_UI;

    [SerializeField] GameObject tranjitionPanel;

    public static float elapsedTime {  get; private set; } = 0;

    bool onGame = true;

    void Start()
    {

        TryGetComponent(out impulseSource);
        TryGetComponent(out sceneloader);

        switch (StartSceneDirector.stage)
        {
            case StartSceneDirector.stageEnum.stage0:

                theBoss = Instantiate(bossEnemy[0], new Vector3(0, 3, 0), Quaternion.Euler(0, 0, 180));

                break;

            case StartSceneDirector.stageEnum.stage1:

                theBoss = Instantiate(bossEnemy[1], new Vector3(0, 3, 0), Quaternion.Euler(0, 0, 180));

                break;
        }

        switch (StartSceneDirector.weapon)
        {
            case StartSceneDirector.weaponEnum.weapon0:

                image_UI.overrideSprite = icon_Player[0];
                break;

            case StartSceneDirector.weaponEnum.weapon1:

                image_UI.overrideSprite = icon_Player[1];
                break;

            case StartSceneDirector.weaponEnum.weapon2:

                image_UI.overrideSprite = icon_Player[2];
                break;
        }

        theBoss.TryGetComponent(out _bossController);
        bossStatus = _bossController.status;
        bossStatus.SetHPGauge(HPBar_Boss);
        _disposable.Add(bossStatus.Subscribe(this));

        player.TryGetComponent(out playerStatus);
        _disposable.Add(playerStatus.Subscribe(this));

        tranjitionPanel.SetActive(true);
        tranjitionPanel.TryGetComponent(out RectTransform _rect);
        _rect.DOAnchorPos(new Vector2(0, -1200), 0.8f);

        targetGroup.m_Targets = new CinemachineTargetGroup.Target[]
        {
            new CinemachineTargetGroup.Target
            {
                target = player.transform,
                weight = 1,
                radius = 1.2f
            },

            new CinemachineTargetGroup.Target
            {
                target = theBoss.transform,
                weight = 1,
                radius = 1.2f
            }
        };

        _bossController.cam_BeDef.AddListener(() => _camDirector.SetCam_Def());
        _bossController.cam_BeOnlyPlayer.AddListener(() => _camDirector.SetCam_FollowPlayer());
        _bossController.cam_BeWide.AddListener(() => _camDirector.SetCam_Wide());

        _camDirector.SetCam_Def();
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
    }

    void PlayerDied()
    {
        if (!onGame) return;

        impulseSource.GenerateImpulse();

        remainingHP_Boss = bossStatus.HP / bossStatus.MaxHP;

        Time.timeScale *= 0.4f;

        onGame = false;

        onFinish.Invoke();

        _bossController.cam_BeDef.RemoveAllListeners();
        _bossController.cam_BeOnlyPlayer.RemoveAllListeners();
        _bossController.cam_BeWide.RemoveAllListeners();



        sceneloader.GoToGamaOverScene();
    }


    void ClearGame()
    {
        if (!onGame) return;

        impulseSource.GenerateImpulse();

        Time.timeScale *= 0.4f;

        onGame = false;

        onFinish.Invoke();

        sceneloader.GoToGameClearScene();
    }

    public void Retire()
    {
        if (! onGame) return;

        onGame = false;

        onFinish.Invoke();

        sceneloader.GoToTitleScene();
    }

    private void OnDisable()
    {
        foreach (var disposable in _disposable)
        {
            if (disposable != null) disposable.Dispose();
        }
    }

    //以下オブザーバーのコールバック

    public void OnCompleted()
    {

    }

    public void OnError(Exception error)
    {
        //Debug.LogError(error);
    }

    public void OnNext(GameObject obj)
    {
        PlayerDied();
    }
}
