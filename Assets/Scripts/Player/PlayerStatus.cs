using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStatus : MobStatus
{
    [SerializeField] GameObject effect_Evasion;
    [SerializeField] float timeScale_WhenEvasion;

    bool isEvasionable = false;
    int evasionalCount_flame;

    Collider2D _collider;

    readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    CancellationToken _cancellationToken;

    AudioSource _audioSource;
    [SerializeField] AudioClip _clip_Evasion;

    [NonSerialized] public UnityEvent _event_JustAction = new UnityEvent();


    protected override void Start()
    {
        base.Start();

        TryGetComponent(out _collider);
        TryGetComponent(out _audioSource);

        _cancellationToken = _cancellationTokenSource.Token;
    }

    private void Update()
    {
        if(evasionalCount_flame > 0 || isEvasionable)
        {
            evasionalCount_flame--;

            if(evasionalCount_flame <= 0) isEvasionable = false ;
        }
    }

    public override bool Damage(float damage, Vector3 damagedPosi, bool isCritical)
    {
        if (isEvasionable)
        {
            TriggerJustAction();

            isEvasionable = false;
            evasionalCount_flame = 0;

            return false;
        }

        base.Damage(damage, damagedPosi, isCritical);
        return true;
    }

    public void Evasing(int flame)
    {
        if (flame <= 0) return;

        evasionalCount_flame = flame;
        isEvasionable = true;
    }

    public void TriggerJustAction()
    {
        WhenJustAction(_cancellationToken).Forget();
    }

    async UniTask WhenJustAction(CancellationToken token)
    {
        Instantiate(effect_Evasion, transform.position, Quaternion.identity);

        _audioSource.PlayOneShot(_clip_Evasion);

        _collider.enabled = false;

        _event_JustAction.Invoke();

        Time.timeScale *= timeScale_WhenEvasion;

        await UniTask.Delay((int)(0.6f * 1000f * Time.timeScale), cancellationToken: token);

        Time.timeScale /= timeScale_WhenEvasion;

        await UniTask.Delay((int)(0.5f * 1000f * Time.timeScale), cancellationToken: token);

        _collider.enabled = true;
    }

    public override void Die()
    {
        base.Die();

        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
    }
}
