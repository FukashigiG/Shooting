using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using DG.Tweening;
using System.Threading;

[RequireComponent (typeof(Collider2D))]
public class Base_BulletController : MonoBehaviour, Projectile
{
    [SerializeField] protected float speed;

    [SerializeField] protected float power;

    [SerializeField] protected float chance_Critical;

    [SerializeField] protected GameObject hitEffect;

    [SerializeField]  protected AudioClip SE_Hit;
 
    protected TrailRenderer trailRenderer;

    protected Collider2D collider2d;

    protected bool isHittableToWall = false;

    readonly protected CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    protected CancellationToken _cancellationToken;

    protected virtual void Start()
    {
        TryGetComponent(out trailRenderer);
        TryGetComponent(out collider2d);

        _cancellationToken = cancellationTokenSource.Token;

        BeHittableToWall();
    }

    protected virtual void Update()
    {
        MoveOn();
    }

    protected virtual void MoveOn()
    {
        transform.Translate(0, speed * Time.deltaTime, 0);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall") && isHittableToWall == false) return;

        if (! collision.TryGetComponent(out IDamagable ID)) return;
        if (! ID.Damage(power, transform.position, JudgeCritical())) return;

        Hit(collision.gameObject);
    }

    public void Hit(GameObject terget)
    {
        Instantiate(hitEffect, transform.position, Quaternion.identity);
        AudioSource.PlayClipAtPoint(SE_Hit, (Vector2)transform.position);

        speed = 0;

        collider2d.enabled = false;

        BeSmallAndDie(_cancellationToken).Forget();
    }

    protected async UniTask BeSmallAndDie(CancellationToken token)
    {
        float x;

        if(trailRenderer == null)
        {
            x = 0.1f;
        } else
        {
            x = trailRenderer.time;
        }

        await transform.DOScale(Vector2.zero, x).ToUniTask(cancellationToken: token);

        Destroy(gameObject);
    }

    protected bool JudgeCritical()
    {
        bool x = false;

        float y = UnityEngine.Random.Range(0, 100);

        if(chance_Critical >= y) x = true;

        return x;
    }

    protected async void BeHittableToWall()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.05f));

        isHittableToWall = true;
    }

    protected void OnDestroy()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
    }
}
