using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam_Director : SingletonMono<Cam_Director>
{
    [SerializeField] GameObject cam_Def;
    [SerializeField] GameObject cam_OnlyPlayer;
    [SerializeField] GameObject cam_Wide;


    void Start()
    {
        
    }

    public void SetCam_Def()
    {
        cam_Def.SetActive(true);
        cam_OnlyPlayer.SetActive(false);
        cam_Wide.SetActive(false);
    }

    public void SetCam_FollowPlayer()
    {
        cam_Def.SetActive(false);
        cam_OnlyPlayer.SetActive(true);
        cam_Wide.SetActive(false);
    }

    public void SetCam_Wide()
    {
        cam_Def.SetActive(false);
        cam_OnlyPlayer.SetActive(false);
        cam_Wide.SetActive(true);
    }
}
