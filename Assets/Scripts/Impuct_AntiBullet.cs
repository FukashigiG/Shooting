using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Impuct_AntiBullet : MonoBehaviour
{
    [SerializeField] int flame_LifeTime;

    [SerializeField] GameObject hitEffect;

    List<GameObject> hits = new List<GameObject>();

    void Start()
    {
        
    }

    void Update()
    {
        flame_LifeTime--;

        if(flame_LifeTime <= 0) Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (hits.Contains(collision.gameObject)) return;

        if (collision.TryGetComponent(out Projectile projectile))
        {
            Destroy(collision.gameObject);

            if (hitEffect != null) Instantiate(hitEffect, collision.transform.position, Quaternion.identity);

            hits.Add(collision.gameObject);
        }

        Debug.Log(collision.gameObject);
    }
}
