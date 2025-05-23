using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerArmerd : Base_PlayerAttack
{
    [Serializable]
    class ArmerdFunction_MainWeapon : BaseFunction_Weapon
    {
        public override bool ActionIfCan()
        {
            bool x = base.ActionIfCan();

            if (x == true) SetCooling(0);

            return x;
        }

        public ArmerdFunction_MainWeapon(ArmerdStatus_MainWeapon _status, Image _fillImage) : base(_status, _fillImage)
        {
        }
    }

    [Serializable]
    class ArmerdFunction_SubWeapon : BaseFunction_Weapon
    {
        float guargableTime;

        float ratio_GuardGauge;

        public UnityEvent _event;

        Image image_ForFill_Guard;

        public void onDeployShield()
        {
            SetCooling(0f);

            _weaponEnum = WeaponEnum.onAction;

            ratio_GuardGauge = 1f;
        }

        public void onGuardIf()
        {
            if (_weaponEnum != WeaponEnum.onAction) return;
            if (ratio_GuardGauge < 0)
            {
                _event.Invoke();

                return;
            }

            ratio_GuardGauge -= Time.deltaTime / guargableTime;

            image_ForFill_Guard.fillAmount = ratio_GuardGauge;
        }

        public void onCloseShield()
        {
            _weaponEnum = WeaponEnum.standBy;

            image_ForFill_Guard.fillAmount = 0f;
        }

        public ArmerdFunction_SubWeapon(ArmerdStatus_SubWeapon _status, Image _fillImage, Image _fill_Guard) : base(_status, _fillImage)
        {
            guargableTime = _status.sec_DeployableShield;

            _event = new UnityEvent();

            ratio_GuardGauge = 0;

            image_ForFill_Guard = _fill_Guard;
        }
    }


    ArmerdFunction_MainWeapon funk_Main;
    ArmerdFunction_SubWeapon funk_Sub;





    [Serializable]
    protected class ArmerdStatus
    {
        public ArmerdStatus_MainWeapon main;
        public ArmerdStatus_SubWeapon sub;
    }

    [Serializable]
    protected class ArmerdStatus_MainWeapon : BaseStatus
    {
        public float interval_shot;
        public float length_Ray_SearchTatget;
        public int num_Missile;
        public float mag_Shake;
    }

    [Serializable]
    protected class ArmerdStatus_SubWeapon : BaseStatus
    {
        public float sec_DeployableShield;
    }



    ArmerdStatus status = new ArmerdStatus();

    [SerializeField] Image image_ForFill_Guard;

    [SerializeField] GameObject laser;

    [SerializeField] LayerMask enemyLayer;

    [SerializeField] GameObject prefab_Shield;

    [SerializeField] GameObject FX_CloseShield;

    GameObject shield;

    ShieldStatus _shieldStatus;

    protected override void Awake()
    {
        base.Awake();

        string jsonStr = jsonFile.ToString();

        status = JsonUtility.FromJson<ArmerdStatus>(jsonStr);

        funk_Main = new ArmerdFunction_MainWeapon(_status: status.main, _fillImage: image_ForFill_Main);
        funk_Sub = new ArmerdFunction_SubWeapon(_status: status.sub, _fillImage: image_ForFill_Sub, image_ForFill_Guard);

        shield = null;
    }

    protected override void Start()
    {
        base.Start();

        funk_Sub._event.AddListener(CloseShield);
    }

    protected override void Update()
    {
        if (!onPlay) return;

        base.Update();

        funk_Main.Cooling();
        funk_Sub.Cooling();

        funk_Sub.onGuardIf();
    }

    protected override void OnMainAttackPlessed()
    {
        if (!onPlay) return;
        if (!funk_Main.ActionIfCan()) return;

        base.OnSubAttackPlessed();

        FiringMissiles(_cancellationToken).Forget();
    }

    protected override void OnSubAttackHolded()
    {
        if (! onPlay) return;
        if (! funk_Sub.ActionIfCan()) return;

        base.OnSubAttackHolded();

        funk_Sub.onDeployShield();
        DeployShield();
    }

    protected override void OnSubAttackReleased()
    {
        if (! onPlay) return;
        if (funk_Sub._weaponEnum != WeaponEnum.onAction) return;

        base.OnSubAttackPlessed();

        CloseShield();
    }




    //-^-^-^-^-^-^-^-^-^-^-^-^-^-^-^-^-^--^-^-^-^-^-^-^-^-^-^-^-^-^-^-^-^-^-^-^-^-^-^-^-^-^-^-^-^


    async UniTask FiringMissiles(CancellationToken token)
    {
        funk_Main._weaponEnum = WeaponEnum.onAction;

        GameObject theBullet;
        GameObject terget = null;

        RaycastHit2D hit = Physics2D.CircleCast(transform.position, 3f, transform.up, status.main.length_Ray_SearchTatget, enemyLayer);

        if (hit.collider)
        {
            terget = hit.collider.gameObject;
        }

        for (int i = 0; i < status.main.num_Missile; i++)
        {
            theBullet = Instantiate(laser, transform.position, Quaternion.Euler(0, 0, transform.localEulerAngles.z + UnityEngine.Random.Range(-1f, 1f) * status.main.mag_Shake));

            if (theBullet.TryGetComponent(out PlayerMissileCtrl missileCtrl)) missileCtrl.SetTerget(terget);

            await transform.DOShakePosition(status.main.interval_shot, 0.5f, 15).ToUniTask(cancellationToken: token);
        }

        funk_Main._weaponEnum = WeaponEnum.standBy;
    }

    void DeployShield()
    {
        funk_Sub.onDeployShield();

        shield = Instantiate(prefab_Shield, transform.position + transform.up * 1.4f, transform.rotation, parent: this.transform); 

        _shieldStatus = shield.GetComponent<ShieldStatus>();

        _shieldStatus._event_JustGuard.AddListener(_playerStatus.TriggerJustAction);
    }

    void CloseShield()
    {
        funk_Sub?.onCloseShield();

        _shieldStatus._event_JustGuard?.RemoveAllListeners();

        Instantiate(FX_CloseShield, shield.transform.position, Quaternion.identity);

        Destroy(shield);
    }

    protected override void WhenJustAction()
    {
        base.WhenJustAction();

        funk_Main.AddStock();
        funk_Sub.AddStock(); ;
    }
}
