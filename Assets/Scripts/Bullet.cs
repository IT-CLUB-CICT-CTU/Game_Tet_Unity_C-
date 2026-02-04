using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 5f;
    public string[] sentences;
    public PlayerDie player;
    public DeathMenu d;
    public bool isRotate = false;
    public float rotateSpeed = 100f;
    public Transform prefab;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        RotateBullet();
    }

    void AttackPlayer()
    {
        if (sentences == null || sentences.Length == 0) return;

        int index = Random.Range(0, sentences.Length);

        player = FindObjectOfType<PlayerDie>();
        d = FindObjectOfType<DeathMenu>();

        if (player != null && !player.isProtect)
            player.TakeDamage(1);
        else if (player != null && player.isProtect)
            return;

        if (d != null)
            d.ShowNewDeadText(sentences[index]);

        Destroy(gameObject);
    }
    
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AttackPlayer();
        }
    }

    void RotateBullet()
    {
        if (isRotate)
        {
            prefab.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
        }
    }
}
