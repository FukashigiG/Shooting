using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class ShieldStatus : MonoBehaviour, IDamagable
{
    [SerializeField] float timeScale_WhenJustGuard;

    [SerializeField] GameObject FX_JustGuard;

    bool JustGuardable;

    int count_JustGuardable;

    public UnityEvent _event_JustGuard {  get; private set; } = new UnityEvent();

    readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    CancellationToken _cancellationToken;


    private void Start()
    {
        JustGuardable = true;

        count_JustGuardable = 4;

        _cancellationToken = _cancellationTokenSource.Token;
    }

    private void Update()
    {
        if (count_JustGuardable > 0)
        {
            count_JustGuardable -= 1;

            if (count_JustGuardable <= 0) JustGuardable = false;
        }

        
    }

    public bool Damage(float x, Vector3 y, bool isCritical)
    {
        if (JustGuardable)
        {
            JustGuard();
        }

        return true;
    }

    public bool Recover(float x, Vector3 y)
    {
        return false;
    }

    void JustGuard()
    {
        JustGuardable = false;
        count_JustGuardable = 0;

        _event_JustGuard.Invoke();
    }
}
