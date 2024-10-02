using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;

public class Impuct : MonoBehaviour
{
    [SerializeField] float Power;
    [SerializeField] float chance_Critical;
    [SerializeField] float survivalTime;
    [SerializeField] GameObject hitEffect;

    List<GameObject> hits = new List<GameObject>();

    readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    CancellationToken _cancellationToken;

    private void Start()
    {
        _cancellationToken = cancellationTokenSource.Token;

        WaitAndDestroy(_cancellationToken).Forget();
    }

    async UniTask WaitAndDestroy(CancellationToken token)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(survivalTime), cancellationToken: token);

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hits.Contains(collision.gameObject)) return;

        if(collision.TryGetComponent(out IDamagable ID))
        {
            ID.Damage(Power, transform.position, JudgeCritical());

            if(hitEffect != null) Instantiate(hitEffect, collision.ClosestPoint(this.transform.position), Quaternion.identity);

            hits.Add(collision.gameObject);
        }
    }

    bool JudgeCritical()
    {
        bool x = false;

        float y = UnityEngine.Random.Range(0, 100);

        if (chance_Critical >= y) x = true;

        return x;
    }

    private void OnDestroy()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
    }
}
