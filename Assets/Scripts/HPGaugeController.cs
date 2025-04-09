using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UniRx;

public class HPGaugeController : MonoBehaviour
{
    [SerializeField] Image health;

    [SerializeField] float dulation;

    RectTransform rect;

    MobStatus _mobStatus;

    bool isAnimating = false;
    private void Start()
    {

    }

    public void SetGauge_Damage(float tergetRate)
    {
        health.fillAmount = tergetRate;

        if(isAnimating != true)
        {
            isAnimating = true;
            transform.DOShakePosition(0.4f, 20f, 20).OnComplete(() =>
            {
                isAnimating = false;
            });
        }
    }

    public void SetGauge_Heal(float tergetRate)
    {
        health.fillAmount = tergetRate;
    }
}
