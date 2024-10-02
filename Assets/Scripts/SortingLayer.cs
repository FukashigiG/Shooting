using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingLayer : MonoBehaviour
{

    private void Awake()
    {
        SetSortingLayer(transform);
    }

    void SetSortingLayer(Transform parent)
    {
        if (parent.gameObject.TryGetComponent(out Renderer _renderer))
        {
            _renderer.sortingLayerName = "";

            foreach(Transform child in transform)
            {
                SetSortingLayer(child);
            }
        }
    }
}
