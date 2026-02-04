using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BigBossMove : MonoBehaviour
{
    [Header("Refs")]
    public PlayerDie playerDie;
    public PlayerMove playerMove;

    [Header("Move")]
    public float moveSpeed;

    [Header("Jump")]
    public float jumpForce = 5f;
    public bool isGrounded = true, jump = false;

    [Header("Barrier Check")]
    public float checkDistance = 2.5f;
    public float checkRadius = 0.5f;
    public LayerMask barrierLayer;

    private Rigidbody rb;

    public BigBossAttack bigBossAttack;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (playerDie == null)
            playerDie = FindObjectOfType<PlayerDie>();

        if (playerMove == null)
            playerMove = FindObjectOfType<PlayerMove>();
    }

    void Update()
    {
        if (playerDie == null || playerMove == null) return;
        if (playerDie.isDead || playerDie.isWin) return;

        MoveForward();
        if (bigBossAttack != null && !bigBossAttack.isReadyAtk)
        {
            CheckBarrierAhead();
        }
        if (jump)
        {
            Jump();
            jump = false;
        }
    }

    // ================= MOVE =================
    void MoveForward()
    {
        moveSpeed = playerMove.forwardSpeed;
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.World);
    }

    // ================= BARRIER CHECK =================
    void CheckBarrierAhead()
    {
        if (!isGrounded) return;

        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        if (Physics.SphereCast(
            origin,
            checkRadius,
            transform.forward,
            out hit,
            checkDistance,
            barrierLayer
        ))
        {
            if (hit.collider.CompareTag("ShortBarrier"))
            {
                Jump();
            }
        }
    }

    // ================= JUMP =================
    public void Jump()
    {
        if (!isGrounded) return;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
    }

    // ================= GROUND CHECK =================
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    // ================= DEBUG =================
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        Gizmos.DrawLine(origin, origin + transform.forward * checkDistance);
        Gizmos.DrawWireSphere(origin + transform.forward * checkDistance, checkRadius);
    }
}
