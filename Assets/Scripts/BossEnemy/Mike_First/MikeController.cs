using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using System.Threading;
using System;
using System.Linq;

public class MikeController : Base_BossController
{
    [SerializeField] GameObject bullet_Attack01;
    [SerializeField] int range_Bullet_Attack01;
    [SerializeField] GameObject effect_AnnounceUfo;
    [SerializeField] GameObject effect_Hyper;
    [SerializeField] GameObject ufo_Attack02;
    [SerializeField] float chance_GreenBullet;
    [SerializeField] GameObject rollingBullet_Attack03;
    [SerializeField] GameObject rollingGreenBullet_Attack03;
    [SerializeField] GameObject bullet_Attack04;
    [SerializeField] GameObject Impuct_Attack04;
    [SerializeField] AudioClip SE_HeroPunch;
    [SerializeField] GameObject bullet_SpeciallAttack;

    [SerializeField] AudioClip SE_A01;
    [SerializeField] AudioClip SE_A02;
    [SerializeField] AudioClip SE_A03;
    [SerializeField] AudioClip SE_A04_jump;
    [SerializeField] AudioClip SE_SA_Charge;
    [SerializeField] AudioClip SE_SA_Shot;

    Vector3 defScale = new Vector3();

    GameObject core;

    bool flag_HPUnder50;

    float def_CoolTime_AfterAttack = 3;
    int num_Shot_A01 = 3;
    int num_CallUFO_A02 = 3;
    int num_bullet_A03 = 20;

    int[] weight_Attack = new int[4];

    async protected override void Start()
    {
        base.Start();

        defScale = transform.localScale;

        core = transform.GetChild(0).gameObject;

        weight_Attack[0] = 30;
        weight_Attack[1] = 10;
        weight_Attack[2] = 30;
        weight_Attack[3] = 30;

        await UniTask.Delay(TimeSpan.FromSeconds(0.8f));

        Attack02(_cancellationToken).Forget();
    }

    void DrawAttack()
    {
        if (flag_HPUnder50 == false && status.HP <= status.MaxHP * 0.5f)
        {
            SpecialAttack(_cancellationToken).Forget();
            flag_HPUnder50 = true;
            return;
        }

        int totalWeight = weight_Attack.Sum();

        int x = UnityEngine.Random.Range(0, totalWeight + 1);

        for (int i = 0; i < weight_Attack.Length; i++)
        {
            if (x <= weight_Attack[i])
            {
                switch (i)
                {
                    case 0:

                        Attack01(_cancellationToken).Forget();
                        return;

                    case 1:

                        Attack02(_cancellationToken).Forget();
                        return;

                    case 2:

                        Attack03(_cancellationToken).Forget();
                        return;

                    case 3:

                        Attack04(_cancellationToken).Forget();
                        return;

                    default:

                        Attack01(_cancellationToken).Forget();
                        return;

                    
                }
            }

            x -= weight_Attack[i];
        }
    }

    async UniTask Attack01(CancellationToken token)
    {
        Bullet_MikeController bulletController;
        float ratio;
        float swipeDirection;

        await transform.DORotate(new Vector3(0, 0, PlayerDirection() - range_Bullet_Attack01 / 2), 0.2f).ToUniTask(cancellationToken: token);

        for (int qz = 1; qz <= num_Shot_A01; qz++)
        {
            GameObject[] bullets = new GameObject[5 + (qz - 1)];

            audioSource.PlayOneShot(SE_A01);

            if (qz % 2 == 1)
            {
                swipeDirection = 1;
            }
            else
            {
                swipeDirection = -1;
            }

            float startAngle = transform.localEulerAngles.z;

            for (int i = 0; i < bullets.Length; i++)
            {
                ratio = ((float)i + 1f) / ((float)bullets.Length + 1f);

                await transform.DORotate(new Vector3(0, 0, startAngle + range_Bullet_Attack01 * ratio * swipeDirection), 0.03f).SetEase(Ease.Linear).ToUniTask(cancellationToken: token);

                bullets[i] = Instantiate(bullet_Attack01, transform.position + transform.up * 2f, transform.rotation);

                bullets[i].TryGetComponent(out bulletController);
                bulletController.SetColor(ratio);


                if (i + 1 == bullets.Length) await transform.DORotate(new Vector3(0, 0, startAngle + range_Bullet_Attack01 * swipeDirection), 0.2f).ToUniTask(cancellationToken: token);
            }

            foreach (GameObject bullet in bullets)
            {
                if (bullet.TryGetComponent(out Bullet_MikeController controller))
                {
                    controller.Activation().Forget();
                }
            }
        }

        await UniTask.Delay(TimeSpan.FromSeconds(def_CoolTime_AfterAttack * 0.85f), cancellationToken: token);

        DrawAttack();
    }

    async UniTask Attack02(CancellationToken token)
    {
        GameObject ufo;
        UfoController _ufoController;
        Vector2 fallPosi;

        audioSource.PlayOneShot(SE_A02);

        await transform.DORotate(new Vector3(0, 0, 45), 0.2f).ToUniTask(cancellationToken: token);
        transform.DORotate(new Vector3(0, 0, -45), 1f);
        transform.DOShakeScale(1f);

        for (int i = 0; i < num_CallUFO_A02; i++)
        {
            fallPosi = (Vector2)player.transform.position +  new Vector2(UnityEngine.Random.Range(-2.5f, 2.5f), UnityEngine.Random.Range(-2.5f,2.5f));

            Instantiate(effect_AnnounceUfo, fallPosi, Quaternion.identity);
            ufo = Instantiate(ufo_Attack02, fallPosi + Vector2.one * 15, Quaternion.Euler(0, 0, 135));

            ufo.TryGetComponent(out _ufoController);

            _ufoController.falling(fallPosi).Forget();

            await UniTask.Delay(400, cancellationToken: token);
        }

        await transform.DORotate(new Vector3(0, 0, PlayerDirection()), 1f).ToUniTask(cancellationToken: token);

        await UniTask.Delay(TimeSpan.FromSeconds(def_CoolTime_AfterAttack * 0.7f), cancellationToken: token);

        DrawAttack();
    }

    async UniTask Attack03(CancellationToken token)
    {
        await transform.DORotate(new Vector3(0, 0, PlayerDirection()), 0.5f).ToUniTask(cancellationToken: token);

        audioSource.PlayOneShot(SE_A03);

        transform.DOMove(player.transform.position, 2.4f);

        for (int i = 0; i < num_bullet_A03; i++)
        {
            await transform.DORotate(new Vector3(0, 0, transform.localEulerAngles.z + 32f), 2.4f / num_bullet_A03).SetEase(Ease.Linear).ToUniTask(cancellationToken: token);

            float x = UnityEngine.Random.Range(0f, 1f);

            switch (x)
            {
                case < 0.08f:
                    Instantiate(rollingGreenBullet_Attack03, transform.position + transform.up * 1.2f, transform.rotation);
                    break;

                default:
                    Instantiate(rollingBullet_Attack03, transform.position + transform.up * 1.2f, transform.rotation);
                    break;
            }
        }

        await transform.DORotate(new Vector3(0, 0, transform.localEulerAngles.z + 170f), 0.45f).ToUniTask(cancellationToken: token);

        await UniTask.Delay(TimeSpan.FromSeconds(def_CoolTime_AfterAttack * 0.9f), cancellationToken: token);

        DrawAttack();
    }

    async UniTask Attack04(CancellationToken token)
    {
        cam_BeOnlyPlayer.Invoke();

        await transform.DORotate(Vector3.zero, 0.1f).ToUniTask(cancellationToken: token);

        audioSource.PlayOneShot(SE_A04_jump);

        transform.DOMoveY(transform.position.y - 1.75f, 0.3f);
        await transform.DOScaleY(defScale.y * 0.35f, 0.3f).ToUniTask(cancellationToken: token);

        transform.DOScale(new Vector2(defScale.x * 0.2f, defScale.y), 0.1f);
        await transform.DOMoveY(player.transform.position.y + 12f, 0.4f).ToUniTask(cancellationToken: token);

        transform.localScale = defScale;

        Vector2 tergetPosi = player.transform.position;

        await UniTask.Delay(TimeSpan.FromSeconds(0.2f + UnityEngine.Random.Range(0f, 0.2f)), cancellationToken: token);

        switch (tergetPosi.x)
        {
            case >=0:
                transform.position = tergetPosi + new Vector2(-6f, 0);
                transform.rotation = Quaternion.Euler(0, 0, 40);

                await transform.DOMove(tergetPosi + new Vector2(-3f, 0), 0.1f).SetEase(ease:Ease.Linear).ToUniTask(cancellationToken: token);

                cam_BeDef.Invoke();

                audioSource.PlayOneShot(SE_HeroPunch);

                Instantiate(Impuct_Attack04, core.transform.position, transform.rotation, core.transform);
                Instantiate(bullet_Attack04, transform.position + new Vector3(3f, 0, 0), Quaternion.identity);
                if (flag_HPUnder50)
                {
                    Instantiate(bullet_Attack04, transform.position + new Vector3(2.5f, 1f, 0), Quaternion.identity);
                    Instantiate(bullet_Attack04, transform.position + new Vector3(2.5f, -1f, 0), Quaternion.identity);
                }

                await transform.DOLocalRotate(new Vector3(0, 0, -360), 1f, RotateMode.FastBeyond360).SetEase(ease:Ease.OutCubic).ToUniTask(cancellationToken: token);
                break;

            case < 0:
                transform.position = tergetPosi + new Vector2(6f, 0);
                transform.rotation = Quaternion.Euler(0, 0, -40);

                await transform.DOMove(tergetPosi + new Vector2(3f, 0), 0.1f).SetEase(ease: Ease.Linear).ToUniTask(cancellationToken: token);

                cam_BeDef.Invoke();

                audioSource.PlayOneShot(SE_HeroPunch);

                Instantiate(Impuct_Attack04, core.transform.position, transform.rotation, core.transform);
                Instantiate(bullet_Attack04, transform.position + new Vector3(-3f, 0, 0), Quaternion.identity);
                if(flag_HPUnder50)
                {
                    Instantiate(bullet_Attack04, transform.position + new Vector3(-2.5f, 1f, 0), Quaternion.identity);
                    Instantiate(bullet_Attack04, transform.position + new Vector3(-2.5f, -1f, 0), Quaternion.identity);
                }

                await transform.DOLocalRotate(new Vector3(0, 0, 720), 1f, RotateMode.FastBeyond360).SetEase(ease: Ease.OutCubic).ToUniTask(cancellationToken: token);
                break;
        }

        await UniTask.Delay(TimeSpan.FromSeconds(def_CoolTime_AfterAttack * 0.3f), cancellationToken: token);

        DrawAttack();
    }

    async UniTask SpecialAttack(CancellationToken token)
    {
        float shotReat = 0.04f;
        int x = 1;
        float angleBetween = 0;
        GameObject ufo;
        UfoController _ufoController;
        Vector2 fallPosi;

        audioSource.PlayOneShot(SE_SA_Charge);

        await transform.DORotate(new Vector3(0, 0, PlayerDirection()), 0.25f).ToUniTask(cancellationToken: token);

        await transform.DOShakePosition(0.8f, 1f, 20, 1, false, false);

        Instantiate(effect_Hyper, core.transform.position, Quaternion.identity, core.transform);

        impulseSource.GenerateImpulse();

        audioSource.PlayOneShot(SE_SA_Shot);

        for (int i = 0; i < 10; i++)
        {
            Instantiate(bullet_SpeciallAttack, core.transform.position, Quaternion.Euler(0, 0, transform.localEulerAngles.z + UnityEngine.Random.Range(-1f, 1f)));

            await core.transform.DOShakePosition(shotReat, 0.5f, 15).ToUniTask(cancellationToken: token);
        }

        for (int i = 0; i < 12; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                Instantiate(bullet_SpeciallAttack, core.transform.position, Quaternion.Euler(0, 0, transform.localEulerAngles.z + UnityEngine.Random.Range(-1f, 1f)));

                angleBetween = Mathf.Abs(PlayerDirection() - Mathf.Repeat(transform.localEulerAngles.z, 360));

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

                if (angleBetween > 2.2f)
                {
                    transform.Rotate(0, 0, 2.2f * x);
                }
                else
                {
                    transform.Rotate(0, 0, angleBetween * x);
                }

                await core.transform.DOShakePosition(shotReat, 0.5f, 15).ToUniTask(cancellationToken: token);
            }

            fallPosi = (Vector2)player.transform.position + new Vector2(UnityEngine.Random.Range(-2.5f, 2.5f), UnityEngine.Random.Range(-2.5f, 2.5f));

            Instantiate(effect_AnnounceUfo, fallPosi, Quaternion.identity);
            ufo = Instantiate(ufo_Attack02, fallPosi + Vector2.one * 15, Quaternion.Euler(0, 0, 135));

            ufo.TryGetComponent(out _ufoController);

            _ufoController.falling(fallPosi).Forget();
        }

        BeHyperMode();

        await UniTask.Delay(800, cancellationToken: token);

        DrawAttack();
    }

    void BeHyperMode()
    {
        def_CoolTime_AfterAttack *= 0.8f;
        num_Shot_A01++;
        num_CallUFO_A02++;
        num_bullet_A03 = 30;
        chance_GreenBullet /= 1.6f;

        weight_Attack[1] += 10;

        Debug.Log("Hyper!");
    }
}
