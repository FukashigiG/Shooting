using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindowCtrler : MonoBehaviour
{

    public void OnAnimationFinished()
    {
        this.gameObject.SetActive(false);
    }

}
