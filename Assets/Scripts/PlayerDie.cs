using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerDie : MonoBehaviour
{
    [Header("Reference")]
    public PlayerMove playerMove;
    public Animator animator;
    public DistanceShow distanceShow;

    [Header("State")]
    public bool isDead = false;
    public bool isProtect = false;
    public bool isDelete = false;
    public bool isWin = false;

    [Header("Protect")]
    public int protectTime = 10;
    public GameObject protectEffect;

    [Header("Die")]
    public GameObject dieEffect;

    [Header("Delete Around")]
    public float outerRadius = 10f;
    public float innerRadius = 5f;

    [Header("UI")]
    public Image image;
    public GameObject dieShow;

    Coroutine protectCoroutine;
    Coroutine uiCoroutine;

    [Header("Hearts")]
    public bool isShock = false;
    public int maxHearts = 3;
    public int currentHearts;
    public PlayerHeart playerHeart;
    public Image image2;
    public TextMeshProUGUI text;
    public GameObject heartShow;
    public float popUpDuration;
    public float popUpHeight;
    Vector3 heartStartPos;
    Coroutine popupCoroutine;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip protectClip;
    public AudioClip hitClip;

    void Start()
    {
        currentHearts = maxHearts;
        heartStartPos = heartShow.transform.localPosition;
        heartShow.SetActive(false);
    }

    void Update()
    {
        if (isWin) return;
        if (distanceShow == null) return;

        if (!isDelete && distanceShow.progress >= 190f)
        {
            DeleteObjectsAround(innerRadius, outerRadius);
            isDelete = true;
        }
    }

    // ================= DIE =================
    public void TakeDamage(int damage)
    {
        if (isDead || isProtect || isWin) return;

        // Phát âm thanh khi bị trúng đòn
        audioSource?.PlayOneShot(hitClip);

        currentHearts -= damage;
        if (currentHearts <= 0)
        {
            Die();
        }
        bool isMinus = false;
        if (playerHeart != null && !isMinus)
        {
            isShock = true;
            isMinus = true;
            playerHeart.UpdateHearts(currentHearts);
            StartCoroutine(WaitShock());
        }
        PopUpAndFade();
    }

    void PopUpAndFade()
    {
        if (popupCoroutine != null)
            StopCoroutine(popupCoroutine);

        // RESET CỨNG về vị trí gốc
        heartShow.transform.localPosition = heartStartPos;

        // RESET màu
        Color cText = text.color;
        text.color = new Color(cText.r, cText.g, cText.b, 1);

        Color cImg = image2.color;
        image2.color = new Color(cImg.r, cImg.g, cImg.b, 1);

        heartShow.SetActive(true);
        popupCoroutine = StartCoroutine(PopUpAndFadeCoroutine());
    }

    IEnumerator PopUpAndFadeCoroutine()
    {
        Vector3 endPos = heartStartPos + new Vector3(0, popUpHeight, 0);

        float elapsed = 0f;

        Color textStartColor = text.color;
        Color textEndColor = new Color(textStartColor.r, textStartColor.g, textStartColor.b, 0);

        Color imgStartColor = image2.color;
        Color imgEndColor = new Color(imgStartColor.r, imgStartColor.g, imgStartColor.b, 0);

        while (elapsed < popUpDuration)
        {
            float t = elapsed / popUpDuration;

            heartShow.transform.localPosition =
                Vector3.Lerp(heartStartPos, endPos, t);

            text.color = Color.Lerp(textStartColor, textEndColor, t);
            image2.color = Color.Lerp(imgStartColor, imgEndColor, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        heartShow.SetActive(false);
    }

    IEnumerator WaitShock()
    {
        animator.SetBool("isShock", true);
        yield return new WaitForSeconds(1.5f);
        animator.SetBool("isShock", false);
    }
    public void Die()
    {
        if (isDead || isProtect || isWin) return;

        isDead = true;
        playerMove.canMove = false;

        dieEffect.SetActive(true);
        animator?.SetBool("isDie", true);
    }

    public void Revive()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 20f);
        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("ShortBarrier") || hit.CompareTag("TallBarrier"))
                Destroy(hit.gameObject);
        }

        isDead = false;
        playerMove.canMove = true;

        dieEffect.SetActive(false);

        bool isAdd = false;
        if (playerHeart != null && !isAdd)
        {
            isAdd = true;
            currentHearts += 1;
            playerHeart.UpdateHearts(currentHearts);
        }
        animator?.SetBool("isDie", false);
    }

    // ================= PROTECT =================
    public void Protect()
    {
        // 🔥 reset protect nếu đang có
        if (protectCoroutine != null)
            StopCoroutine(protectCoroutine);

        if (uiCoroutine != null)
            StopCoroutine(uiCoroutine);

        protectCoroutine = StartCoroutine(ProtectionRoutine());
        uiCoroutine = StartCoroutine(TimeShowCoroutine(protectTime));
    }

    IEnumerator ProtectionRoutine()
    {
        // phát nhạc
        audioSource?.PlayOneShot(protectClip);

        isProtect = true;
        protectEffect.SetActive(true);

        yield return new WaitForSeconds(protectTime);

        isProtect = false;
        protectEffect.SetActive(false);
    }

    // ================= COLLISION =================
    void OnTriggerEnter(Collider other)
    {
        if (isWin || isDead) return;

        if (other.CompareTag("ShortBarrier") || other.CompareTag("TallBarrier"))
        {
            if(!isProtect)
            {
                TakeDamage(1);
            }
            else
            {
                BreakProtect();
                //Xóa chướng ngại vật
                Destroy(other.gameObject);
            }
        }
    }

    // ================= DELETE OBJECT =================
    void DeleteObjectsAround(float innerRadius, float outerRadius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, outerRadius);

        foreach (var hit in hitColliders)
        {
            if (!hit.CompareTag("Barrier") &&
                !hit.CompareTag("Lixi") &&
                !hit.CompareTag("Item"))
                continue;

            float distance = Vector3.Distance(transform.position, hit.transform.position);

            if (distance >= innerRadius && distance <= outerRadius)
                Destroy(hit.gameObject);
        }
    }

    // ================= WIN =================
    public void Win()
    {
        if (isWin) return;

        isWin = true;
        playerMove.canMove = true;
        playerMove.isAutoJump = true;
    }

    // ================= UI TIME =================
    IEnumerator TimeShowCoroutine(float time)
    {
        dieShow.SetActive(true);
        float timeLeft = time;

        while (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            image.fillAmount = timeLeft / time;
            yield return null;
        }

        dieShow.SetActive(false);
    }

    // ================= GIZMOS =================
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, outerRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, innerRadius);
    }

    public void BreakProtect()
    {
        if (!isProtect) return;

        // Stop coroutine protect
        if (protectCoroutine != null)
        {
            StopCoroutine(protectCoroutine);
            protectCoroutine = null;
        }

        // Stop coroutine UI time
        if (uiCoroutine != null)
        {
            StopCoroutine(uiCoroutine);
            uiCoroutine = null;
        }

        // 🔥 RESET & ẨN UI NGAY
        image.fillAmount = 0f;
        dieShow.SetActive(false);

        // Tắt effect bảo vệ
        protectEffect.SetActive(false);
        StartCoroutine(WaitNotProtect());
    }

    IEnumerator WaitNotProtect()
    {
        yield return new WaitForSeconds(0.5f);
        isProtect = false;
    }
}
