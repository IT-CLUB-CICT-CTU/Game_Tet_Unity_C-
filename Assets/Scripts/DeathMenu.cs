using UnityEngine;
using System.Collections;
using TMPro;

public class DeathMenu : MonoBehaviour
{
    public DistanceShow distanceShow;
    public LixiCountShow lixiCountShow;
    public PlayerDie playerDie;
    public GameObject board, boardReward;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI deadText;
    public TextMeshProUGUI rewardText;

    public int reviveTime = 10;

    float deathTime;
    bool isOpening = false;
    bool isOpeningReward = false;
    bool isTimeout = false;

    void Update()
    {
        if (playerDie.isWin && isOpeningReward == false)
        {
            OpenRewardBoard(); 
        }

        if (playerDie != null && playerDie.isDead && !board.activeSelf && !isOpening)
        {
            OpenDeathMenu();
        }

        if (board.activeSelf)
        {
            UpdateTime();
        }
    }

    void OpenDeathMenu()
    {
        isOpening = true;
        StartCoroutine(WaitDieEffect());
    }

    IEnumerator WaitDieEffect()
    {
        yield return new WaitForSecondsRealtime(2f);

        board.SetActive(true);
        Time.timeScale = 0f;

        deathTime = Time.unscaledTime;
    }

    void UpdateTime()
    {
        int remaining = reviveTime - Mathf.FloorToInt(Time.unscaledTime - deathTime);
        remaining = Mathf.Max(0, remaining);

        if (timeText != null)
            timeText.text = remaining + "s";

        if (remaining <= 0 && !isTimeout)
        {
            isTimeout = true;

            Time.timeScale = 0f;

            boardReward.SetActive(true);
            board.SetActive(false);
        }
    }

    public void Revive()
    {
        isOpening = false;
        isTimeout = false;

        board.SetActive(false);
        Time.timeScale = 1f;

        if (playerDie != null)
            playerDie.Revive();
    }

    public void ShowNewDeadText(string s)
    {
        if (deadText != null)
            deadText.text = s;
    }

    void OpenRewardBoard()
    {
        isOpeningReward = true;
        StartCoroutine(WaitRewardEffect());
    }

    IEnumerator WaitRewardEffect()
    {
        yield return new WaitForSecondsRealtime(3f);
        Time.timeScale = 0f;
        boardReward.SetActive(true);
        rewardText.text = "CHÚC MỪNG TẾT BÍNH NGỌ";
    }
}
