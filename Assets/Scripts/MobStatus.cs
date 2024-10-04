using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Events;
using UnityEngine.UI;

public class MobStatus : MonoBehaviour, IDamagable
{
    [field:SerializeField] public float MaxHP {  get; private set; }

    public float HP { get; private set; }

    bool canTakeDamage;

    [SerializeField] GameObject obj_Txt_Damage;
    [SerializeField] GameObject obj_Txt_RecoverHP;

    GameObject canvas;

    [NonSerialized] public UnityEvent onDie = new UnityEvent();

    [SerializeField] HPGaugeController HPG_C;

    class TheDamageTxt
    {
        GameObject txtObj;
        public Txt_DamageValue txtScript {  get; private set; }
        float sum_Damage;

        public TheDamageTxt(GameObject obj, float num)
        {
            this.txtObj = obj;
            this.txtScript = obj?.GetComponent<Txt_DamageValue>();

            this.sum_Damage = num;
        }

        public void AddDamage(float damage, Vector2 posi)
        {
            sum_Damage += damage;

            txtScript?.SetTxt(sum_Damage, posi);
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

        Vector3 dmgPosi = Vector3.Lerp(transform.position, damagedPosi, 0.5f);
        var screenPosi = RectTransformUtility.WorldToScreenPoint(Camera.main, dmgPosi);

        if (damageTxt.txtScript == null || damageTxt.txtScript.reloadable != true)
        {
            GameObject obj = Instantiate(obj_Txt_Damage, screenPosi, Quaternion.identity, canvas.transform);

            //ê∂ê¨ÇµÇΩobjÇÃèÓïÒÇì¸ÇÍÇÈèâä˙âª
            damageTxt = new TheDamageTxt(obj, 0);
        }

        damageTxt.AddDamage(damage, screenPosi);

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
