using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class LineBetweenTwoPoints : MonoBehaviour
{
    [Header("Blink")]
    public float indexTime;
    public float speedBlink;

    [Header("Points")]
    public Transform pointA;
    public Transform pointB;

    private LineRenderer lr;
    private Coroutine blinkCoroutine;

    [Header("Attack")]
    public GameObject weaponPrefab;
    public Transform firePoint;
    public float attackSpeed;

    // ===================== SWEEP CONFIG =====================
    [Header("Sweep X")]
    public float minX = -3f;
    public float maxX = 3f;
    public float sweepSpeed = 2f;
    public float sweepDuration = 2f;

    private float sweepTimer;
    private bool sweeping;
    private int direction;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.useWorldSpace = true;
    }

    void OnEnable()
    {
        // 🔥 RESET TOÀN BỘ TRẠNG THÁI MỖI LẦN SPAWN
        indexTime = Random.Range(4f, 7f);
        sweepTimer = indexTime + 0.5f;
        sweeping = true;
        direction = 1;

        StartBlink(indexTime);
    }

    void Update()
    {
        if (pointA == null || pointB == null) return;

        if (sweeping)
        {
            SweepX();
        }

        lr.SetPosition(0, pointA.position);
        lr.SetPosition(1, pointB.position);
    }

    // ===================== SWEEP =====================
    void SweepX()
    {
        sweepTimer -= Time.deltaTime;

        Vector3 pos = transform.position;
        if (sweepTimer <= 2f) 
            pos.x += 0f * direction * Time.deltaTime;
        else
            pos.x += sweepSpeed * direction * Time.deltaTime;

        if (pos.x >= maxX)
        {
            pos.x = maxX;
            direction = -1;
        }
        else if (pos.x <= minX)
        {
            pos.x = minX;
            direction = 1;
        }

        transform.position = pos;

        if (sweepTimer <= 0f)
        {
            sweeping = false;
            StopBlink();
            Fire();
        }
    }

    // ===================== FIRE =====================
    void Fire()
    {
        if (weaponPrefab == null || firePoint == null) return;

        Vector3 dir = (pointB.position - pointA.position).normalized;

        GameObject bullet = Instantiate(
            weaponPrefab,
            firePoint.position,
            Quaternion.LookRotation(dir)
        );

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = dir * attackSpeed;
        }
        this.gameObject.SetActive(false);
    }

    // ===================== BLINK =====================
    public void StartBlink(float t)
    {
        StopBlink();
        if (t <= 0f) t = 0.1f;
        blinkCoroutine = StartCoroutine(BlinkAlphaRoutine(t));
    }

    public void StopBlink()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }

        SetAlpha(1f);
    }

    IEnumerator BlinkAlphaRoutine(float indexTime)
    {
        float t = 0f;

        while (true)
        {
            t += Time.deltaTime * speedBlink;
            float alpha = (Mathf.Sin(t / indexTime * Mathf.PI * 2f) + 1f) / 2f;
            SetAlpha(alpha);
            yield return null;
        }
    }

    void SetAlpha(float alpha)
    {
        Color c = lr.startColor;
        c.a = alpha;
        lr.startColor = c;
        lr.endColor = c;
    }
}
