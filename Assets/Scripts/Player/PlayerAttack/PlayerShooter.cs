using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using System;
using DG.Tweening;
using System.Threading;

public class PlayerShooter : Base_PlayerAttack
{
    [Serializable]
    protected class ShooterData : BaseData
    {
        public float interval_Shot;
        public int maxNum_Bullet;
        public float maxMag_Shake;
        public float minMag_Shake;

        public float distance_Slide;

        public float ratio_cooltime_Shot;
        public float ratio_cooltime_Slide;

        public int flame_JustAction;
    }

    ShooterData shooterData = new ShooterData();

    [SerializeField] GameObject bullet;

    [SerializeField] GameObject effect_Slide;
    [SerializeField] GameObject effect_Charge;
    [SerializeField] GameObject effect_StartShot;

    float magnitude_Shake;

    [SerializeField] AudioClip SE_Shot;
    [SerializeField] AudioClip SE_Slide;

    GameObject chargeEffect;

    protected override void Awake()
    {
        base.Awake();

        string jsonStr = jsonFile.ToString();

        shooterData = JsonUtility.FromJson<ShooterData>(jsonStr);
    }

    protected override void OnAttackTapped()
    {
        if (onPlay != true) return;
        if (onAttack) return;
        if (cooling < (data.cooltime_Attack * shooterData.ratio_cooltime_Slide)) return;

        base.OnAttackPlessed();

        Slide(_cancellationToken).Forget();

        cooling -= (data.cooltime_Attack * shooterData.ratio_cooltime_Slide);
        image_Fill.fillAmount = 1 - (cooling / data.cooltime_Attack);
    }

    protected override void OnAttackHolded()
    {
        if (onPlay != true) return;
        if (onAttack) return;
        if (cooling < (data.cooltime_Attack * shooterData.ratio_cooltime_Shot)) return;

        base.OnAttackHolded();

        chargeValue = 0;

        isCharging = true;

        chargeEffect = Instantiate(effect_Charge, transform.position, Quaternion.identity, transform.parent);
    }

    protected override void OnAttackReleased()
    {
        if (onPlay != true) return;
        if (isCharging != true) return;

        base.OnAttackPlessed();

        Shot(_cancellationToken, chargeValue / data.sec_MaxCharge).Forget();

        if (chargeEffect != null) Destroy(chargeEffect);

        isCharging = false;
        chargeValue = 0;
        attackable = false;
        cooling -= (data.cooltime_Attack * shooterData.ratio_cooltime_Shot);
        image_Fill.fillAmount = 1 - (cooling / data.cooltime_Attack);
        image_Fill_charge.fillAmount = 0;
    }

    protected override void WhenEvasioned()
    {
        base.WhenEvasioned();

        Shot360(_cancellationToken).Forget();
    }

    async UniTask Slide(CancellationToken token)
    {
        onAttack = true;

        _controller.isMovable = false;

        Vector3 slideDirection  = Vector3.zero;

        if(_controller.moveVector != Vector2.zero)
        {
            slideDirection = (Vector3)_controller.moveVector;
        } else
        {
            slideDirection = transform.up;
        }


        float x = (Mathf.Atan2(slideDirection.y, slideDirection.x) * Mathf.Rad2Deg);

        x -= 90f;

        x = Mathf.Repeat(x, 360f);

        Instantiate(effect_Slide, transform.position, Quaternion.Euler(0, 0, x));

        AS.PlayOneShot(SE_Slide);


        _status.Evasing(shooterData.flame_JustAction);


        RaycastHit2D hit = Physics2D.Raycast(transform.position, slideDirection, shooterData.distance_Slide, wallLayer);

        if (hit.collider)
        {
            await transform.parent.DOMove(hit.point, 0.08f).ToUniTask(cancellationToken: token);
        }
        else
        {
            await transform.parent.DOMove(transform.position + slideDirection * shooterData.distance_Slide, 0.08f).ToUniTask(cancellationToken: token);
        }

        _controller.isMovable = true;

        onAttack = false;
    }

    async UniTask Shot(CancellationToken token, float chargeWariai)
    {
        int num_bullet = (int)((float)shooterData.maxNum_Bullet * chargeWariai);
        if (num_bullet <= 0) return;

        Debug.Log(Time.time);

        onAttack = true;

        Instantiate(effect_StartShot, transform.position, Quaternion.identity, transform.parent);

        for (int i = 0; i < num_bullet; i++)
        {
            if (_controller.isMoving)
            {
                magnitude_Shake = UnityEngine.Random.Range(-1 * shooterData.maxMag_Shake, shooterData.maxMag_Shake);
            }
            else
            {
                magnitude_Shake = UnityEngine.Random.Range(-1 * shooterData.minMag_Shake, shooterData.minMag_Shake);
            }

            Instantiate(bullet, transform.position, Quaternion.Euler(0, 0, transform.localEulerAngles.z + magnitude_Shake));
            AS.PlayOneShot(SE_Shot);

            await transform.DOShakePosition(shooterData.interval_Shot, 0.5f, 15).ToUniTask(cancellationToken: token);
        }

        onAttack = false;
    }

    async UniTask Shot360(CancellationToken token)
    {
        onAttack = true;

        _controller.isRotatable = false;

        for(int i = 0; i < shooterData.maxNum_Bullet; i++)
        {
            Instantiate(bullet, transform.position, transform.rotation);

            await transform.DORotate(new Vector3(0, 0, transform.localEulerAngles.z + (360f / shooterData.maxNum_Bullet)), 0.05f).SetEase(Ease.Linear).ToUniTask(cancellationToken: token);
        }

        _controller.isRotatable = true;

        onAttack = false;
    }
}
