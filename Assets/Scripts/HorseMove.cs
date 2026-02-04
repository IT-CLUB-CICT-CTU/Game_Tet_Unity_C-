using System.Collections;
using UnityEngine;

public class HorseMove : MonoBehaviour
{
    [Header("Move")]
    public float speed = 10f;
    public bool canMove = false;

    [Header("Steering")]
    public float steerStrength = 2f;       // ?? b? lái
    public float steerDuration = 0.3f;     // th?i gian b? lái
    public float steerCooldown = 1f;       // X giây / 1 l?n
    public float chaseChance = 0.5f;       // hên xui m?i l?n ??n l??t

    private float steerTimer = 0f;
    private float cooldownTimer = 0f;
    private bool isSteering = false;

    Transform player;

    void Update()
    {
        if (!canMove) return;

        MoveForward();

        // ? cooldown
        cooldownTimer -= Time.deltaTime;

        // ?? t?i l??t thì m?i xét hên xui
        if (cooldownTimer <= 0f && !isSteering)
        {
            cooldownTimer = steerCooldown;

            if (Random.value < chaseChance && player != null)
            {
                StartCoroutine(SteerOnce());
            }
        }
    }

    void MoveForward()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.World);
    }

    IEnumerator SteerOnce()
    {
        isSteering = true;
        float t = 0f;

        float targetX = player.position.x;

        while (t < steerDuration)
        {
            // h??ng chéo lên tr??c
            Vector3 targetPos = new Vector3(
                targetX,
                transform.position.y,
                transform.position.z + 5f
            );

            Vector3 dir = (targetPos - transform.position).normalized;

            transform.position += new Vector3(
                dir.x * steerStrength * Time.deltaTime,
                0,
                0
            );

            t += Time.deltaTime;
            yield return null;
        }

        isSteering = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canMove = true;
            player = other.transform;
        }
    }
}
