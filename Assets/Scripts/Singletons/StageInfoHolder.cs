using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageInfoHolder : SingletonMonoDontDestroy<StageInfoHolder>
{
    protected override bool dontDestroyOnLoad { get { return true; } }

    public int stageID { get; private set; }
    public int weaponID { get; private set; }

    public void SetID(int x, int y)
    {
        stageID = x;
        weaponID = y;
    }

    public void DestroyThis()
    {
        Destroy(this.gameObject);
    }
}
