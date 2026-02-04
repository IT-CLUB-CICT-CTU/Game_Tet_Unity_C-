using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleMove : MonoBehaviour
{
    public BarrierMove barrierMove;
    public float distanceCheck;
    public float radiusCheck;

    void Update()
    {
        CheckBarrierAhead();
    }

    void CheckBarrierAhead()
    {
        Vector3 forwardCheckPos = transform.position + transform.forward * distanceCheck;
        Collider[] hitColliders = Physics.OverlapSphere(forwardCheckPos, radiusCheck);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("BigBoss"))
            {
                BigBossAttack bigBossAttack = hitCollider.GetComponent<BigBossAttack>();
                BigBossMove bigBossMove = hitCollider.GetComponent<BigBossMove>();
                if (bigBossMove != null && bigBossMove.isGrounded && bigBossAttack != null && !bigBossAttack.isReadyAtk)
                {
                    bigBossMove.Jump();
                }
            }
            if (hitCollider.CompareTag("Player"))
            {
                PlayerDie playerDie = hitCollider.GetComponent<PlayerDie>();
                if (playerDie != null && !playerDie.isDead && playerDie.isWin)
                {
                    playerDie.playerMove.Jump();
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            barrierMove.canMove = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 forwardCheckPos = transform.position + transform.forward * distanceCheck;
        Gizmos.DrawWireSphere(forwardCheckPos, radiusCheck);
    }
}
