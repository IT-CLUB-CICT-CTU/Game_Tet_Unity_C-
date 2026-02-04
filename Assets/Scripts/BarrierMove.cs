using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierMove : MonoBehaviour
{
    public float radiusCheck = 5f;
    public float speed;
    public bool canMove = false;

    void Update()
    {
        if (canMove)
        {
            Move();
        }
        Check();
    }

    void Move()
    {
        transform.Translate(Vector3.back * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canMove = true;
        }
    }

    void Check()
    {
        Vector3 vector3 = new Vector3(transform.position.x, transform.position.y, transform.position.z - 10f);
        Collider[] hitColliders = Physics.OverlapSphere(vector3, radiusCheck);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("ShortBarrier") || hitCollider.CompareTag("TallBarrier"))
            {
                canMove = false;
            }
        }
    }
}
