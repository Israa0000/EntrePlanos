using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Pathfinding;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] Transform target;
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] private float stoppingDistance = 1f;
    [SerializeField] Animator animator;
    private float distanceToTarget;
    private AIPath path;
    private Vector2 direction;
    private Vector2 lastPosition;
    private Vector2 movement;
    private bool isMoving;

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

        direction = (target.position - transform.position).normalized;

        movement = (Vector2)transform.position - lastPosition;
        lastPosition = transform.position;

        animationSistem(movement);

    }



    private void animationSistem(Vector2 movement)   //Sistema de animacion
    {
        if (animator != null)
            {
                
                if(movement != Vector2.zero)
                {
                    animator.SetFloat("XMovement", direction.x);
                    animator.SetFloat("YMovement", direction.y);
                    isMoving = true;
                   
                }
            else
            {
                isMoving = false;
               
            }

             animator.SetBool("IsMoving", isMoving);
                

            }

    }
}
