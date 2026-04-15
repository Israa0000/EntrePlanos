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


        Vector2 vel = path.velocity;
        animationSistem(vel);


    }



    private void animationSistem(Vector2 vel)   //Sistema de animacion
    {
        if (animator != null)
            {
                
                if(vel != Vector2.zero)
                {
                    animator.SetFloat("XMovement", vel.x);
                    animator.SetFloat("YMovement", vel.y);
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
