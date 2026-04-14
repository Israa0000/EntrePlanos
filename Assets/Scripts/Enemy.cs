using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] Transform target;
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] private float stoppingDistance = 1f;
    private float distanceToTarget;
    private AIPath path;
    void Start()
    {
        path = GetComponent<AIPath>();
    }

    // Update is called once per frame
    void Update()
    {

        path.maxSpeed = moveSpeed;      

        distanceToTarget = Vector3.Distance(transform.position, target.position);
        
        if(distanceToTarget < stoppingDistance)
        {
            path.destination = transform.position; 
        }
        else
        {
            path.destination = target.position;
        }
    }
}
