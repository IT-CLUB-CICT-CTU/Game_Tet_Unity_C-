using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMiniBoss : MonoBehaviour
{
    public DistanceShow distance;

    [Header("Spawn Points (CHỈ 2 VỊ TRÍ)")]
    public Transform[] spawnPoints;   // index 0, 1
    public Transform[] stopPoints;    // index 0, 1

    [Header("MiniBoss Prefabs")]
    public GameObject[] prefabs;
    public int spawnDelay = 3;

    private bool isSpawn = false;

    private List<GameObject> shuffledPrefabs = new List<GameObject>();
    private int currentIndex = 0;

    // 👉 Luân phiên spawn point
    private int rPoint = 0;

    [Header("BigBoss")]
    public GameObject bigBoss;
    public Transform bigBossSpawnPoint;
    private bool isBigBossSpawned = false;

    void Start()
    {
        ResetAndShuffle();
    }

    void Update()
    {
        if (!isSpawn)
        {
            Spawn();
        }

        if (distance.progress >= 198f)
        {
            SpawnBigBoss();
        }
    }

    void ResetAndShuffle()
    {
        shuffledPrefabs = new List<GameObject>(prefabs);
        Shuffle(shuffledPrefabs);
        currentIndex = 0;
    }

    void Spawn()
    {
        isSpawn = true;
        StartCoroutine(WaitSpawn(spawnDelay));
    }

    IEnumerator WaitSpawn(int time)
    {
        yield return new WaitForSeconds(time);

        // ❌ Không spawn ngoài khoảng này
        if (distance.progress < 90f || distance.progress >= 190f)
        {
            isSpawn = false;
            yield break;
        }

        // ✅ Spawn hết → shuffle lại
        if (currentIndex >= shuffledPrefabs.Count)
        {
            ResetAndShuffle();
        }

        // ✅ Spawn MiniBoss
        GameObject miniB = Instantiate(
            shuffledPrefabs[currentIndex],
            spawnPoints[rPoint].position,
            Quaternion.identity
        );

        MiniBoss mb = miniB.GetComponent<MiniBoss>();
        mb.stopPoint = stopPoints[rPoint];
        mb.currentPoint = rPoint;

        // 🔁 Đổi spawn point: 0 → 1 → 0 → 1
        rPoint = 1 - rPoint;

        currentIndex++;
        isSpawn = false;
    }

    void SpawnBigBoss()
    {
        if (isBigBossSpawned) return;

        // ❌ Xóa toàn bộ MiniBoss trước khi spawn BigBoss
        GameObject[] miniBosses = GameObject.FindGameObjectsWithTag("MiniBoss");
        foreach (var go in miniBosses)
            Destroy(go);

        Instantiate(bigBoss, bigBossSpawnPoint.position, Quaternion.identity);
        isBigBossSpawned = true;
    }

    void Shuffle(List<GameObject> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);
            (list[i], list[r]) = (list[r], list[i]);
        }
    }
}
