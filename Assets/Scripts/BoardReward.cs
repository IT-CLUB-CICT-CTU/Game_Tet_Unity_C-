using System.Collections;
using UnityEngine;
using TMPro;

public class BoardReward : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI countReward;
    public TextMeshProUGUI rewardText;
    public GameObject[] continueButton;

    [Header("Reference")]
    public LixiCountShow lixiCountShow;
    public DistanceShow distanceShow;

    [Header("Effect")]
    public AudioSource audioSource;
    public AudioClip tickSound;
    public PlayerDie playerDie;

    public float decreaseDuration = 1.5f;
    public float scaleUp = 1.2f;
    public float shakeStrength = 5f;

    Vector3 originalScale;
    Vector3 originalPos;
    Color originalColor;

    void Awake()
    {
        originalScale = countReward.transform.localScale;
        originalPos = countReward.transform.localPosition;
        originalColor = countReward.color;

        if (lixiCountShow != null)
            ShowReward(lixiCountShow.count.Get());

        if (distanceShow != null &&
            distanceShow.progress >= 200f &&
            !playerDie.isWin)
        {
            rewardText.text = "ĐỂ MẸ GIỮ PHÂN NỬA CHO";

            int bonus = lixiCountShow.count.Get() / 2;
            StartCoroutine(Degrease(bonus));
        }
    }

    public void ShowReward(int count)
    {
        if (count < 0)
        {
            countReward.text = count + "K";
        }
        else countReward.text = "+" + count + "K";
    }

    IEnumerator Degrease(int decreaseAmount)
    {
        foreach (var btn in continueButton)
            btn.SetActive(false);

        yield return new WaitForSecondsRealtime(1f);

        int start = lixiCountShow.count.Get();
        int end = start - decreaseAmount;

        countReward.color = Color.red;
        float time = 0f;

        while (time < decreaseDuration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / decreaseDuration;

            int value = Mathf.RoundToInt(Mathf.Lerp(start, end, t));
            ShowReward(value);

            // Scale punch
            countReward.transform.localScale =
                Vector3.Lerp(originalScale * scaleUp, originalScale, t);

            // Shake
            countReward.transform.localPosition =
                originalPos + Random.insideUnitSphere * shakeStrength;

            // Tick sound
            if (audioSource && tickSound && Random.value < 0.15f)
                audioSource.PlayOneShot(tickSound);

            yield return null;
        }

        // Reset UI
        ShowReward(end);
        countReward.transform.localScale = originalScale;
        countReward.transform.localPosition = originalPos;
        countReward.color = originalColor;

        // 🔥 SET LẠI BẰNG SecureInt (KHÔNG GÁN TRỰC TIẾP)
        lixiCountShow.count.Set(end);

        foreach (var btn in continueButton)
            btn.SetActive(true);
    }
}
