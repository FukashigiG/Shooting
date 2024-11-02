using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ExprodingBullet : Base_BulletController
{
    [SerializeField] GameObject childBullet;

    [SerializeField] int num_ChildBullet;
    [SerializeField] float lifeTime;

    bool onLiving = true;

    protected override void Update()
    {
        base.Update();

        lifeTime -= Time.deltaTime;

        if (lifeTime <= 0 & onLiving)
        {
            onLiving = false;

            BeSmallAndDie().Forget();
        }
    }

    protected override async UniTask BeSmallAndDie()
    {
        for (int i =  0; i < num_ChildBullet; i++)
        {
            Instantiate(childBullet, transform.position, Quaternion.Euler(0, 0, 360 / num_ChildBullet * i));
        }

        await base.BeSmallAndDie();
    }
}
