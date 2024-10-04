using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Base_BossController : MonoBehaviour
{
    protected GameObject player;

    protected GameDirector _director;
    [field: SerializeField] public MobStatus status {  get; protected set; }
    protected CinemachineImpulseSource impulseSource;
    protected Collider2D collider2d;
    protected AudioSource audioSource;

    readonly protected CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    protected CancellationToken _cancellationToken;

    bool isStopped = false;

    protected virtual void Start()
    {
        player = GameObject.Find("Player_Core");

        _director = GameObject.Find("GameDirector").GetComponent<GameDirector>();

        TryGetComponent(out impulseSource);
        TryGetComponent(out collider2d);
        TryGetComponent(out audioSource);

        _cancellationToken = cancellationTokenSource.Token;

        status.onDie.AddListener(WhenDie);
        _director.onFinish.AddListener(StopAction);
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
}
