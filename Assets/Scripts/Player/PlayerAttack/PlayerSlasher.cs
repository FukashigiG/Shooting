using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.InputSystem;
using System.Threading;

public class PlayerSlasher : Base_PlayerAttack
{
    [Serializable]
    protected class SlasherData : BaseData
    {
        public int maxNum_SlashHit;
        public float maxLength_Slash;
        public float fleezeTime_AfterAttack;

        public float ratio_cooltime_Dash;
        public float ratio_cooltime_Spiral;

        public int flame_JustAction;
    }

    [SerializeField] SlasherData slasherData = new SlasherData();

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

        slasherData = JsonUtility.FromJson<SlasherData>(jsonStr);
    }

    protected override void OnAttackTapped()
    {
        if (onPlay != true) return;
        if (cooling < data.cooltime_Attack * slasherData.ratio_cooltime_Spiral) return;

        base.OnAttackTapped();

        Slash(_cancellationToken).Forget();

        cooling -= (data.cooltime_Attack * slasherData.ratio_cooltime_Spiral);
        image_Fill.fillAmount = 1 - (cooling / data.cooltime_Attack);
    }

    protected override void OnAttackHolded()
    {
        if (onPlay != true) return;
        if (cooling < data.cooltime_Attack * slasherData.ratio_cooltime_Dash) return;

        base.OnAttackHolded();

        chargeValue = 0;

        isCharging=true;

        _controller.isMovable = false;

        AS.PlayOneShot(SE_AttackStart);

        chargeEffect = Instantiate(effect_Charging, transform.position, Quaternion.identity, this.transform);
    }

    protected override void OnAttackReleased()
    {
        if (onPlay != true) return;
        if(isCharging != true) return;

        base.OnAttackPlessed();

        float ratio_Power = chargeValue / data.sec_MaxCharge;
        DashSlash(ratio_Power, _cancellationToken).Forget();

        isCharging = false;
        chargeValue = 0;

        cooling -= (data.cooltime_Attack * slasherData.ratio_cooltime_Dash);
        image_Fill.fillAmount = 1 - (cooling / data.cooltime_Attack);
        image_Fill_charge.fillAmount = 0;

        Destroy(chargeEffect);

    }
    

    async UniTask Slash(CancellationToken token)
    {
        _controller.isMovable = false;
        _controller.isRotatable = false;

        Instantiate(SpiralSlash, transform.position , transform.rotation);

        await transform.DORotate(new Vector3(0, 0, transform.localEulerAngles.z - 360) ,0.2f, RotateMode.FastBeyond360).ToUniTask(cancellationToken: token);

        _controller.isMovable = true;
        _controller.isRotatable = true;
    }

    async UniTask DashSlash(float power, CancellationToken token)
    {
        Debug.Log(power);
        AS.PlayOneShot(SE_AttackEnd);

        onAttack = true;

        _controller.isRotatable = false;

        Vector2 posi_BeforeSlash = transform.position;

        _status.Evasing((int)(slasherData.flame_JustAction * power));

        int num_slash = (int)(slasherData.maxNum_SlashHit * power);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, slasherData.maxLength_Slash * power, wallLayer);

        if (hit.collider)
        {
            await transform.parent.DOMove(hit.point, 0.08f).SetEase(Ease.OutCubic).ToUniTask(cancellationToken: token);
        }
        else
        {
            await transform.parent.DOMove(transform.position + this.transform.up * power * slasherData.maxLength_Slash, 0.08f).SetEase(Ease.OutCubic).ToUniTask(cancellationToken: token);
        }

        if(num_slash > 0)
        {
            Vector2 posi;

            float fleezeTime_This = slasherData.fleezeTime_AfterAttack * power * 0.7f + slasherData.fleezeTime_AfterAttack * 0.3f;

            for (int i = 1; i <= num_slash; i++)
            {
                posi = Vector2.Lerp(posi_BeforeSlash, transform.position, ((float)i / (float)(num_slash + 1)));

                Instantiate(Slashbullet, posi, Quaternion.identity);

                await UniTask.Delay(TimeSpan.FromSeconds(fleezeTime_This / num_slash), cancellationToken: token);
            }
        }

        onAttack = false;

        _controller.isMovable = true;
        _controller.isRotatable = true;
    }

    protected override void WhenEvasioned()
    {
        base.WhenEvasioned();

        Slash(_cancellationToken).Forget();
    }

    float R()
    {
        return UnityEngine.Random.Range(-0.5f, 0.5f);
    }
}
