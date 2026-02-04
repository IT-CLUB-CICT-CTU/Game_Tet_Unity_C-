using UnityEngine;

public class LixiMagnet : MonoBehaviour
{
    public float followSpeed = 15f;
    private Transform target;
    private bool isMagneted;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void AttachToMagnet(Transform point)
    {
        target = point;
        isMagneted = true;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true; // ❗ tắt vật lý
        }
    }

    void Update()
    {
        if (!isMagneted || target == null) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            followSpeed * Time.deltaTime
        );
    }
}
