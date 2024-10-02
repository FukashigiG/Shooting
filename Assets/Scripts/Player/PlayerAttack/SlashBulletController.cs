using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using Unity.VisualScripting;

public class SlashBulletController : MonoBehaviour
{
    [SerializeField] int num_hit;

    [SerializeField] float power;
    [SerializeField] float sec_Interval_Hit;
    [SerializeField] float chance_Critical;

    [SerializeField] GameObject effect_Hit;

    [SerializeField] AudioClip SE_Souwd;

    IDamagable ID;
    GameObject terget;


    private void Start()
    {
        hit();
    }

    async void hit()
    {
        var token = this.GetCancellationTokenOnDestroy();

        Instantiate(effect_Hit, new Vector3(transform.position.x + R(), transform.position.y + R(), 0), Quaternion.Euler(0, 0, R() * 360));
        AudioSource.PlayClipAtPoint(SE_Souwd, transform.position);

        for (int i  = 0; i < num_hit; i++)
        {
            if (terget != null) ID.Damage(power, transform.position, JudgeCritical());

            await UniTask.Delay(TimeSpan.FromSeconds(sec_Interval_Hit));
        }

        Destroy(gameObject);
    }

    float R()
    {
        return UnityEngine.Random.Range(-0.5f, 0.5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out ID)) terget = collision.gameObject;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject == terget)
        {
            terget = null;
            ID = null;
        }
    }

    bool JudgeCritical()
    {
        bool x = false;

        float y = UnityEngine.Random.Range(0, 100);

        if (chance_Critical >= y) x = true;

        return x;
    }
}
