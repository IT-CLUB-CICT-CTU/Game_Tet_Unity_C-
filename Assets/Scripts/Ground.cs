using UnityEngine;

public class Ground : MonoBehaviour
{
    private GroundSpawner spawner;
    public SpawnObjectsOnRoad[] spawnObjectsOnRoad;
    public Transform endPoint;

    [HideInInspector]
    public int prefabIndex;

    private bool isReached;

    void Awake()
    {
        spawner = FindObjectOfType<GroundSpawner>();
    }

    void OnEnable()
    {
        // 🔥 reset khi lấy từ pool
        isReached = false;

        // Khởi tạo trạng thái spawn object trên đoạn đường này
        if (spawnObjectsOnRoad == null || spawnObjectsOnRoad.Length == 0) return;
        foreach (var spawner in spawnObjectsOnRoad)
        {
            spawner.StartState();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isReached) return;
        if (!other.CompareTag("Player")) return;

        isReached = true;

        if (spawner != null)
        {
            spawner.RequestSpawn();
        }
    }
}
