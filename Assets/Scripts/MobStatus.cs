using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UniRx;
using static Base_BossController;
using Unity.Collections;
using System.Drawing;

public class MobStatus : MonoBehaviour, IDamagable, IObservable<GameObject>
{
    //�w�ǐ�̃��X�g
    private List<IObserver<GameObject>> _observers = new List<IObserver<GameObject>>();

    [field:SerializeField] public float MaxHP {  get; private set; }

    public float HP { get; private set; }

    public ReactiveProperty<float> ratio_HP { get; private set; } = new ReactiveProperty<float>();

    bool canTakeDamage;

    [SerializeField] GameObject obj_Txt_Damage;
    [SerializeField] GameObject obj_Txt_RecoverHP;

    GameObject parent_DamageUI;

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


    //�������p
    public MobStatus()
    {
        
    }

    protected virtual void Awake()
    {
        HP = MaxHP;
        HP = Mathf.Clamp(HP, 0, MaxHP);

        ratio_HP.Value = HP / MaxHP;

    }

    protected virtual void Start()
    {
        parent_DamageUI = GameObject.Find("parent_DamageUI");

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
            GameObject obj = Instantiate(obj_Txt_Damage, screenPosi, Quaternion.identity, parent_DamageUI.transform);

            //��������obj�̏������鏉����
            damageTxt = new TheDamageTxt(obj, 0);
        }

        damageTxt.AddDamage(damage, screenPosi);

        HP += damage;
        HP = Mathf.Clamp(HP, 0, MaxHP);

        ratio_HP.Value = HP / MaxHP;

        if (HP <= 0) Die();

        return true;
    }

    public virtual bool Recover(float point, Vector3 Posi)
    {
        if (canTakeDamage != true) return false;

        Vector3 dmgTxtPosi = Vector3.Lerp(transform.position, Posi, 0.5f);

        var screenPosi = RectTransformUtility.WorldToScreenPoint(Camera.main, dmgTxtPosi);
        screenPosi = new Vector2(screenPosi.x * UnityEngine.Random.Range(0.96f, 1.04f), screenPosi.y * UnityEngine.Random.Range(0.96f, 1.04f));

        Txt_DamageValue txt = Instantiate(obj_Txt_RecoverHP, screenPosi, Quaternion.identity, parent_DamageUI.transform).GetComponent<Txt_DamageValue>();
        txt.SetTxt(point, screenPosi);

        HP += point;
        HP = Mathf.Clamp(HP, 0, MaxHP);

        ratio_HP.Value = HP / MaxHP;

        return true;
    }

    //�w�ǃ��\�b�h
    public IDisposable Subscribe(IObserver<GameObject> observer)
    {
        if(! _observers.Contains(observer)) _observers.Add(observer);

        //�w�ǉ����p�̃N���X��IDisposable�Ƃ��ĕԂ�
        return new Unsubscriber(_observers, observer);
    }

    //HP���s�������̏���
    public virtual void Die()
    {
        canTakeDamage = false;

        //_observers�����̂܂�foreach���Ă��܂���OnNext��ŃI�u�W�F�������鏈�������������ꍇ
        //foreach�������Ƀ��X�g�̒��g���ς��G���[���N����
        var observers = new List<IObserver<GameObject>>(_observers);

        foreach (var observer in observers)
        {
            observer.OnNext(this.gameObject);
        }
    }
}

class Unsubscriber : IDisposable
{
    private List<IObserver<GameObject>> _observers;
    private IObserver<GameObject> _observer;

    public Unsubscriber(List<IObserver<GameObject>> observers, IObserver<GameObject> observer)
    {
        _observers = observers;
        _observer = observer;
    }

    //�w�ǐ惊�X�g����Ώۂ̍w�ǎ҂��폜
    public void Dispose()
    {
        _observers.Remove(_observer);
    }
}
