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

public class GameDirector : SingletonMono<GameDirector>, IObserver<GameObject>
{
    List<IDisposable> _disposable = new List<IDisposable>();

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

    public static float remainingHP_Boss {  get; private set; }

    [SerializeField] Sprite[] icon_Player;
    [SerializeField] Image[] image_UI = new Image[2];

    [SerializeField] GameObject tranjitionPanel;

    float startTime;
    public static float elapsedTime {  get; private set; } = 0;

    public bool onGame { get; private set; } = true;

    void Start()
    {
        TryGetComponent(out impulseSource);

        theBoss = Instantiate(bossEnemy[0], new Vector3(0, 3, 0), Quaternion.Euler(0, 0, 180));

        image_UI[0].overrideSprite = icon_Player[0];
        image_UI[1].overrideSprite = icon_Player[0];

        theBoss.TryGetComponent(out _bossController);
        bossStatus = _bossController.status;
        //bossStatus.SetHPGauge(HPBar_Boss);
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

        Cam_Director.Instance.SetCam_Def();

        startTime = Time.time;
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

        elapsedTime = Time.time - startTime;

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
                Cam_Director.Instance.SetCam_Def();
                break;

            case Base_BossController.CameraStateEnum.followOnlyPlayer:
                Cam_Director.Instance.SetCam_FollowPlayer();
                break;

            case Base_BossController.CameraStateEnum.wide:
                Cam_Director.Instance.SetCam_Wide();
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
