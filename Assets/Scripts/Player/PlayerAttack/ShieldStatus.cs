using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShieldStatus : MonoBehaviour, IDamagable
{
    [SerializeField] GameObject FX_JustGuard;

    bool JustGuardable;

    int count_JustGuardable;

    public UnityEvent _event {  get; private set; } = new UnityEvent();

    private void Start()
    {
        JustGuardable = true;

        count_JustGuardable = 4;
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
        Instantiate(FX_JustGuard, transform.position, transform.rotation);

        _event.Invoke();
    }
}
