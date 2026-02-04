using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSpawner : MonoBehaviour
{
    [Header("Ground Prefabs")]
    public GameObject[] groundPrefab;

    [Header("Spawn")]
    public float spawnDelay = 1.5f;
    public int startGroundCount = 25;
    public int poolSizePerPrefab = 10;

    [Header("Runtime")]
    public List<GameObject> groundList = new List<GameObject>();
    public bool isSpawning = false;

    // ================= POOL =================
    private Dictionary<int, Queue<GameObject>> groundPools =
        new Dictionary<int, Queue<GameObject>>();

    // ================= SHUFFLE =================
    private List<int> shuffleIndexes = new List<int>();
    private int shufflePointer = 0;

    void Awake()
    {
        InitPools();
        InitShuffle();
    }

    void Start()
    {
        // Spawn ground ban đầu
        for (int i = 0; i < startGroundCount; i++)
        {
            int index = GetNextShuffleIndex();
            SpawnImmediate(index);
        }
    }

    // ================= POOL INIT =================
    void InitPools()
    {
        for (int i = 0; i < groundPrefab.Length; i++)
        {
            Queue<GameObject> pool = new Queue<GameObject>();

            for (int j = 0; j < poolSizePerPrefab; j++)
            {
                GameObject g = Instantiate(groundPrefab[i], transform);
                g.SetActive(false);
                pool.Enqueue(g);
            }

            groundPools.Add(i, pool);
        }
    }

    GameObject GetFromPool(int index)
    {
        if (!groundPools.ContainsKey(index))
            return null;

        if (groundPools[index].Count > 0)
            return groundPools[index].Dequeue();

        // pool thiếu → tạo thêm (fallback)
        GameObject g = Instantiate(groundPrefab[index], transform);
        g.SetActive(false);
        return g;
    }

    void ReturnToPool(GameObject ground, int index)
    {
        ground.SetActive(false);
        ground.transform.SetParent(transform);
        groundPools[index].Enqueue(ground);
    }

    // ================= SHUFFLE =================
    void InitShuffle()
    {
        shuffleIndexes.Clear();

        for (int i = 0; i < groundPrefab.Length; i++)
            shuffleIndexes.Add(i);

        Shuffle(shuffleIndexes);
        shufflePointer = 0;
    }

    int GetNextShuffleIndex()
    {
        if (shufflePointer >= shuffleIndexes.Count)
        {
            Shuffle(shuffleIndexes);
            shufflePointer = 0;
        }

        return shuffleIndexes[shufflePointer++];
    }

    void Shuffle(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    // ================= SPAWN =================
    void SpawnImmediate(int index)
    {
        if (groundList.Count == 0)
        {
            Debug.LogError("GroundList is empty! Cần ground đầu tiên trong scene.");
            return;
        }

        GameObject lastGround = groundList[groundList.Count - 1];
        Transform spawnPoint = lastGround.GetComponent<Ground>().endPoint;

        GameObject newGround = GetFromPool(index);
        newGround.transform.position = spawnPoint.position;
        newGround.transform.rotation = Quaternion.identity;
        newGround.SetActive(true);

        groundList.Add(newGround);
    }

    public void RequestSpawn()
    {
        if (!isSpawning)
        {
            int index = GetNextShuffleIndex();
            StartCoroutine(SpawnGroundWithDelay(index));
        }
    }

    IEnumerator SpawnGroundWithDelay(int index)
    {
        isSpawning = true;
        yield return new WaitForSeconds(spawnDelay);

        SpawnImmediate(index);
        isSpawning = false;

        // ================= REMOVE OLD GROUND =================
        yield return new WaitForSeconds(10f);

        if (groundList.Count > 20)
        {
            GameObject oldGround = groundList[0];
            groundList.RemoveAt(0);

            Ground g = oldGround.GetComponent<Ground>();
            ReturnToPool(oldGround, g.prefabIndex);
        }
    }
}
