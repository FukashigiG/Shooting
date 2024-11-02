using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class UfoController : Base_BossController
{
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject Impuct_bomb_forPlayer;
    [SerializeField] GameObject Impuct_bomb_forEnemy;
    [SerializeField] float time_Delay_fall;
    [SerializeField] float distance_Move;
    [SerializeField] float coolTime_Move;
    [SerializeField] AudioClip SE_Bomb;
    [SerializeField] LayerMask wallLayer;

    TrailRenderer _trailRenderer;

    protected override void Start()
    {
        base.Start();

        TryGetComponent(out _trailRenderer);
    }

    async public UniTask falling(Vector2 tergetPosi)
    {
        var token = cancellationTokenSource.Token;

        await UniTask.Delay(100, cancellationToken: token);

        float x = time_Delay_fall * 1000;
        float y = _trailRenderer.time * 1000;

        await UniTask.Delay((int) x, cancellationToken: token);

        await transform.DOMove(tergetPosi, 0.6f).SetEase(Ease.Linear).ToUniTask(cancellationToken: token);

        Instantiate(Impuct_bomb_forPlayer, transform.position, Quaternion.identity);

        audioSource.PlayOneShot(SE_Bomb);

        await UniTask.Delay((int)y, cancellationToken: token);

        _trailRenderer.enabled = false;

        Moving(_cancellationToken).Forget();
    }

    async UniTask Moving(CancellationToken token)
    {
        while (true)
        {
            await transform.DORotate(new Vector3(0, 0, PlayerDirection()), 0.5f).ToUniTask(cancellationToken: token);

            float x = Vector2.Distance(transform.position, player.transform.position);

            switch (x)
            {
                case > 3:
                    await transform.DOMove(transform.position + transform.up * distance_Move, 1f).ToUniTask(cancellationToken: token);
                    break;

                default:

                    RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, distance_Move, wallLayer);
                    if(! hit)
                    {
                        await transform.DOMove(transform.position + (transform.right * distance_Move), 1f).ToUniTask(cancellationToken: token);
                        break;
                    }
                    else
                    {
                        hit = Physics2D.Raycast(transform.position, transform.right * -1f, distance_Move, wallLayer);
                        if (! hit)
                        {
                            await transform.DOMove(transform.position + (transform.right * distance_Move * -1f), 1f).ToUniTask(cancellationToken: token);
                            break;
                        }
                        else
                        {
                            await transform.DOMove(transform.position + transform.up * distance_Move, 1f).ToUniTask(cancellationToken: token);
                            break;
                        }
                    }

                    
            }

            

            await transform.DORotate(new Vector3(0, 0, PlayerDirection()), 0.5f).ToUniTask(cancellationToken: token);

            Instantiate(bullet, transform.position, Quaternion.Euler(0, 0, transform.localEulerAngles.z + UnityEngine.Random.Range(-5f, 5f)));
            await transform.DOShakePosition(0.1f, 0.5f, 15).ToUniTask(cancellationToken: token);

            await UniTask.Delay(TimeSpan.FromSeconds(coolTime_Move) , cancellationToken: token);
        }
    }

    protected override void StopAction()
    {
        base.StopAction();

        Instantiate(Impuct_bomb_forEnemy, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
