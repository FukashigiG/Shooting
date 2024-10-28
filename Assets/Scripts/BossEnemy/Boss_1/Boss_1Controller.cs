using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Cinemachine;
using System.Threading;

public class Boss_1Controller : Base_BossController
{
    [SerializeField] GameObject bullet_A01;
    [SerializeField] GameObject Rock_A03;
    [SerializeField] GameObject onigiri_A03;

    [SerializeField] GameObject effect_HitToPlayer;
    [SerializeField] GameObject effect_Impuct;
    [SerializeField] GameObject effect_HyperMode;

    [SerializeField] AudioClip SE_ShotBullet;
    [SerializeField] AudioClip SE_Impact;
    [SerializeField] AudioClip SE_RockDrop;
    [SerializeField] AudioClip SE_Hit;

    Vector3 defScale = new Vector3();

    bool flag_HPUnder50;
    bool isinAttack03 = false;
    bool onStage = true;

    [SerializeField] LayerMask wallLayer;

    float def_CoolTime_AfterAttack = 3;
    float A01_Interval_Shot = 0.4f;
    int A01_Num_Shot = 3;
    int A02_Num_Shot = 10;
    int A03_Num_Rock = 4;

    async protected override void Start()
    {
        base.Start();

        defScale = transform.localScale;

        await UniTask.Delay(TimeSpan.FromSeconds(1.2f));

        Attack01(_cancellationToken).Forget();
    }

    void DrawingMove()
    {
        if(flag_HPUnder50 == false && status.HP <= status.MaxHP * 0.5f)
        {
            Attack04(_cancellationToken).Forget();
            flag_HPUnder50 = true;
            return;
        }

        int x = UnityEngine.Random.Range(1, 4);

        switch (x)
        {
            case 1:

                Attack01(_cancellationToken).Forget();
                return;

            case 2:
                Attack02(_cancellationToken).Forget();
                return;

            case 3: 
                Attack03(_cancellationToken).Forget();
                return;
        }
    }

    async UniTask Attack01(CancellationToken token)
    {
        for (int i = 0; i < A01_Num_Shot; i++)
        {
            if(i == 0 || flag_HPUnder50) await transform.DORotate(new Vector3(0, 0, PlayerDirection()), 0.2f).ToUniTask(cancellationToken: token);

            Instantiate(bullet_A01, transform.position, transform.rotation);
            audioSource.PlayOneShot(SE_ShotBullet);

            await transform.DOPunchPosition(-transform.up, A01_Interval_Shot, 1, 1f).ToUniTask(cancellationToken: token);
        }

        await UniTask.Delay(TimeSpan.FromSeconds(def_CoolTime_AfterAttack * 0.4), cancellationToken: token);

        DrawingMove();
    }

    async UniTask Attack02(CancellationToken token)
    {
        collider2d.enabled = false;

        transform.DOMoveY(transform.position.y + 0.7f, 0.35f).ToUniTask(cancellationToken: token).Forget();

        await transform.DOScale(Vector3.zero, 0.35f).ToUniTask(cancellationToken: token);

        await UniTask.Delay(TimeSpan.FromSeconds(0.75f), cancellationToken: token);

        transform.position = -transform.position;

        transform.rotation = Quaternion.Euler(0, 0, PlayerDirection());

        await transform.DOScale(defScale, 0.35f).ToUniTask(cancellationToken: token);

        collider2d.enabled = true;

        for(int i = 0;i < A02_Num_Shot; i++)
        {
            Instantiate(bullet_A01, transform.position, transform.rotation);
            audioSource.PlayOneShot(SE_ShotBullet);

            await transform.DOPunchPosition(-transform.up, 0.1f, 1, 1).ToUniTask(cancellationToken: token);

            float x = 1;

            if(Mathf.Abs(PlayerDirection() - Mathf.Repeat(transform.localEulerAngles.z, 360)) > 10f)
            {
                if (PlayerDirection() >= Mathf.Repeat(transform.localEulerAngles.z, 360))
                {
                    if (PlayerDirection() - Mathf.Repeat(transform.localEulerAngles.z, 360) >= 180)
                    {
                        x = -1;
                    }
                    else
                    {
                        x = 1;
                    }
                }
                else
                {
                    if (Mathf.Repeat(transform.localEulerAngles.z, 360) - PlayerDirection() >= 180)
                    {
                        x = 1;
                    }
                    else
                    {
                        x = -1;
                    }
                }

                await transform.DORotate(new Vector3(0, 0, transform.localEulerAngles.z + 10 * x), 0.1f).SetEase(Ease.Linear).ToUniTask(cancellationToken: token);
            }
            else
            {
                await transform.DORotate(new Vector3(0, 0, PlayerDirection()), 0.1f).SetEase(Ease.Linear).ToUniTask(cancellationToken: token);
            }
        }

        await UniTask.Delay(TimeSpan.FromSeconds(def_CoolTime_AfterAttack), cancellationToken: token);

        DrawingMove();
    }

    async UniTask Attack03(CancellationToken token)
    {
        SetCameraState(CameraStateEnum.wide);

        await transform.DORotate(new Vector3(0, 0, PlayerDirection()), 0.8f).ToUniTask(cancellationToken: token);

        await transform.DOMove(transform.position - transform.up * 0.5f, 1.2f).ToUniTask(cancellationToken: token);

        float mylage = 40;

        isinAttack03 = true;
        onStage = true;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, mylage, wallLayer);

        if (hit.collider)
        {
            float length = Vector2.Distance(hit.point, transform.position);

            await transform.DOMove(hit.point + (Vector2)transform.up * 5, 0.7f).SetEase(Ease.Linear).ToUniTask(cancellationToken: token);

            transform.position = -1 * transform.position;
            transform.rotation = Quaternion.Euler(0, 0, PlayerDirection());

            onStage = false;

            await transform.DOMove(player.transform.position, 0.7f).SetEase(Ease.OutCubic).ToUniTask(cancellationToken: token);
        }

        onStage = true;
        isinAttack03 = false;

        SetCameraState(CameraStateEnum.def);

        await UniTask.Delay(TimeSpan.FromSeconds(def_CoolTime_AfterAttack), cancellationToken: token);

        DrawingMove();
    }

    async UniTask Attack04(CancellationToken token)
    {
        for (int p = 0; p < 4; p++)
        {
            await transform.DORotate(Vector3.zero, 0.1f).ToUniTask(cancellationToken: token);

            transform.DOMoveY(transform.position.y - 1.75f, 0.3f).ToUniTask(cancellationToken: token).Forget();
            await transform.DOScaleY(defScale.y * 0.35f, 0.3f).ToUniTask(cancellationToken: token);

            transform.DOScale(new Vector2(defScale.x * 0.2f, defScale.y), 0.1f).ToUniTask(cancellationToken: token).Forget();
            await transform.DOMoveY(player.transform.position.y + 10, 0.4f).ToUniTask(cancellationToken: token);

            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: token);

            transform.position = new Vector3(player.transform.position.x, transform.position.y, 0);

            transform.localScale = defScale;

            transform.rotation = Quaternion.Euler(0, 0, 180);

            await UniTask.Delay(TimeSpan.FromSeconds(0.3f), cancellationToken: token);

            await transform.DOMoveY(player.transform.position.y, 0.3f).SetEase(Ease.Linear).ToUniTask(cancellationToken: token);

            Instantiate(effect_Impuct, (Vector2)transform.position - Vector2.up * 1.5f, Quaternion.identity);
            impulseSource.GenerateImpulse();
            audioSource.PlayOneShot(SE_Impact);
            audioSource.PlayOneShot(SE_RockDrop);


            GameObject rock;
            float randomScale;
            Rigidbody2D rb_Rock;

            GameObject obj;

            for (int i = 0; i < 8; i++)
            {
                obj = Rock_A03;
                if (UnityEngine.Random.Range(0f, 100f) <= 5.5f) obj = onigiri_A03;

                rock = Instantiate(obj, transform.position, Quaternion.identity);

                randomScale = UnityEngine.Random.Range(0.4f, 1.0f);

                rock.transform.localScale = new Vector3(randomScale, randomScale, 1);
                rb_Rock = rock.GetComponent<Rigidbody2D>();
                rb_Rock.velocity = Vector2.up * 10 + new Vector2(UnityEngine.Random.Range(-4, 4), UnityEngine.Random.Range(-2, 4));
            }

            for (int i = 0; i < 4; i++)
            {
                rock = Instantiate(Rock_A03, new Vector2(UnityEngine.Random.Range(-8f, 8f), transform.position.y + 10 + UnityEngine.Random.Range(0f, 5f)), Quaternion.identity);

                randomScale = UnityEngine.Random.Range(0.4f, 1.0f);

                rock.transform.localScale = new Vector3(randomScale, randomScale, 1);
            }

            if (p == 3) Instantiate(effect_HyperMode, transform.position, Quaternion.identity, this.transform);
        }

        transform.DOMoveY(transform.position.y - 1.75f, 0.05f).ToUniTask(cancellationToken: token).Forget();
        await transform.DOScale(new Vector2(defScale.x, defScale.y * 0.35f), 0.05f).ToUniTask(cancellationToken: token);

        transform.DOMoveY(transform.position.y + 1.75f, 1f).ToUniTask(cancellationToken: token).Forget();
        await transform.DOScale(defScale, 1f).ToUniTask(cancellationToken: token);

        await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);

        BeHyperMode(_cancellationToken);
    }

    async void BeHyperMode(CancellationToken token)
    {
        def_CoolTime_AfterAttack = 2.25f;
        A01_Interval_Shot = 0.25f;
        A01_Num_Shot = 4;
        A02_Num_Shot = 12;
        A03_Num_Rock = 6;

        await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);

        DrawingMove();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isinAttack03)
        {
            if (collision.CompareTag("Wall"))
            {
                GameObject rock;
                Vector2 randomScale;
                Rigidbody2D rb_Rock;

                impulseSource.GenerateImpulse();

                audioSource.PlayOneShot(SE_Impact);
                audioSource.PlayOneShot(SE_RockDrop);

                GameObject obj;

                for (int i = 0; i < A03_Num_Rock; i++)
                {
                    obj = Rock_A03;
                    if (UnityEngine.Random.Range(0f, 100f) <= 5.5f) obj = onigiri_A03;

                    rock = Instantiate(obj, transform.position, Quaternion.identity);

                    randomScale = rock.transform.localScale * UnityEngine.Random.Range(0.4f, 1.0f);

                    int x = 1;
                    if (onStage) x = -1;

                    rock.transform.localScale = randomScale;
                    rb_Rock = rock.GetComponent<Rigidbody2D>();
                    rb_Rock.velocity = (Vector2)transform.up * x * 10 + new Vector2(UnityEngine.Random.Range(-4, 4), UnityEngine.Random.Range(-4, 4));
                }

            }

            if (collision.CompareTag("Player"))
            {
                collision.TryGetComponent(out IDamagable ID);
                if(ID.Damage(20f, transform.position, false))
                {
                    Instantiate(effect_HitToPlayer, Vector2.Lerp(transform.position, collision.transform.position, 0.5f), Quaternion.identity);
                    audioSource.PlayOneShot(SE_Hit);
                }
            }
        }
    }
}
