using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMissileCtrl : Base_BulletController
{
    GameObject terget;

    [SerializeField] float def_RotateSpeed;
    [SerializeField] float time_freezeRotate;
    [SerializeField] float length_Ray_SearchTatget;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] GameObject bomb;

    float rotateSpeed;

    float math_x, math_y;

    protected override void Start()
    {
        base.Start();

        rotateSpeed = def_RotateSpeed;

        math_x = 0;
        math_y = 0;
    }

    protected override void Update()
    {
        base.Update();

        if(terget != null)
        {
            math_x += Time.deltaTime * 2f;

            math_y = -1 * Mathf.Pow((math_x - 1f), 2) + 1f;

            math_y = Mathf.Clamp01(math_y);

            rotateSpeed = def_RotateSpeed * math_y;

            if (rotateSpeed > 0)
            {
                int x;

                if (TergetDirection() >= Mathf.Repeat(transform.localEulerAngles.z, 360))
                {
                    if (TergetDirection() - Mathf.Repeat(transform.localEulerAngles.z, 360) >= 180)
                    {
                        x = -1;
                    }
                    else
                    {
                        x = 1;
                    }
                }
                else
                {
                    if (Mathf.Repeat(transform.localEulerAngles.z, 360) - TergetDirection() >= 180)
                    {
                        x = 1;
                    }
                    else
                    {
                        x = -1;
                    }
                }

                transform.Rotate(0, 0, rotateSpeed * x * Time.deltaTime);
            }
        }
    }

    float TergetDirection()
    {
        float x = (Mathf.Atan2(transform.position.y - terget.transform.position.y, transform.position.x - terget.transform.position.x) * Mathf.Rad2Deg) + 90; ;

        x = Mathf.Repeat(x, 360);

        return x;
    }

    public void SetTerget(GameObject obj)
    {
        terget = obj;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall") && isHittableToWall == false) return;

        if (!collision.TryGetComponent(out IDamagable ID)) return;

        Instantiate(bomb, transform.position, Quaternion.identity);

        AudioSource.PlayClipAtPoint(SE_Hit, (Vector2)transform.position);

        speed = 0;

        collider2d.enabled = false;

        BeSmallAndDie(_cancellationToken).Forget();
    }
}
