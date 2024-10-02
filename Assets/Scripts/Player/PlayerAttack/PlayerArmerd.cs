using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerArmerd : Base_PlayerAttack
{
    [Serializable]
    protected class ArmerdData : BaseData
    {
        public float interval_shot;
        public float length_Ray_SearchTatget;
        public int num_Missile;
        public float mag_Shake;

        public float ratio_Missile;

        public float sec_DeployableShield;
        public float ratio_DeploySheld;
    }

    ArmerdData armerdData = new ArmerdData();

    [SerializeField] GameObject laser;

    [SerializeField] LayerMask enemyLayer;

    bool onGuard;

    protected override void Awake()
    {
        base.Awake();

        string jsonStr = jsonFile.ToString();

        armerdData = JsonUtility.FromJson<ArmerdData>(jsonStr);

        onGuard = false;
    }

    protected override void OnAttackTapped()
    {
        if (!onPlay) return;
        if (onAttack) return;
        if (cooling < data.cooltime_Attack) return;

        base.OnAttackTapped();

        FiringMissiles(_cancellationToken).Forget();

        cooling -= data.cooltime_Attack * armerdData.ratio_Missile;
        image_Fill.fillAmount = 1 - (cooling / data.cooltime_Attack);
    }

    protected override void Update()
    {
        if (onGuard)
        {
            cooling -= data.cooltime_Attack * Time.deltaTime / armerdData.sec_DeployableShield;
        }

        base.Update();
    }

    protected override void OnAttackHolded()
    {
        if (onPlay != true) return;
        if (onAttack) return;
        if (cooling < data.cooltime_Attack * armerdData.ratio_DeploySheld) return;

        base.OnAttackHolded();

        onAttack = true;

        cooling -= data.cooltime_Attack * armerdData.ratio_DeploySheld;
        image_Fill.fillAmount = 1 - (cooling / data.cooltime_Attack);

        DeployShield();
    }

    protected override void OnAttackReleased()
    {
        if (onPlay != true) return;
        if (onAttack != true) return;

        base.OnAttackPlessed();

        onAttack = false;
        
        CloseShield();
    }

    async UniTask FiringMissiles(CancellationToken token)
    {
        onAttack = true;

        GameObject theBullet;
        GameObject terget = null;

        RaycastHit2D hit = Physics2D.CircleCast(transform.position, 3f, transform.up, armerdData.length_Ray_SearchTatget, enemyLayer);

        if (hit.collider)
        {
            terget = hit.collider.gameObject;
        }

        for (int i = 0; i < armerdData.num_Missile; i++)
        {
            theBullet = Instantiate(laser, transform.position, Quaternion.Euler(0, 0, transform.localEulerAngles.z + UnityEngine.Random.Range(-1f, 1f) * armerdData.mag_Shake));

            if (theBullet.TryGetComponent(out PlayerMissileCtrl missileCtrl)) missileCtrl.SetTerget(terget);

            await transform.DOShakePosition(armerdData.interval_shot, 0.5f, 15).ToUniTask(cancellationToken: token);
        }

        onAttack = false;
    }

    void DeployShield()
    {
        onGuard = true;
    }

    void CloseShield()
    {
        onGuard = false;
    }
}
