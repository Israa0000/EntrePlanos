using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    public NavMeshAgent EnemyMeshAgent;
    public float speed;
    public bool isChasing;
    public float rangeToChase;
    float distance;

    public Transform player;

    private void Update()
    {
        distance = Vector3.Distance(EnemyMeshAgent.transform.position, player.position);

        if (distance < rangeToChase)
        {
            isChasing = true;
        }
        else
        {
            isChasing = false;
        }

        if (isChasing)
        {
            EnemyMeshAgent.speed = speed;
            EnemyMeshAgent.SetDestination(player.position);
        }

        if (isChasing == false)
        {
            EnemyMeshAgent.speed = 0;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(EnemyMeshAgent.transform.position, rangeToChase);
    }
}
