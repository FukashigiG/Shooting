using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowCtrler : MonoBehaviour
{
    public void OnAnimationFinished()
    {
        this.gameObject.SetActive(false);
    }

}
