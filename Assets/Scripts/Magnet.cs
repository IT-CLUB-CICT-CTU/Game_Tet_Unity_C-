using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Magnet : MonoBehaviour
{
    [Header("Config")]
    public string targetTag = "Lixi";
    public float radius = 5f;
    public Transform point;
    public GameObject magnetVisual;
    public float timeUse = 10f;

    [Header("UI")]
    public Image progress;
    public GameObject showUI;

    bool isPoweredOn;

    Coroutine magnetCoroutine;
    Coroutine uiCoroutine;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip magnetClip;

    void FixedUpdate()
    {
        if (!isPoweredOn)
        {
            magnetVisual.SetActive(false);
            return;
        }

        magnetVisual.SetActive(true);

        Collider[] hits = Physics.OverlapSphere(point.position, radius);
        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag(targetTag)) continue;

            LixiMagnet lm = hit.GetComponent<LixiMagnet>();
            if (lm != null)
                lm.AttachToMagnet(point);
        }
    }

    // ================= MAGNET POWER =================
    public void PowerOnMagnet()
    {
        // 🔥 Phát âm thanh khi kích hoạt nam châm
        if (audioSource != null && magnetClip != null)
        {
            audioSource.PlayOneShot(magnetClip);
        }
        // 🔥 Reset magnet nếu đang hoạt động
        if (magnetCoroutine != null)
            StopCoroutine(magnetCoroutine);

        if (uiCoroutine != null)
            StopCoroutine(uiCoroutine);

        magnetCoroutine = StartCoroutine(MagnetRoutine());
        uiCoroutine = StartCoroutine(MagnetUIRoutine(timeUse));
    }

    IEnumerator MagnetRoutine()
    {
        isPoweredOn = true;
        magnetVisual.SetActive(true);

        yield return new WaitForSeconds(timeUse);

        isPoweredOn = false;
        magnetVisual.SetActive(false);
    }

    // ================= UI TIME =================
    IEnumerator MagnetUIRoutine(float time)
    {
        showUI.SetActive(true);
        float timeLeft = time;

        while (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            progress.fillAmount = timeLeft / time;
            yield return null;
        }

        progress.fillAmount = 0f;
        showUI.SetActive(false);
    }

    // ================= GIZMO =================
    void OnDrawGizmosSelected()
    {
        if (point == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(point.position, radius);
    }
}
