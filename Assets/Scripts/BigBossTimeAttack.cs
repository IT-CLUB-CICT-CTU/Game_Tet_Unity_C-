using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BigBossAttack : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text timeText;
    public Image timeFillImage;

    [Header("Attack Config")]
    public float attackDuration = 10f, elapsedTime = 0f;
    public float delayBeforeAttack = 7f;

    [Header("Boss State")]
    public bool isEnded = false;
    public bool isReadyAtk = false;

    public PlayerDie playerDie;

    [Header("Line Attacks")]
    public List<GameObject> lineRenderers = new List<GameObject>();

    public bool isAttacking = false;

    void Start()
    {
        if (playerDie == null)
            playerDie = FindObjectOfType<PlayerDie>();

        StartCoroutine(AttackCoroutine());
    }

    void Update()
    {
        if (!isReadyAtk || isEnded || isAttacking) return;

        CheckState();
    }

    // ===================== TIME CONTROL =====================
    IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(delayBeforeAttack);

        isReadyAtk = true;

        elapsedTime = 0f;

        while (elapsedTime < attackDuration)
        {
            elapsedTime += Time.deltaTime;

            float remaining = attackDuration - elapsedTime;
            float ratio = remaining / attackDuration;

            timeText.text = Mathf.CeilToInt(remaining) + "/" + attackDuration;
            timeFillImage.fillAmount = ratio;

            yield return null;
        }

        EndAttack();
    }

    void EndAttack()
    {
        isEnded = true;
        timeText.text = "0";
        timeFillImage.fillAmount = 0f;

        foreach (var line in lineRenderers)
        {
            if (line.activeSelf)
            {
                line.SetActive(false);
            }
        }
        playerDie?.Win();
    }

    // ===================== STATE CHECK =====================
    void CheckState()
    {
        float ratio = elapsedTime / attackDuration;

        if (ratio <= 0.5f)
        {
                SetState(1);
        }
        else if (ratio <= 0.75f)
        {
                SetState(2);
        }
        else
        {
                SetState(3);
        }
    }


    void SetState(int state)
    {
        if (isAttacking) return;

        Debug.Log("Activating State " + state);
        StartCoroutine(ActiveLine(state));
    }

    // ===================== ACTIVATE LINE =====================
    IEnumerator ActiveLine(int amount)
    {
        isAttacking = true;

        yield return new WaitForSeconds(2f);

        for (int i = 0; i < amount && i < lineRenderers.Count; i++)
        {
            if (!lineRenderers[i].activeSelf)
            {
                lineRenderers[i].SetActive(true);
            }

            yield return new WaitForSeconds(0.4f);
        }

        isAttacking = false;
    }
}
