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
        public float ratio_Cooling {  get; private set; }

        Image image_Fill;


        public void Cooling()
        {
            if (_weaponEnum == WeaponEnum.standBy)
            {
                ratio_Cooling += Time.deltaTime / cooltime_Attack;

                ratio_Cooling = Mathf.Clamp01(ratio_Cooling);
            }

            image_Fill.fillAmount = 1 - ratio_Cooling;
        }

        public void SetCooling(float size)
        {
            size = Mathf.Clamp01(size);

            ratio_Cooling = size;

            image_Fill.fillAmount = 1f - ratio_Cooling;
        }

        public bool isReadyToAct()
        {
            if(ratio_Cooling >= 1f)
            {
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

            SetCooling(1f);
        }
    }

    [Serializable] protected class BaseStatus
    {
        public float cooltime_Attack;
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
        onCharge,
    }

    WeaponEnum _weaponEnum;

    protected bool onPlay;



    //à»â∫ÉÅÉCÉìèàóù



    protected virtual void Awake()
    {
        _cancellationToken = cancellationTokenSource.Token;

        onPlay = true;

        TryGetComponent(out AS);

        transform.parent.TryGetComponent(out _controller);
        transform.parent.TryGetComponent(out _status);

        _status.Evasioned.AddListener(WhenJustAction);
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {

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

    protected virtual void WhenJustAction()
    {

    }

    public void FInishPlaying()
    {
        onPlay = false;

        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
    }
}
