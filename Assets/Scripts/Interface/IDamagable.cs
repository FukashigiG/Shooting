using UnityEngine;

public interface IDamagable
{
    bool Damage(float damage, Vector3 damagedPosi, bool isCritical);

    bool Recover(float damage, Vector3 posi);
}
