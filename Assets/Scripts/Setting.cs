using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setting : MonoBehaviour
{
    public static Setting Instance;
    public GameObject board;
    public bool isPause = false;

    void Awake()
    {
        Instance = this;
    }

    public void OpenSetting()
    {
        if (isPause) return;

        Time.timeScale = 0f;

        board.SetActive(true); // OnEnable chạy
        isPause = true;
    }

    public void CloseSetting()
    {
        Time.timeScale = 1f;

        board.SetActive(false);
        AudioListener.volume = 1f;
        isPause = false;
    }

}
