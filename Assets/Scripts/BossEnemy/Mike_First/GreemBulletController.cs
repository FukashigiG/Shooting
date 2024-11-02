using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GreemBulletController : Base_BulletController
{
    [SerializeField] float rotateSpeed;

    Vector2 moveDirection;

    protected override void Start()
    {
        base.Start();

        moveDirection = transform.up;

        transform.Rotate(0, 0, UnityEngine.Random.Range(0, 180));
    }

    protected override void Update()
    {
        transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);

        base.Update();
    }

    protected override void MoveOn()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall") && isHittableToWall == false) return;

        if (!collision.TryGetComponent(out IDamagable ID)) return;
        if (!ID.Recover(power, transform.position)) return;


        Instantiate(hitEffect, transform.position, Quaternion.identity);
        if (! collision.CompareTag("Wall")) AudioSource.PlayClipAtPoint(SE_Hit, (Vector2)transform.position);

        speed = 0;

        collider2d.enabled = false;

        BeSmallAndDie().Forget();
    }
}
