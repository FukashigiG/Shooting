using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;

public class BGController : MonoBehaviour
{
    readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    CancellationToken _cancellationToken;

    GameDirector _director;

    void Start()
    {
        _cancellationToken = cancellationTokenSource.Token;

        Spining(_cancellationToken).Forget();
    }


    async UniTask Spining(CancellationToken token)
    {
        await transform.DORotate(Vector3.forward * 360, 60, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Incremental) .SetEase(Ease.InOutSine).ToUniTask(cancellationToken: token);
    }

    private void OnDestroy()
    {
        StopAction();
    }

    public void StopAction()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
    }


}
