using UnityEngine;

public class TurretAim : MonoBehaviour
{
    public Transform player;
    public float rotateSpeed = 5f;

    void Update()
    {
        if (player == null)
        {
            GameObject pl = GameObject.FindWithTag("Player");
            if (pl != null)
                player = pl.transform;
            return;
        }

        // Hướng đến player, bỏ chiều cao
        Vector3 dir = player.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRot = Quaternion.LookRotation(dir);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            rotateSpeed * Time.deltaTime
        );
    }

    // 🔒 Khóa trục X (không ngẩng / cúi)
    void LateUpdate()
    {
        Vector3 euler = transform.eulerAngles;
        euler.x = 0f;
        transform.eulerAngles = euler;
    }
}
