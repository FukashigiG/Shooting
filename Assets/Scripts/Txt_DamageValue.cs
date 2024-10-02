using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using DG.Tweening;
using TMPro;
using System.Threading;

public class Txt_DamageValue : MonoBehaviour
{
    [SerializeField] float sec_delay;
    [SerializeField] float sec_disappear;

    RectTransform rectTransform;
    TextMeshPro textMeshPro;

    Vector3 defScale;

    readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    CancellationToken _cancellationToken;

    void Start()
    {
        TryGetComponent(out rectTransform);
        TryGetComponent(out textMeshPro);

        _cancellationToken = cancellationTokenSource.Token;

        defScale = rectTransform.localScale;

        rectTransform.localScale = defScale * 0.1f;

        rectTransform.DOScale(defScale, 0.05f);

        TxtAnim(_cancellationToken).Forget();
    }

    async UniTask TxtAnim(CancellationToken token)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(sec_delay), cancellationToken: token);

        rectTransform.DOMoveY(rectTransform.position.y + 10, 0.1f);
        await rectTransform.DOScale(defScale * 0.4f, 0.1f).ToUniTask(cancellationToken: token);

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
    }
}
