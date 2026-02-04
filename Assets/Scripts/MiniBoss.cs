using UnityEngine;
using System.Collections;

public class MiniBoss : MonoBehaviour
{
    [Header("Reference")]
    public PlayerMove playerMove;
    public Transform player;
    public Transform stopPoint;
    public int currentPoint;

    [Header("Move")]
    public float enterSpeed = 25f;
    public float zOffset = 15f;
    public float followSmooth = 6f;

    [Header("Attack")]
    public GameObject bullet;
    public Transform firePoint;
    public float bulletSpeed = 12f;
    public float coolDown = 1.5f;
    private float nextShootTime;

    [Header("Shot Mode")]
    public bool doubleShot = false;
    public float spreadAngle = 10f;

    [Header("Aim Prediction")]
    public Vector3 aimOffset;   // 🔥 chỉnh sai số bắn (+ / -)

    public enum BossState { Enter, Fight, Escape }
    public BossState state = BossState.Enter;

    [Header("Sound")]
    public AudioSource shootAudio;
    public AudioClip shootClip;

    void Start()
    {
        Find();
    }

    void Update()
    {
        if (player == null || playerMove == null) return;

        switch (state)
        {
            case BossState.Enter:
                MoveEnter();
                CheckReachStopPoint();
                break;

            case BossState.Fight:
                FollowPlayerZ();
                Attack();
                break;

            case BossState.Escape:
                MoveEscape();
                break;
        }
    }

    // ---------------- FIND ----------------
    void Find()
    {
        if (playerMove == null)
            playerMove = FindObjectOfType<PlayerMove>();

        if (player == null)
        {
            GameObject pl = GameObject.FindWithTag("Player");
            if (pl != null)
                player = pl.transform;
        }
    }

    // ---------------- ENTER ----------------
    void MoveEnter()
    {
        transform.Translate(Vector3.forward * enterSpeed * Time.deltaTime);
    }

    // ---------------- FOLLOW PLAYER ----------------
    void FollowPlayerZ()
    {
        Vector3 targetPos = transform.position;
        targetPos.z = player.position.z + zOffset;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            followSmooth * Time.deltaTime
        );
        //Tăng tốc độ bằng player speed
        float playerSpeed = playerMove.forwardSpeed;
    }

    // ---------------- ESCAPE ----------------
    void MoveEscape()
    {
        transform.Translate(Vector3.forward * enterSpeed * Time.deltaTime);
    }

    // -------- ENTER → FIGHT --------
    void CheckReachStopPoint()
    {
        if (transform.position.z >= stopPoint.position.z)
        {
            state = BossState.Fight;
            StartCoroutine(FightTimer());
        }
    }

    // ---------------- ATTACK ----------------
    void Attack()
    {
        if (Time.time < nextShootTime) return;

        nextShootTime = Time.time + coolDown;
        Shoot();
    }

    // ---------------- SHOOT ----------------
    void Shoot()
    {
        // 🔊 âm thanh bắn
        if (shootAudio != null && shootClip != null)
        {
            shootAudio.PlayOneShot(shootClip);
        }

        Vector3 predictedPos = GetPredictedPlayerPosition();

        Vector3 dir = (predictedPos - firePoint.position).normalized;

        if (!doubleShot)
        {
            ShootOne(dir);
        }
        else if (doubleShot && currentPoint == 0)
        {
            ShootOne(dir);
            ShootOne(Quaternion.Euler(0, -spreadAngle, 0) * dir);
        }
        else if (doubleShot && currentPoint == 1)
        {
            ShootOne(Quaternion.Euler(0, spreadAngle, 0) * dir);
            ShootOne(dir);
        }
    }

    // ================= PREDICTION CORE =================
    Vector3 GetPredictedPlayerPosition()
    {
        Rigidbody playerRb = player.GetComponent<Rigidbody>();

        Vector3 targetPos = player.position;

        // 🚫 KHÓA TRỤC Y (không bắt theo nhảy)
        targetPos.y = firePoint.position.y;

        if (playerRb != null)
        {
            float distance = Vector3.Distance(firePoint.position, player.position);
            float leadTime = distance / bulletSpeed;

            // ✅ Đón đầu theo Z (runner)
            targetPos.z += playerRb.velocity.z * leadTime;
        }

        // 🔧 sai số (nếu muốn)
        targetPos += aimOffset;

        return targetPos;
    }


    // ---------------- SHOOT ONE ----------------
    void ShootOne(Vector3 dir)
    {
        GameObject b = Instantiate(bullet, firePoint.position, Quaternion.identity);

        Rigidbody rb = b.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.velocity = dir * bulletSpeed;

        Destroy(b, 5f);
    }

    // -------- FIGHT → ESCAPE --------
    IEnumerator FightTimer()
    {
        yield return new WaitForSeconds(5f);

        state = BossState.Escape;

        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

#if UNITY_EDITOR
    // 🔍 DEBUG VỊ TRÍ DỰ ĐOÁN
    void OnDrawGizmos()
    {
        if (player == null || firePoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(GetPredictedPlayerPosition(), 0.4f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(firePoint.position, GetPredictedPlayerPosition());
    }
#endif
}
