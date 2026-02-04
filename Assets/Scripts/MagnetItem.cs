using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetItem : MonoBehaviour
{
    public Magnet magnet;
    
    void Start()
    {
        if (magnet == null)
        {
            magnet = FindObjectOfType<Magnet>();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (magnet == null) return;
        if (other.CompareTag("Player"))
        {
            magnet.PowerOnMagnet();
            Destroy(gameObject);
        }
    }
}
