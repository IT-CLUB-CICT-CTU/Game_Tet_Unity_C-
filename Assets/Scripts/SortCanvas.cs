using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortCanvas : MonoBehaviour
{
    public int sortingOrder = 10;

    void Awake()
    {
        Canvas canvas = GetComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = sortingOrder;
    }
}
