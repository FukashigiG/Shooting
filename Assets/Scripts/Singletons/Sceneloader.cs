using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class Sceneloader : SingletonMonoDontDestroy<Sceneloader>
{
    protected override bool dontDestroyOnLoad { get { return true; } }

    public async void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;

        await SceneManager.LoadSceneAsync(sceneName);
    }
}
