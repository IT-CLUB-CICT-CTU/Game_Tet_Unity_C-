using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DistanceShow : MonoBehaviour
{
    public PlayerMove player;
    public PlayerDie playerDie;
    public TextMeshProUGUI number;
    public float progress;

    void Start()
    {
        progress = 0f;
        Calculate();
    }

    void Update()
    {
        if (!playerDie.isDead)
        {
            Calculate();
        }
    }
    void Show()
    {
        number.text = progress.ToString("F2") + "/m";
    }

    void Calculate()
    {
        progress += (player.forwardSpeed / 10f) * Time.deltaTime;
        Show();
    }
}
