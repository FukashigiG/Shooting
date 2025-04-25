using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using DG.Tweening;

public class LastBossCtrl : Base_BossController
{
    [SerializeField] GameObject exprodeBullet;
    [SerializeField] GameObject chaseBullet;
    [SerializeField] GameObject pullBullet;
    [SerializeField] GameObject blockBomb;

    int Num_Bullet_A01 = 6;
    int Num_Bullet_A02 = 3;

    protected override void Start()
    {
        base.Start();

        var token = this.GetCancellationTokenOnDestroy();

        AttackTask(token).Forget();
    }

    async UniTask AttackTask(CancellationToken token)
    {
        int x;

        while(true)
        {
            x = Random.Range(0, 4);

            switch (x)
            {
                case 0:
                    await Attack01(token);
                    break;

                case 1:
                    await Attack02(token);
                    break;

                case 2:
                    await Attack03(token);
                    break;

                case 3:
                    await Attack04(token);
                    break;

                default:
                    await Attack01(token);
                    break;
            }

            await UniTask.Delay(1000, cancellationToken:token);
        }

        
    }

    async UniTask Attack01(CancellationToken token)
    {
        await transform.DORotate(new Vector3(0, 0, PlayerDirection()), 0.2f).ToUniTask(cancellationToken:  token);
        await transform.DOMove(transform.position + transform.up * -1f, 1f).ToUniTask(cancellationToken:  token);
        await transform.DOMove(transform.position + transform.up * 8f, 0.6f).ToUniTask(cancellationToken:  token);

        for(int i = 0; i < Num_Bullet_A01; i++)
        {
            Instantiate(exprodeBullet, transform.position + transform.up * 0.6f, transform.rotation);

            await transform.DORotate(new Vector3(0, 0, transform.localEulerAngles.z + 360 / Num_Bullet_A01), 0.05f).SetEase(Ease.Linear).ToUniTask(cancellationToken: token);
        }
    }

    async UniTask Attack02(CancellationToken token)
    {
        await transform.DORotate(new Vector3(0, 0, PlayerDirection()), 0.8f).ToUniTask(cancellationToken: token);

        for (int i = 0;i < Num_Bullet_A02; i++)
        {
            GameObject bullet = Instantiate(chaseBullet, transform.position + transform.up, transform.rotation);
            bullet.GetComponent<LandMineBulletCtrl>().SetTerget(player);

            await transform.DOPunchPosition(-transform.up, 1.2f, 1, 1f).ToUniTask(cancellationToken: token);
        }
    }

    async UniTask Attack03(CancellationToken token)
    {
        await transform.DORotate(new Vector3(0, 0, PlayerDirection()), 0.8f).ToUniTask(cancellationToken: token);

        Instantiate(pullBullet, transform.position + transform.up, transform.rotation);

        await transform.DOPunchPosition(-transform.up, 1.2f, 1, 1f).ToUniTask(cancellationToken: token);
    }

    async UniTask Attack04(CancellationToken token)
    {
        await transform.DORotate(new Vector3(0, 0, PlayerDirection()), 0.8f).ToUniTask(cancellationToken: token);

        Instantiate(blockBomb, transform.position + transform.up * 2, transform.rotation);
    }
}
