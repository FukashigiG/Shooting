using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class Bullet_MikeController : Base_BulletController
{
    float defSpeed;

    Vector3 defScale;

    [SerializeField] float ratio_SpeedBoost;

    protected override void Start()
    {
        base.Start();

        collider2d.enabled = false;

        defSpeed = speed;
        speed = 0f;

        defScale = transform.localScale;
        transform.localScale = defScale * 0.1f;

        transform.DOScale(defScale, 0.1f);
    }

    public void SetColor(float ratio)
    {
        TryGetComponent(out SpriteRenderer renderer);
        renderer.color = Color.HSVToRGB(ratio, 0.6f, 1);

        var colorKey = new GradientColorKey[2];
        colorKey[0].color = Color.HSVToRGB(ratio, 0.6f, 1);
        colorKey[0].time = 0f;
        colorKey[1].color = Color.HSVToRGB(ratio, 0.6f, 1);
        colorKey[1].time = 1f;

        var alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.5f;
        alphaKey[1].alpha = 0.0f;
        alphaKey[1].time = 1.0f;

        var _gradient = new Gradient();
        _gradient.SetKeys(colorKey, alphaKey);

        if(trailRenderer == null) TryGetComponent(out trailRenderer);
        trailRenderer.colorGradient = _gradient;
    }

    async public UniTask Activation()
    {
        var token = _cancellationToken;

        await transform.DOMove(transform.position - transform.up * 0.5f, 0.2f).ToUniTask(cancellationToken: token);

        collider2d.enabled = true;

        speed = defSpeed * ratio_SpeedBoost;
    }

    protected override void Update()
    {
        base.Update();

        if (speed > defSpeed) speed -= Time.deltaTime * (defSpeed * (ratio_SpeedBoost - 1)) * 1.5f;
    }
}
