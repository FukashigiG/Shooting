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

    public bool reloadable { get; private set; }

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

    public void SetTxt(float x, Vector2 posi)
    {
        transform.position = posi;

        TMP.text = x.ToString();

        cancellationTokenSource.Cancel();

        cancellationTokenSource = new CancellationTokenSource();

        TxtAnim(cancellationTokenSource.Token).Forget();
    }

    async UniTask TxtAnim(CancellationToken token)
    {
        reloadable = true;

        await UniTask.Delay(TimeSpan.FromSeconds(sec_delay), cancellationToken: token);

        reloadable = false;

        await rectTransform.DOScale(defScale * 0.4f, 0.1f).ToUniTask(cancellationToken: token);

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
    }
}
