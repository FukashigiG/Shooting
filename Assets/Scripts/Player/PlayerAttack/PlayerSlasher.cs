using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.InputSystem;
using System.Threading;
using UnityEngine.UI;

public class PlayerSlasher : Base_PlayerAttack
{
    [Serializable]
    class SlasherFunction_MainWeapon : BaseFunction_Weapon
    {
        float maxChargeTime;
        public float ratio_Charging { get; private set; }

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

        public SlasherFunction_MainWeapon(SlasherStatus_MainWeapon _status, Image _fillImage) : base(_status, _fillImage)
        {
            maxChargeTime = _status.sec_MaxCharge;

            ratio_Charging = 0f;
        }
    }

    [Serializable]
    class SlasherFunction_SubWeapon : BaseFunction_Weapon
    {
        public SlasherFunction_SubWeapon(SlasherStatus_SubWeapon _status, Image _fillImage) : base(_status, _fillImage)
        {
        }
    }


    SlasherFunction_MainWeapon funk_Main;
    SlasherFunction_SubWeapon funk_Sub;



    [Serializable]
    protected class SlasherStatus
    {
        public SlasherStatus_MainWeapon main;
        public SlasherStatus_SubWeapon sub;
    }

    [Serializable]
    protected class SlasherStatus_MainWeapon : BaseStatus
    {
        public float sec_MaxCharge;
        public int maxNum_SlashHit;
        public float maxLength_Slash;
        public float fleezeTime_AfterAttack;
        public int flame_JustAction;
    }

    [Serializable]
    protected class SlasherStatus_SubWeapon : BaseStatus
    {
        
    }


    SlasherStatus status = new SlasherStatus();


    [SerializeField] GameObject Slashbullet;
    [SerializeField] GameObject SpiralSlash;
    [SerializeField] GameObject effect_Charging;
    [SerializeField] GameObject effect_AnnounceMaxCharge;
    GameObject chargeEffect;

    [SerializeField] AudioClip SE_AttackStart;
    [SerializeField] AudioClip SE_AttackEnd;

    protected override void Awake()
    {
        base.Awake();

        string jsonStr = jsonFile.ToString();

        status = JsonUtility.FromJson<SlasherStatus>(jsonStr);

        funk_Main = new SlasherFunction_MainWeapon(_status: status.main, _fillImage: image_ForFill_Main);
        funk_Sub = new SlasherFunction_SubWeapon(_status: status.sub, _fillImage: image_ForFill_Sub);
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
        if (! funk_Main.isReadyToAct()) return;

        base.OnMainAttackHolded();

        funk_Main.StartCharge();

        _controller.isMovable = false;

        AS.PlayOneShot(SE_AttackStart);

        chargeEffect = Instantiate(effect_Charging, transform.position, Quaternion.identity, this.transform);
    }

    protected override void OnMainAttackReleased()
    {
        if (onPlay != true) return;
        if(funk_Main._weaponEnum != WeaponEnum.onCharge) return;

        base.OnMainAttackReleased();

        DashSlash(funk_Main.SubmitChargeValue(), _cancellationToken).Forget();

        Destroy(chargeEffect);

    }

    protected override void OnSubAttackPlessed()
    {
        if (onPlay != true) return;
        if (!funk_Sub.isReadyToAct()) return;

        base.OnSubAttackPlessed();

        Slash(_cancellationToken).Forget();

        funk_Sub.SetCooling(0f);
    }


    async UniTask Slash(CancellationToken token)
    {
        if (onPlay != true) return;

        _controller.isMovable = false;
        _controller.isRotatable = false;

        Instantiate(SpiralSlash, transform.position , transform.rotation);

        await transform.DORotate(new Vector3(0, 0, transform.localEulerAngles.z - 360) ,0.2f, RotateMode.FastBeyond360).ToUniTask(cancellationToken: token);

        _controller.isMovable = true;
        _controller.isRotatable = true;
    }

    async UniTask DashSlash(float power, CancellationToken token)
    {
        if (onPlay != true) return;

        AS.PlayOneShot(SE_AttackEnd);

        funk_Main._weaponEnum = WeaponEnum.onAction;

        _controller.isRotatable = false;

        Vector2 posi_BeforeSlash = transform.position;

        _status.Evasing((int)(status.main.flame_JustAction * power));

        int num_slash = (int)(status.main.maxNum_SlashHit * power);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, status.main.maxLength_Slash * power, wallLayer);

        if (hit.collider)
        {
            await transform.parent.DOMove(hit.point, 0.08f).SetEase(Ease.OutCubic).ToUniTask(cancellationToken: token);
        }
        else
        {
            await transform.parent.DOMove(transform.position + this.transform.up * power * status.main.maxLength_Slash, 0.08f).SetEase(Ease.OutCubic).ToUniTask(cancellationToken: token);
        }

        if(num_slash > 0)
        {
            Vector2 posi;

            float fleezeTime_This = status.main.fleezeTime_AfterAttack * power * 0.7f + status.main.fleezeTime_AfterAttack * 0.3f;

            for (int i = 1; i <= num_slash; i++)
            {
                posi = Vector2.Lerp(posi_BeforeSlash, transform.position, ((float)i / (float)(num_slash + 1)));

                Instantiate(Slashbullet, posi, Quaternion.identity);

                await UniTask.Delay(TimeSpan.FromSeconds(fleezeTime_This / num_slash), cancellationToken: token);
            }
        }

        funk_Main._weaponEnum = WeaponEnum.standBy;

        _controller.isMovable = true;
        _controller.isRotatable = true;
    }

    protected override void WhenJustAction()
    {
        if (onPlay != true) return;

        base.WhenJustAction();

        funk_Main.SetCooling(1);
        funk_Sub.SetCooling(1);

        Slash(_cancellationToken).Forget();
    }

    float R()
    {
        return UnityEngine.Random.Range(-0.5f, 0.5f);
    }
}
