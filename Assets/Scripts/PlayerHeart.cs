using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeart : MonoBehaviour
{
    public GameObject[] hearts = new GameObject[3];

    void Start()
    {
        UpdateHearts(hearts.Length);
    }

    public void UpdateHearts(int currentHearts)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHearts)
            {
                hearts[i].SetActive(true);
            }
            else
            {
                hearts[i].SetActive(false);
            }
        }
    }
}
