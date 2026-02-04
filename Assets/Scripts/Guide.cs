using UnityEngine;
using System.Collections.Generic;

public class Guide : MonoBehaviour
{
    [Header("Guide Objects")]
    public List<GameObject> guides = new List<GameObject>();

    [Header("Index")]
    public int currentIndex = 0;

    [Header("Option")]
    public bool loop = false; // có quay vòng hay không

    void Start()
    {
        ShowGuide(currentIndex);
    }

    void ShowGuide(int index)
    {
        if (guides.Count == 0) return;

        // Tắt tất cả
        for (int i = 0; i < guides.Count; i++)
        {
            guides[i].SetActive(false);
        }

        // Bật guide hiện tại
        guides[index].SetActive(true);
    }

    // ================= BUTTON =================

    public void Next()
    {
        if (guides.Count == 0) return;

        currentIndex++;

        if (currentIndex >= guides.Count)
        {
            currentIndex = loop ? 0 : guides.Count - 1;
        }

        ShowGuide(currentIndex);
    }

    public void Prev()
    {
        if (guides.Count == 0) return;

        currentIndex--;

        if (currentIndex < 0)
        {
            currentIndex = loop ? guides.Count - 1 : 0;
        }

        ShowGuide(currentIndex);
    }

    public void CloseAll()
    {
        foreach (var g in guides)
            g.SetActive(false);
    }
}
