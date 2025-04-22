using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullBullet : Base_BulletController
{
    [SerializeField] float power_Pull;

    Rigidbody2D pullTerget;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Rigidbody2D _rb)) _rb.AddForce(transform.up * 100 * -1, ForceMode2D.Impulse);

        base.OnTriggerEnter2D(collision);
    }

}
