using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;
using Cinemachine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;

public class GameDirector : MonoBehaviour, IObserver<GameObject>
{
    List<IDisposable> _disposable = new List<IDisposable>();

    float randomAngle;

    [SerializeField] GameObject[] bossEnemy;
    [SerializeField] HPGaugeController HPBar_Boss;

    GameObject theBoss;
    MobStatus bossStatus;
    Base_BossController _bossController;

    [SerializeField] GameObject player;
    MobStatus playerStatus;

    [NonSerialized] public UnityEvent onFinish = new UnityEvent();

    CinemachineImpulseSource impulseSource;

    [SerializeField] CinemachineTargetGroup targetGroup;

    [SerializeField] Cam_Director _camDirector;
    public static float remainingHP_Boss {  get; private set; }

    [SerializeField] Sprite[] icon_Player;
    [SerializeField] Image[] image_UI = new Image[2];

    [SerializeField] GameObject tranjitionPanel;

    public static float elapsedTime {  get; private set; } = 0;

    bool onGame = true;

    void Start()
    {
        TryGetComponent(out impulseSource);

        switch (StartSceneDirector.stage)
        {
            case StartSceneDirector.stageEnum.stage0:

                theBoss = Instantiate(bossEnemy[0], new Vector3(0, 3, 0), Quaternion.Euler(0, 0, 180));

                break;

            case StartSceneDirector.stageEnum.stage1:

                theBoss = Instantiate(bossEnemy[1], new Vector3(0, 3, 0), Quaternion.Euler(0, 0, 180));

                break;

            case StartSceneDirector.stageEnum.stage2:

                theBoss = Instantiate(bossEnemy[2], new Vector3(0, 3, 0), Quaternion.Euler(0, 0, 180));

                break;
        }

        switch (StartSceneDirector.weapon)
        {
            case StartSceneDirector.weaponEnum.weapon0:

                image_UI[0].overrideSprite = icon_Player[0];
                image_UI[1].overrideSprite = icon_Player[0];
                break;

            case StartSceneDirector.weaponEnum.weapon1:

                image_UI[0].overrideSprite = icon_Player[1];
                image_UI[1].overrideSprite = icon_Player[1];
                break;

            case StartSceneDirector.weaponEnum.weapon2:

                image_UI[0].overrideSprite = icon_Player[2];
                image_UI[1].overrideSprite = icon_Player[2];

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

        _bossController.CamStateProp
            .Subscribe(SetCamera)
            .AddTo(gameObject);

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

        Sceneloader.Instance.LoadScene("GameOverScene");
    }


    void ClearGame()
    {
        if (!onGame) return;

        impulseSource.GenerateImpulse();

        Time.timeScale *= 0.4f;

        onGame = false;

        onFinish.Invoke();

        Sceneloader.Instance.LoadScene("ClearScene");
    }

    public void Retire()
    {
        if (! onGame) return;

        onGame = false;

        onFinish.Invoke();

        Sceneloader.Instance.LoadScene("StartScene");
    }

    void SetCamera(Base_BossController.CameraStateEnum _enum)
    {
        switch (_enum)
        {
            case Base_BossController.CameraStateEnum.def:
                _camDirector.SetCam_Def();
                break;

            case Base_BossController.CameraStateEnum.followOnlyPlayer:
                _camDirector.SetCam_FollowPlayer();
                break;

            case Base_BossController.CameraStateEnum.wide:
                _camDirector.SetCam_Wide();
                break;

            default:
                break;
        }
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
        Debug.LogError(error);
    }

    public void OnNext(GameObject obj)
    {

        if (obj == player)
        {
            PlayerDied();
        }
        else if(obj == theBoss)
        {
            ClearGame();
        }
    }
}
