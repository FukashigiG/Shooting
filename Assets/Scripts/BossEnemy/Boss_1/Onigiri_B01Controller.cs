using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Onigiri_B01Controller : MonoBehaviour
{
    [SerializeField] float power;

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

        if (transform.position.y <= -12f) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerStatus _ps))
        {
            _ps.Recover(power, transform.position);

            Instantiate(effect_Hit, transform.position, Quaternion.identity);
            AudioSource.PlayClipAtPoint(SE_Hit, transform.position);

            Destroy(gameObject);
        }
    }
}
