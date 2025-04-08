using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMDirector : SingletonMonoDontDestroy<BGMDirector>
{
    protected override bool dontDestroyOnLoad { get { return true; } }

    AudioSource _audioSource;

    protected override void Awake()
    {
        base.Awake();

        if (_audioSource == null) TryGetComponent(out _audioSource);
    }

    public void Play()
    {

    }
}
