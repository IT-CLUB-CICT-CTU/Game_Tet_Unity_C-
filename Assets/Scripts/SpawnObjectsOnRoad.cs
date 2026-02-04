using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PrefabSpawnChance
{
    public GameObject prefab;

    [Range(0, 100)]
    public float spawnChance;

    public bool isSpecial;
}

public class SpawnObjectsOnRoad : MonoBehaviour
{
    [Header("Lane Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Prefabs")]
    public PrefabSpawnChance[] prefabs;

    [Header("Mode")]
    public bool spawnReWard = false;

    [Header("Spawn Control")]
    [Range(0, 100)]
    public float spawnRate = 100f;

    // 🔒 lane đã có special
    private HashSet<int> specialLanes = new HashSet<int>();
    public bool isBarrier = false;

    void Start()
    {
        if (isBarrier)
        {
            if (Random.Range(0, 100) > spawnRate)
                return;

            if (spawnReWard)
                SpawnReward();
            else
                SpawnObjects();
        }
    }

    public void StartState()
    {
        ClearAllSpawnedObjects(); // reset trạng thái spawner

        if (Random.Range(0, 100) > spawnRate)
            return;

        if (spawnReWard)
            SpawnReward();
        else
            SpawnObjects();
    }

    // ================= OBJECT THƯỜNG =================
    void SpawnObjects()
    {
        int spawnCount = Random.Range(1, 3);
        List<int> usedIndexes = new List<int>();

        for (int i = 0; i < spawnCount; i++)
        {
            int lane = GetRandomLane(usedIndexes);

            PrefabSpawnChance item = GetRandomPrefabByChance();
            if (item == null || item.prefab == null) continue;

            if (item.isSpecial && IsAdjacentToSpecial(lane))
            {
                item = GetNonSpecialPrefab();
                if (item == null) continue;
            }

            SpawnAtLane(item, lane);
            usedIndexes.Add(lane);

            if (item.isSpecial)
                specialLanes.Add(lane);
        }
    }

    // ================= REWARD =================
    void SpawnReward()
    {
        int spawnCount = Random.Range(1, 4);
        List<int> usedLanes = new List<int>();

        PrefabSpawnChance firstItem = GetRandomPrefabByChance();
        if (firstItem == null || firstItem.prefab == null) return;

        // ===== SPECIAL (ziczac) =====
        if (firstItem.isSpecial)
        {
            int centerLane = spawnPoints.Length / 2;

            if (IsAdjacentToSpecial(centerLane))
                return;

            SpawnAtLane(firstItem, centerLane);
            specialLanes.Add(centerLane);
            return;
        }

        // ===== NON SPECIAL =====
        int firstLane = GetRandomLane(usedLanes);
        SpawnAtLane(firstItem, firstLane);
        usedLanes.Add(firstLane);

        for (int i = 1; i < spawnCount; i++)
        {
            PrefabSpawnChance item = GetNonSpecialPrefab();
            if (item == null) break;

            int lane = GetRandomLane(usedLanes);
            SpawnAtLane(item, lane);
            usedLanes.Add(lane);
        }
    }

    // ================= HELPERS =================
    int GetRandomLane(List<int> used)
    {
        int lane;
        do
        {
            lane = Random.Range(0, spawnPoints.Length);
        }
        while (used.Contains(lane));

        return lane;
    }

    void SpawnAtLane(PrefabSpawnChance item, int lane)
    {
        GameObject obj = Instantiate(
            item.prefab,
            spawnPoints[lane].position,
            Quaternion.identity
        );
        obj.transform.parent = transform;
    }

    bool IsAdjacentToSpecial(int laneIndex)
    {
        return specialLanes.Contains(laneIndex)
            || specialLanes.Contains(laneIndex - 1)
            || specialLanes.Contains(laneIndex + 1);
    }

    // ================= RANDOM BY % =================
    PrefabSpawnChance GetRandomPrefabByChance()
    {
        float total = 0f;
        foreach (var p in prefabs)
            total += p.spawnChance;

        float roll = Random.Range(0f, total);
        float current = 0f;

        foreach (var p in prefabs)
        {
            current += p.spawnChance;
            if (roll < current)
                return p;
        }

        return null;
    }

    PrefabSpawnChance GetNonSpecialPrefab()
    {
        List<PrefabSpawnChance> list = new List<PrefabSpawnChance>();

        foreach (var p in prefabs)
            if (!p.isSpecial && p.prefab != null && p.spawnChance > 0)
                list.Add(p);

        if (list.Count == 0) return null;

        float total = 0f;
        foreach (var p in list)
            total += p.spawnChance;

        float roll = Random.Range(0f, total);
        float current = 0f;

        foreach (var p in list)
        {
            current += p.spawnChance;
            if (roll < current)
                return p;
        }

        return null;
    }

    public void ClearAllSpawnedObjects()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        specialLanes.Clear(); // reset lane special
    }

}
