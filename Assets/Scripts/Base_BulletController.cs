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

    protected virtual void Start()
    {
        TryGetComponent(out trailRenderer);
        TryGetComponent(out collider2d);
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
        if (! collision.TryGetComponent(out IDamagable ID)) return;
        if (! ID.Damage(power, transform.position, JudgeCritical())) return;

        Hit(collision.gameObject);
    }

    public void Hit(GameObject terget)
    {
        if(hitEffect != null) Instantiate(hitEffect, transform.position, Quaternion.identity);
        if(SE_Hit != null) AudioSource.PlayClipAtPoint(SE_Hit, (Vector2)transform.position);

        BeSmallAndDie().Forget();
    }

    protected virtual async UniTask BeSmallAndDie()
    {
        var token = this.GetCancellationTokenOnDestroy();

        speed = 0;

        collider2d.enabled = false;

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
}
