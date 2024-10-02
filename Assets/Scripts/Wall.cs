using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour, IDamagable
{
    public bool Damage(float x, Vector3 y, bool isCritical)
    {
        return true;
    }

    public bool Recover(float x, Vector3 y)
    {
        return true;
    }
}
