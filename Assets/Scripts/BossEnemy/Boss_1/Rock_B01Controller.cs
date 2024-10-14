using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock_B01Controller : MonoBehaviour, Projectile
{
    [SerializeField] float power;
    [SerializeField] float chance_Critical;

    [SerializeField] GameObject effect_Hit;

    [SerializeField] float def_RotateSpeed;
    float rotateSpeed;

    [SerializeField] AudioClip SE_Hit;

    private void Start()
    {
        rotateSpeed = def_RotateSpeed * UnityEngine.Random.Range(0.4f, 1.6f);
    }

    private void Update()
    {
        transform.Rotate(new Vector3(0, 0, rotateSpeed * Time.deltaTime));

        if(transform.position.y <= -12f) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out IDamagable ID) && collision.CompareTag("Player"))
        {
            if (! ID.Damage(power, transform.position, JudgeCritical())) return;

            Hit(collision.gameObject);
        }
    }

    public void Hit(GameObject terget)
    {
        Instantiate(effect_Hit, transform.position, Quaternion.identity);
        AudioSource.PlayClipAtPoint(SE_Hit, transform.position);
    }

    bool JudgeCritical()
    {
        float x = UnityEngine.Random.Range(0, 100);

        bool isCritical = false;

        if(chance_Critical >= x) isCritical = true;

        return isCritical;
    }
}
