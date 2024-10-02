using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMagicCannon : Base_PlayerAttack
{
    protected override void OnAttackHolded()
    {
        if (onPlay != true) return;
        if (attackable != true) return;

        base.OnAttackHolded();

        Shot();
    }

    void Shot()
    {

    }
}
