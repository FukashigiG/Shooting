using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class Base_PlayerAttack : MonoBehaviour
{
    [Serializable] protected class BaseData
    {
        public float cooltime_Attack;
        public float sec_MaxCharge;
    }

    protected BaseData data = new BaseData();

    [SerializeField] protected TextAsset jsonFile;

    protected bool attackable;
    protected bool onPlay;

    [SerializeField] protected Image image_Fill;
    [SerializeField] protected Image image_Fill_charge;

    [SerializeField] protected LayerMask wallLayer;

    protected PlayerController _controller;
    protected PlayerStatus _status;

    protected float cooling;
    protected bool onAttack;
    protected bool isCharging = false;

    protected AudioSource AS;

    readonly protected CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    protected CancellationToken _cancellationToken;

    protected float chargeValue = 0;

    protected virtual void Awake()
    {
        string jsonStr = jsonFile.ToString();

        data = JsonUtility.FromJson<BaseData>(jsonStr);

        _cancellationToken = cancellationTokenSource.Token;
    }

    protected virtual void Start()
    {
        onPlay = true;

        TryGetComponent(out AS);

        transform.parent.TryGetComponent(out _controller);
        transform.parent.TryGetComponent(out _status);

        _status.Evasioned.AddListener(WhenEvasioned);

        cooling = data.cooltime_Attack;

        image_Fill.fillAmount = 0;
        image_Fill_charge.fillAmount = 0;
    }

    protected virtual void Update()
    {
        if(onAttack != true)
        {
            if(cooling < data.cooltime_Attack)
            {
                cooling += Time.deltaTime;
            }
            else
            {
                cooling = data.cooltime_Attack;
            }
        }

        image_Fill.fillAmount = 1 - (cooling / data.cooltime_Attack);

        if (isCharging) ChargePower(Time.deltaTime);
    }

    protected virtual void OnAttackPlessed()
    {

    }

    protected virtual void OnAttackReleased()
    {

    }

    protected virtual void OnAttackHolded()
    {

    }

    protected virtual void OnAttackTapped()
    {

    }

    protected void ChargePower(float sec)
    {
        if (chargeValue >= data.sec_MaxCharge) return;

        chargeValue += sec;

        image_Fill_charge.fillAmount = chargeValue / data.sec_MaxCharge;

        if (chargeValue >= data.sec_MaxCharge)
        {
            chargeValue = data.sec_MaxCharge;
            image_Fill_charge.fillAmount = 1;
        }
    }

    protected virtual void WhenEvasioned()
    {
        cooling = data.cooltime_Attack;

        image_Fill.fillAmount = 1 - (cooling / data.cooltime_Attack);
    }

    public void FInishPlaying()
    {
        onPlay = false;

        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
    }
}
