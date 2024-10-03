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
    TextMeshProUGUI TMP;

    Vector3 defScale;

    CancellationTokenSource cancellationTokenSource;
    CancellationToken _cancellationToken;

    private void Awake()
    {
        TryGetComponent(out rectTransform);
        TryGetComponent(out TMP);

        cancellationTokenSource = new CancellationTokenSource();
        _cancellationToken = cancellationTokenSource.Token;
    }

    void Start()
    {
        defScale = rectTransform.localScale;

        rectTransform.localScale = defScale * 0.1f;

        rectTransform.DOScale(defScale, 0.05f);
    }

    public void SetTxt(float x, Vector2 screenPosi)
    {
        var posi = new Vector2(screenPosi.x * UnityEngine.Random.Range(0.96f, 1.04f), screenPosi.y * UnityEngine.Random.Range(0.96f, 1.04f));
        transform.position = posi;

        TMP.text = x.ToString();

        cancellationTokenSource.Cancel();

        cancellationTokenSource = new CancellationTokenSource();

        TxtAnim(cancellationTokenSource.Token).Forget();
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
