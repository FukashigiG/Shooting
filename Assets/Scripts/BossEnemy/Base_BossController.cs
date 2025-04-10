using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UniRx;

public class Base_BossController : MonoBehaviour, IObserver<GameObject>
{
    private IDisposable _disposable;

    protected GameObject player;

    [field: SerializeField] public MobStatus status { get; protected set; }
    protected CinemachineImpulseSource impulseSource;
    protected Collider2D collider2d;
    protected AudioSource audioSource;

    readonly protected CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    protected CancellationToken _cancellationToken;

    bool isStopped = false;

    public enum CameraStateEnum
    {
        def, followOnlyPlayer, wide
    }


    //↓CameraStateEnum型の変数っぽく振る舞い、こいつの中身が変わると購読先に通知が行くよ、すごい！
    private readonly ReactiveProperty<CameraStateEnum> _camStateProp;
    public IReadOnlyReactiveProperty<CameraStateEnum> CamStateProp => _camStateProp;
    public CameraStateEnum CameraStateValue => _camStateProp.Value;


    //コンストラクタ
    //初期化処理的な感じかな？
    public Base_BossController()
    {
        _camStateProp = new ReactiveProperty<CameraStateEnum>(CameraStateEnum.def);
    }

    protected virtual void Start()
    {
        player = GameObject.Find("Player_Core");

        TryGetComponent(out impulseSource);
        TryGetComponent(out collider2d);
        TryGetComponent(out audioSource);

        _cancellationToken = cancellationTokenSource.Token;

        _disposable = status.Subscribe(this).AddTo(this);
        GameDirector.Instance.onFinish.AddListener(StopAction);
    }

    //このメソッドを実行してカメラの状態を変更しよう
    protected void SetCameraState(CameraStateEnum stateEnum)
    {
        _camStateProp.Value = stateEnum;
    }

    protected float PlayerDirection()
    {
        float x = (Mathf.Atan2(transform.position.y - player.transform.position.y, transform.position.x - player.transform.position.x) * Mathf.Rad2Deg) + 90;

        x = Mathf.Repeat(x, 360);

        return x;
    }

    protected virtual void WhenDie()
    {
        StopAction();
    }


    protected virtual void StopAction()
    {
        if (_cancellationToken == null) return;
        if(isStopped) return;

        isStopped = true;
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
    }


    //以下コールバック
    public void OnCompleted()
    {

    }

    public void OnError(Exception error)
    {
        Debug.LogError(error);
    }

    public void OnNext(GameObject obj)
    {
        WhenDie();
    }
}
