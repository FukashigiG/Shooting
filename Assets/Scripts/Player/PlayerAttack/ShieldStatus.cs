using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldStatus : MonoBehaviour, IDamagable
{
    [SerializeField] GameObject FX_JustGuard;

    bool JustGuardable;

    int count_JustGuardable;

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
    }
}
