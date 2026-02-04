using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    private PlayerDie player;

    [Header("Follow")]
    public Vector3 offset;
    private Vector3 originalOffset;
    public float followSpeed = 10f;
    public Vector3 defaultState;
    public bool isFixedX = false;

    [Header("Distance")]
    public DistanceShow distanceShow;
    public bool isOffsetMoving = false;

    [Header("Shake")]
    public float shakeDuration = 0.3f;
    public float shakeStrength = 0.8f; // 🔥 tăng mạnh

    Vector3 shakeOffset;
    Coroutine shakeCoroutine;

    void Start()
    {
        originalOffset = offset;
    }

    void LateUpdate()
    {
        if (target == null)
        {
            GameObject obj = GameObject.FindWithTag("Player");
            if (obj != null) target = obj.transform;
            else
            {
                transform.position = defaultState;
                return;
            }
        }

        if (distanceShow == null)
            distanceShow = FindObjectOfType<DistanceShow>();

        player = target.GetComponent<PlayerDie>();
        if (player != null && player.isWin)
            return;

        if (player != null && player.isShock)
        {
            player.isShock = false;
            ShakeCamera();
        }

        if (distanceShow != null && distanceShow.progress >= 200f && !isOffsetMoving)
            MoveOffset();

        // ===== FOLLOW (LERP) =====
        Vector3 followPos = target.position + offset;

        if (isFixedX)
            followPos.x = transform.position.x;

        transform.position = Vector3.Lerp(
            transform.position,
            followPos,
            followSpeed * Time.deltaTime
        );

        // ===== APPLY SHAKE SAU FOLLOW =====
        transform.position += shakeOffset;
    }

    // ================= OFFSET =================
    void MoveOffset()
    {
        if (!isOffsetMoving)
            StartCoroutine(OffsetRoutine());
    }

    IEnumerator OffsetRoutine()
    {
        isOffsetMoving = true;

        Vector3 zoomOut = originalOffset + new Vector3(0, 0, -17f);
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime;
            offset = Vector3.Lerp(originalOffset, zoomOut, t);
            yield return null;
        }

        yield return new WaitForSeconds(3f);

        t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime;
            offset = Vector3.Lerp(zoomOut, originalOffset, t);
            yield return null;
        }
    }

    // ================= SHAKE =================
    void ShakeCamera()
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(ShakeRoutine());
    }

    IEnumerator ShakeRoutine()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float strength = Mathf.Lerp(shakeStrength, 0f, elapsed / shakeDuration);

            shakeOffset = new Vector3(
                Random.Range(-1f, 1f) * strength,
                Random.Range(-1f, 1f) * strength,
                0f
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        shakeOffset = Vector3.zero;
    }
}
