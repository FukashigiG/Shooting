using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading.Tasks;

public class HPGaugeController : MonoBehaviour
{
    [SerializeField] Image health;

    [SerializeField] float dulation;

    RectTransform _rectTransform;

    Vector3 initPosi;

    Tweener shaker;

    private void Start()
    {
        TryGetComponent(out _rectTransform);

        initPosi = _rectTransform.position;
    }

    public void Set(MobStatus x)
    {
        x.ratio_HP
            .Skip(1)
            .Subscribe(ValueFluctuations)
            .AddTo(this.gameObject);
    }

    public void ValueFluctuations(float tergetRate)
    {
        health.fillAmount = tergetRate;

        if (shaker != null) 
        {
            shaker.Kill();
            _rectTransform.position = initPosi;
        }

        shaker = _rectTransform.DOShakeAnchorPos(dulation, 20f, 20);
    }

    private void OnDisable()
    {
        if (DOTween.instance != null) shaker?.Kill();
    }
}
