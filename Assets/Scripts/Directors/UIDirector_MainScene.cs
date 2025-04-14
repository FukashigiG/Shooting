using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameDirector;

public class UIDirector_MainScene : SingletonMono<UIDirector_MainScene>
{
    [SerializeField] HPGaugeController HPBar_Boss;
    [SerializeField] HPGaugeController HPBar_Player;

    [SerializeField] GameObject panel_Pause;

    void OnPause()
    {
        if (GameDirector.Instance.gamestateEnum != GameStateEnum.onGame) return;

        Time.timeScale *= 0.05f;

        panel_Pause.SetActive(true);
    }

    public void ReleasePause()
    {
        if (GameDirector.Instance.gamestateEnum != GameStateEnum.pause) return;

        Time.timeScale /= 0.05f;

        panel_Pause.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
