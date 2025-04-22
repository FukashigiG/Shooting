using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using Cinemachine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;
using System.Threading;

public class GameDirector : SingletonMono<GameDirector>, IObserver<Unit>
{
    [SerializeField] GameObject[] bossEnemy;
    [SerializeField] HPGaugeController HPBar_Boss;

    GameObject theBoss;
    MobStatus bossStatus;
    Base_BossController _bossController;

    [SerializeField] GameObject player;
    [SerializeField] HPGaugeController HPBar_Player;
    MobStatus playerStatus;

    [SerializeField] GameObject panel_Pause;

    public Subject<Unit> finish { get; private set; } = new Subject<Unit>();

    CinemachineImpulseSource impulseSource;

    [SerializeField] CinemachineTargetGroup targetGroup;

    public static float remainingHP_Boss {  get; private set; }

    [SerializeField] Sprite[] icon_Player;
    [SerializeField] Image[] image_UI = new Image[2];

    [SerializeField] GameObject panel_GameOver;


    [SerializeField] GameObject tranjitionPanel;

    float startTime;
    public static float elapsedTime {  get; private set; } = 0;

    public enum GameStateEnum
    {
        onGame,pause, finish
    }
    public GameStateEnum gamestateEnum;

    readonly  CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    CancellationToken _cancellationToken;


    void Awake()
    {
        int theID = StageInfoHolder.Instance.stageID;

        switch (theID)
        {
            case 0:
                theBoss = Instantiate(bossEnemy[0], new Vector3(0, 3, 0), Quaternion.Euler(0, 0, 180));
                break;

            case 1:
                theBoss = Instantiate(bossEnemy[1], new Vector3(0, 3, 0), Quaternion.Euler(0, 0, 180));
                break;

            case 2:
                theBoss = Instantiate(bossEnemy[2], new Vector3(0, 3, 0), Quaternion.Euler(0, 0, 180));
                break;

            default:
                theBoss = Instantiate(bossEnemy[0], new Vector3(0, 3, 0), Quaternion.Euler(0, 0, 180));
                break;
        }

        theBoss.TryGetComponent(out _bossController);
        bossStatus = _bossController.status;
        bossStatus.died
            .Subscribe(x => ClearGame())
            .AddTo(this);

        player.TryGetComponent(out playerStatus);
        playerStatus.died
            .Subscribe(x => PlayerDied().Forget())
            .AddTo(this);

        gamestateEnum = GameStateEnum.onGame;

        _cancellationToken = cancellationTokenSource.Token;
    }

    void Start()
    {
        player.GetComponent<PlayerController>().ActivateWeapon(StageInfoHolder.Instance.weaponID);

        TryGetComponent(out impulseSource);

        image_UI[0].overrideSprite = icon_Player[0];
        image_UI[1].overrideSprite = icon_Player[0];

        HPBar_Boss.Set(bossStatus);
        HPBar_Player.Set(playerStatus);

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

    void OnPause()
    {
        if (gamestateEnum != GameStateEnum.onGame) return;

        Time.timeScale *= 0.05f;

        panel_Pause.SetActive(true);
    }

    public void ReleasePause()
    {
        if (gamestateEnum != GameStateEnum.pause) return;

        Time.timeScale /= 0.05f;

        panel_Pause.SetActive(false);
    }

    async UniTask PlayerDied()
    {
        if (gamestateEnum != GameStateEnum.onGame) return;

        impulseSource.GenerateImpulse();

        remainingHP_Boss = bossStatus.HP / bossStatus.MaxHP;

        Time.timeScale *= 0.4f;

        gamestateEnum = GameStateEnum.finish;

        finish.OnNext(Unit.Default);

        panel_GameOver.SetActive(true);

        await UniTask.Delay(1400, cancellationToken: _cancellationToken);

        Sceneloader.Instance.LoadScene("GameOverScene");
    }


    void ClearGame()
    {
        if (gamestateEnum != GameStateEnum.onGame) return;

        impulseSource.GenerateImpulse();

        elapsedTime = Time.time - startTime;

        Time.timeScale *= 0.4f;

        gamestateEnum = GameStateEnum.finish;

        finish.OnNext(Unit.Default);

        Sceneloader.Instance.LoadScene("ClearScene");
    }

    public void Retire()
    {
        if (gamestateEnum != GameStateEnum.onGame) return;

        gamestateEnum = GameStateEnum.finish;

        finish.OnNext(Unit.Default);

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

    //以下オブザーバーのコールバック

    public void OnCompleted()
    {

    }

    public void OnError(Exception error)
    {
        Debug.LogError(error);
    }

    public void OnNext(Unit x)
    {
    }
}
