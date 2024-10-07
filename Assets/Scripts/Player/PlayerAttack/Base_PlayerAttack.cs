using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using Unity.VisualScripting;

public class Base_PlayerAttack : MonoBehaviour
{
    [Serializable] protected class BaseFunction_Weapon
    {
        public WeaponEnum _weaponEnum;

        float cooltime_Attack;
        public float ratio_Cooling {  get; protected set; }

        int maxNum_Stock;

        int stock_Action;

        protected Image image_Fill { get; private set; }


        public void Cooling()
        {
            if (_weaponEnum == WeaponEnum.standBy && stock_Action < maxNum_Stock)
            {
                ratio_Cooling += Time.deltaTime / cooltime_Attack;

                ratio_Cooling = Mathf.Clamp01(ratio_Cooling);

                image_Fill.fillAmount = 1 - ratio_Cooling;

                if (ratio_Cooling >= 1f && stock_Action < maxNum_Stock)
                {
                    stock_Action++;

                    ratio_Cooling = 0f;
                }
            }
        }

        public void SetCooling(float size)
        {
            size = Mathf.Clamp01(size);

            ratio_Cooling = size;

            image_Fill.fillAmount = 1f - ratio_Cooling;
        }

        public bool ActionIfCan()
        {
            if(stock_Action > 0 && _weaponEnum == WeaponEnum.standBy)
            {
                stock_Action--;

                return true;
            }
            else
            {
                return false;
            }
        }


        public BaseFunction_Weapon(BaseStatus status, Image fillImage)
        {
            cooltime_Attack = status.cooltime_Attack;

            image_Fill = fillImage;

            ratio_Cooling = 0f;

            image_Fill.fillAmount = 0;

            maxNum_Stock = status.num_MaxStock;

            stock_Action = maxNum_Stock;
        }
    }

    [Serializable] protected class BaseStatus
    {
        public float cooltime_Attack;

        public int num_MaxStock;
    }

    [SerializeField] protected TextAsset jsonFile;
    [SerializeField] protected LayerMask wallLayer;
    [SerializeField] protected Image image_ForFill_Main;
    [SerializeField] protected Image image_ForFill_Sub;

    protected PlayerController _controller;
    protected PlayerStatus _status;
    protected AudioSource AS;

    readonly protected CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    protected CancellationToken _cancellationToken;

    protected enum WeaponEnum
    {
        standBy,
        onAction,
        onCharge
    }

    WeaponEnum _weaponEnum;

    protected bool onPlay;



    //à»â∫ÉÅÉCÉìèàóù



    protected virtual void Awake()
    {
        _cancellationToken = cancellationTokenSource.Token;

        TryGetComponent(out AS);

        transform.parent.TryGetComponent(out _controller);
        transform.parent.TryGetComponent(out _status);

        _status.Evasioned.AddListener(WhenJustAction);
    }

    protected virtual void Start()
    {
        onPlay = true;
    }

    protected virtual void Update()
    {
        if (onPlay != true) return;
    }

    protected virtual void OnMainAttackPlessed()
    {
        if (onPlay != true) return;
    }

    protected virtual void OnMainAttackReleased()
    {
        if (onPlay != true) return;
    }

    protected virtual void OnMainAttackHolded()
    {
        if (onPlay != true) return;
    }

    protected virtual void OnMainAttackTapped()
    {
        if (onPlay != true) return;
    }

    protected virtual void OnSubAttackPlessed()
    {
        if (onPlay != true) return;
    }

    protected virtual void OnSubAttackReleased()
    {
        if (onPlay != true) return;
    }

    protected virtual void OnSubAttackHolded()
    {
        if (onPlay != true) return;
    }

    protected virtual void OnSubAttackTapped()
    {
        if (onPlay != true) return;
    }

    protected virtual void WhenJustAction()
    {
        if (onPlay != true) return;
    }

    public void FInishPlaying()
    {
        onPlay = false;

        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
    }
}
