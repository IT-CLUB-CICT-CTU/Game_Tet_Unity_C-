using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjWithPercent : MonoBehaviour
{
    public GameObject prefab;
    public Transform spawnPoint;
    [Range(0, 100)] 
    public float spawnPercent;
    private bool hasSpawned = false;

    void Update()
    {
        if (!hasSpawned)
        {
            TrySpawn();
        }
    }
    void TrySpawn()
    {
        float r = Random.Range(0f, 100f);
        if (r <= spawnPercent)
        {
            Instantiate(
                prefab,
                spawnPoint.position,
                Quaternion.identity
            );
        }
        hasSpawned = true;
    }
}
