using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandMineBulletCtrl : Base_BulletController
{
    GameObject terget;

    [SerializeField] float def_RotateSpeed;
    [SerializeField] float num_Bomb_PerSec;
    [SerializeField] float lifeTime;
    [SerializeField] GameObject bomb;

    float timeCount = 0;
    float lifeCount = 0;

    float rotateSpeed;

    float math_x, math_y;

    bool onLiving = true;

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

        timeCount += Time.deltaTime;
        lifeCount += Time.deltaTime;

        if(timeCount > 1 / num_Bomb_PerSec)
        {
            timeCount = 0;

            Instantiate(bomb, transform.position, Quaternion.identity);
        }

        if(lifeCount > lifeTime && onLiving)
        {
            onLiving = false;

            BeSmallAndDie().Forget();
        }

        if (terget != null)
        {
            math_x += Time.deltaTime;

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

    }
}
