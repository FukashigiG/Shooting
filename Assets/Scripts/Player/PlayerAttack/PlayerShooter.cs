using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using System;
using DG.Tweening;
using System.Threading;
using UnityEngine.UI;

public class PlayerShooter : Base_PlayerAttack
{

    [Serializable]
    protected class ShooterFunction_MainWeapon : BaseFunction_Weapon
    {
        float maxChargeTime;

        public float ratio_Charging {  get; private set; }

        public void StartCharge()
        {
            ratio_Charging = 0;

            _weaponEnum = WeaponEnum.onCharge;
        }

        public void ChargingIfOn()
        {
            if (_weaponEnum != WeaponEnum.onCharge) return;

            ratio_Charging += Time.deltaTime / maxChargeTime;
            ratio_Charging = Mathf.Clamp01(ratio_Charging);
        }

        public float SubmitChargeValue()
        {
            float x = ratio_Charging;

            ratio_Charging = 0;

            _weaponEnum = WeaponEnum.standBy;

            SetCooling(0f);

            return x;
        }

        public ShooterFunction_MainWeapon(ShooterStatus_MainWeapon _status, Image _fillImage) :base(_status, _fillImage)
        {
            maxChargeTime = _status.sec_MaxCharge;

            ratio_Charging = 0f;
        }
    }


    [Serializable] class ShooterFunction_SubWeapon : BaseFunction_Weapon
    {
        public ShooterFunction_SubWeapon(ShooterStatus_SubWeapon _status, Image _fillImage) : base(_status, _fillImage)
        {
        }
    }


    ShooterFunction_MainWeapon funk_Main;
    ShooterFunction_SubWeapon funk_Sub;



    [Serializable] protected class ShooterStatus
    {
        public ShooterStatus_MainWeapon main;
        public ShooterStatus_SubWeapon sub;
    }

    [Serializable] protected class ShooterStatus_MainWeapon : BaseStatus
    {
        public float sec_MaxCharge;
        public float interval_Shot;
        public int maxNum_Bullet;
        public float maxMag_Shake;
        public float minMag_Shake;
    }

    [Serializable] protected class ShooterStatus_SubWeapon : BaseStatus
    {
        public float distance_Slide;
        public int flame_JustAction;
    }



    ShooterStatus status = new ShooterStatus();

    [SerializeField] GameObject bullet;
    [SerializeField] GameObject effect_Slide;
    [SerializeField] GameObject effect_Charge;
    [SerializeField] GameObject effect_StartShot;

    float magnitude_Shake;

    [SerializeField] AudioClip SE_Shot;
    [SerializeField] AudioClip SE_Slide;

    GameObject chargeEffect;



    //à»â∫ÉÅÉCÉìèàóù



    protected override void Awake()
    {
        base.Awake();

        string jsonStr = jsonFile.ToString();

        status = JsonUtility.FromJson<ShooterStatus>(jsonStr);

        funk_Main = new ShooterFunction_MainWeapon(status.main, image_ForFill_Main);
        funk_Sub = new ShooterFunction_SubWeapon(status.sub, image_ForFill_Sub);
    }

    protected override void Update()
    {
        if (onPlay != true) return;

        base.Update();

        funk_Main.Cooling();
        funk_Sub.Cooling();

        funk_Main.ChargingIfOn();
    }

    protected override void OnMainAttackHolded()
    {
        if (onPlay != true) return;
        if (funk_Main._weaponEnum != WeaponEnum.standBy) return;
        if (funk_Main.ratio_Cooling < 1f) return;

        base.OnMainAttackHolded();

        funk_Main.StartCharge();

        chargeEffect = Instantiate(effect_Charge, transform.position, Quaternion.identity, transform.parent);
    }

    protected override void OnMainAttackReleased()
    {
        if (onPlay != true) return;
        if (funk_Main._weaponEnum != WeaponEnum.onCharge) return;

        base.OnMainAttackPlessed();

        Shot(_cancellationToken, funk_Main.SubmitChargeValue()).Forget();

        if (chargeEffect != null) Destroy(chargeEffect);
    }

    protected override void OnSubAttackPlessed()
    {
        if (onPlay != true) return;
        if (funk_Sub._weaponEnum != WeaponEnum.standBy) return;
        if (funk_Sub.ratio_Cooling < 1f) return;

        base.OnSubAttackPlessed();

        Slide(_cancellationToken).Forget();

        funk_Sub.SetCooling(0f);
    }



    protected override void WhenJustAction()
    {
        if (onPlay != true) return;

        base.WhenJustAction();

        funk_Main.SetCooling(1);
        funk_Sub.SetCooling(1);

        Shot360(_cancellationToken).Forget();
    }

    async UniTask Slide(CancellationToken token)
    {
        if (onPlay != true) return;

        funk_Main._weaponEnum = WeaponEnum.onAction;

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


        _status.Evasing(status.sub.flame_JustAction);


        RaycastHit2D hit = Physics2D.Raycast(transform.position, slideDirection, status.sub.distance_Slide, wallLayer);

        if (hit.collider)
        {
            await transform.parent.DOMove(hit.point, 0.08f).ToUniTask(cancellationToken: token);
        }
        else
        {
            await transform.parent.DOMove(transform.position + slideDirection * status.sub.distance_Slide, 0.08f).ToUniTask(cancellationToken: token);
        }

        _controller.isMovable = true;

        funk_Main._weaponEnum = WeaponEnum.standBy;
    }

    async UniTask Shot(CancellationToken token, float chargeWariai)
    {
        if (onPlay != true) return;

        int num_bullet = (int)((float)status.main.maxNum_Bullet * chargeWariai);
        if (num_bullet <= 0) return;

        funk_Main._weaponEnum = WeaponEnum.onAction;

        Instantiate(effect_StartShot, transform.position, Quaternion.identity, transform.parent);

        for (int i = 0; i < num_bullet; i++)
        {
            if (_controller.isMoving)
            {
                magnitude_Shake = UnityEngine.Random.Range(-1 * status.main.maxMag_Shake, status.main.maxMag_Shake);
            }
            else
            {
                magnitude_Shake = UnityEngine.Random.Range(-1 * status.main.minMag_Shake, status.main.minMag_Shake);
            }

            Instantiate(bullet, transform.position, Quaternion.Euler(0, 0, transform.localEulerAngles.z + magnitude_Shake));
            AS.PlayOneShot(SE_Shot);

            await transform.DOShakePosition(status.main.interval_Shot, 0.5f, 15).ToUniTask(cancellationToken: token);
        }

        funk_Main._weaponEnum = WeaponEnum.standBy;
    }

    async UniTask Shot360(CancellationToken token)
    {
        if (onPlay != true) return;

        _controller.isRotatable = false;

        for(int i = 0; i < status.main.maxNum_Bullet; i++)
        {
            Instantiate(bullet, transform.position, transform.rotation);

            await transform.DORotate(new Vector3(0, 0, transform.localEulerAngles.z + (360f / status.main.maxNum_Bullet)), 0.05f).SetEase(Ease.Linear).ToUniTask(cancellationToken: token);
        }

        _controller.isRotatable = true;

        
    }
}
