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

    [NonSerialized] public UnityEvent Evasioned = new UnityEvent();


    protected override void Start()
    {
        base.Start();

        TryGetComponent(out _collider);
        TryGetComponent(out _audioSource);

        _cancellationToken = _cancellationTokenSource.Token;
    }

    private void Update()
    {
        if(evasionalCount_flame > 0)
        {
            evasionalCount_flame--;

            if(evasionalCount_flame <= 0) isEvasionable = false ;
        }
    }

    public override bool Damage(float damage, Vector3 damagedPosi, bool isCritical)
    {
        if (isEvasionable)
        {
            WhenEvasion(_cancellationToken).Forget();

            return false;
        }

        base.Damage(damage, damagedPosi, isCritical);
        return true;
    }

    public void Evasing(int flame)
    {
        evasionalCount_flame = flame;
        isEvasionable = true;
    }

    async UniTask WhenEvasion(CancellationToken token)
    {
        isEvasionable = false;
        evasionalCount_flame = 0;

        Instantiate(effect_Evasion, transform.position, Quaternion.identity);

        _audioSource.PlayOneShot(_clip_Evasion);

        _collider.enabled = false;

        Evasioned.Invoke();

        Time.timeScale *= timeScale_WhenEvasion;

        await UniTask.Delay((int)(0.6f * 1000f * Time.timeScale), cancellationToken: token);

        Time.timeScale /= timeScale_WhenEvasion;

        await UniTask.Delay((int)(0.5f * 1000f * Time.timeScale), cancellationToken: token);

        _collider.enabled = true;
    }
}
