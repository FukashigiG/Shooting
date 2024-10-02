using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RollingBullet : Base_BulletController
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
}
