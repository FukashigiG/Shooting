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
        public ArmerdFunction_MainWeapon(ArmerdStatus_MainWeapon _status, Image _fillImage) : base(_status, _fillImage)
        {
        }
    }

    [Serializable]
    class ArmerdFunction_SubWeapon : BaseFunction_Weapon
    {
        float guargableTime;

        public UnityEvent _event;

        public void onGuardIf()
        {
            if (_weaponEnum != WeaponEnum.onAction) return;
            if (ratio_Cooling <= 0)
            {
                _event.Invoke();

                return;
            }

            ratio_Cooling -= Time.deltaTime / guargableTime;
            image_Fill.fillAmount = 1 - ratio_Cooling;

            
        }

        public ArmerdFunction_SubWeapon(ArmerdStatus_SubWeapon _status, Image _fillImage) : base(_status, _fillImage)
        {
            guargableTime = _status.sec_DeployableShield;

            _event = new UnityEvent();
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



    [SerializeField] GameObject laser;

    [SerializeField] LayerMask enemyLayer;

    [SerializeField] GameObject prefab_Shield;

    GameObject shield;

    protected override void Awake()
    {
        base.Awake();

        string jsonStr = jsonFile.ToString();

        status = JsonUtility.FromJson<ArmerdStatus>(jsonStr);

        funk_Main = new ArmerdFunction_MainWeapon(_status: status.main, _fillImage: image_ForFill_Main);
        funk_Sub = new ArmerdFunction_SubWeapon(_status: status.sub, _fillImage: image_ForFill_Sub);

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
        if (onPlay != true) return;
        if (funk_Sub.ratio_Cooling <= 0) return;

        base.OnSubAttackHolded();

        DeployShield();
    }

    protected override void OnSubAttackReleased()
    {
        if (onPlay != true) return;
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
        funk_Sub._weaponEnum = WeaponEnum.onAction;

        shield = Instantiate(prefab_Shield, transform.position + transform.up * 1f, transform.rotation, parent: this.transform); 

        ShieldStatus _shieldStatus = shield.GetComponent<ShieldStatus>();
    }

    void CloseShield()
    {
        funk_Sub._weaponEnum = WeaponEnum.standBy;

        Destroy(shield);
    }

    protected override void WhenJustAction()
    {
        base.WhenJustAction();

        funk_Main.SetCooling(1);
        funk_Sub.SetCooling(1);
    }
}
