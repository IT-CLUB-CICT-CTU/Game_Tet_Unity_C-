using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public PlayerDie playerDie;

    void Start()
    {
        if (playerDie == null)
        {
            playerDie = FindObjectOfType<PlayerDie>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerDie != null)
            {
                playerDie.Protect();
                Destroy(gameObject);
            }
        }
    }
}
