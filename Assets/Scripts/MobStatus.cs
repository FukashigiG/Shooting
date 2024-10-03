using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Events;
using UnityEngine.UI;

public class MobStatus : MonoBehaviour, IDamagable
{
    [SerializeField] float MaxHP;

    public float TheMaxHP => MaxHP;

    [SerializeField] float HP;

    public float TheHP => HP;

    bool canTakeDamage;

    [SerializeField] GameObject obj_Txt_Damage;
    [SerializeField] GameObject obj_Txt_RecoverHP;

    GameObject canvas;

    [NonSerialized] public UnityEvent onDie = new UnityEvent();

    [SerializeField] HPGaugeController HPG_C;

    class TheDamageTxt
    {
        public Txt_DamageValue txtScript;
        public float sum_Damage;

        public TheDamageTxt(Txt_DamageValue theScript, float num)
        {
            this.txtScript = theScript;
            this.sum_Damage = num;
        }
    }

    TheDamageTxt damageTxt;
 

    protected virtual void Start()
    {
        HP = MaxHP;

        canvas = GameObject.Find("Canvas");

        canTakeDamage = true;

        damageTxt = new TheDamageTxt(null, 0);
    }

    public virtual bool Damage(float damage, Vector3 damagedPosi, bool isCritical)
    {
        if (canTakeDamage != true) return false;

        if (isCritical)
        {
            damage *= 2;
        }

        Vector3 dmgTxtPosi = Vector3.Lerp(transform.position, damagedPosi, 0.5f);

        var screenPosi = RectTransformUtility.WorldToScreenPoint(Camera.main, dmgTxtPosi);

        if (damageTxt.txtScript == null)
        {
            GameObject obj = Instantiate(obj_Txt_Damage, screenPosi, Quaternion.identity, canvas.transform);
            obj.TryGetComponent(out Txt_DamageValue txt_DamageValue);
            damageTxt.sum_Damage = 0;

            //ê∂ê¨ÇµÇΩobjÇÃèÓïÒÇì¸ÇÍÇÈèâä˙âª
            damageTxt = new TheDamageTxt(txt_DamageValue, 0);
        }
        
        damageTxt.sum_Damage += damage;
        damageTxt.txtScript.SetTxt(damageTxt.sum_Damage, screenPosi);

        HP -= damage;
        if (HPG_C != null) HPG_C.SetGauge_Damage(HP / MaxHP);
        if (HP <= 0) Die();

        return true;
    }

    public virtual bool Recover(float point, Vector3 Posi)
    {
        if (canTakeDamage != true) return false;

        Vector3 dmgTxtPosi = Vector3.Lerp(transform.position, Posi, 0.5f);

        var screenPosi = RectTransformUtility.WorldToScreenPoint(Camera.main, dmgTxtPosi);
        screenPosi = new Vector2(screenPosi.x * UnityEngine.Random.Range(0.96f, 1.04f), screenPosi.y * UnityEngine.Random.Range(0.96f, 1.04f));

        Txt_DamageValue txt = Instantiate(obj_Txt_RecoverHP, screenPosi, Quaternion.identity, canvas.transform).GetComponent<Txt_DamageValue>();
        txt.SetTxt(point, screenPosi);


        if (MaxHP - HP <= point)
        {
            HP = MaxHP;
        }
        else
        {
            HP += point;

        }
        if (HPG_C != null) HPG_C.SetGauge_Heal(HP / MaxHP);

        return true;
    }

    public void SetHPGauge(HPGaugeController gauge)
    {
        if (HPG_C == null) HPG_C = gauge;
    }

    public virtual void Die()
    {
        canTakeDamage = false;

        onDie.Invoke();
    }
}
