using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    public NavMeshAgent EnemyMeshAgent;
    public float speed;
    public float patrolSpeed;
    public bool isChasing;
    public float rangeToChase;
    float distance;

    public Transform player;

    [Header("Patrulla")]
    public List<Transform> waypoints;
    int currentWaypoint = 0;
    float waypointReachDistance = 0.5f;

    private void Update()
    {
        distance = Vector3.Distance(EnemyMeshAgent.transform.position, player.position);

        isChasing = distance < rangeToChase;

        if (isChasing)
        {
            EnemyMeshAgent.speed = speed;
            EnemyMeshAgent.SetDestination(player.position);
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        if (waypoints == null || waypoints.Count == 0) return;

        EnemyMeshAgent.speed = patrolSpeed;

        // Si llegó al waypoint actual, avanza al siguiente
        if (!EnemyMeshAgent.pathPending && EnemyMeshAgent.remainingDistance <= waypointReachDistance)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Count;
        }

        EnemyMeshAgent.SetDestination(waypoints[currentWaypoint].position);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(EnemyMeshAgent.transform.position, rangeToChase);

        // Dibujar la ruta de patrulla
        if (waypoints == null || waypoints.Count == 0) return;
        Gizmos.color = Color.blue;
        for (int i = 0; i < waypoints.Count; i++)
        {
            if (waypoints[i] == null) continue;
            Gizmos.DrawSphere(waypoints[i].position, 0.2f);
            Gizmos.DrawLine(waypoints[i].position, waypoints[(i + 1) % waypoints.Count].position);
        }
    }
}