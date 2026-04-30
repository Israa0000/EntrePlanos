using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Pathfinding;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] Transform target;
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] private float stoppingDistance = 1f;
    [SerializeField] Animator animator;
    Rigidbody2D rb;
    private float distanceToTarget;
    private AIPath path;
    private Vector2 direction;
    bool test = false;

    private Vector2 movement;
    private bool isMoving;
    private Vector2 knockbackVelocity;
    private float knockbackDuration = 0.2f;

    void Start()
    {
        path = GetComponent<AIPath>();
        rb = GetComponent<Rigidbody2D>();
    
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

        if (Input.GetKeyDown(KeyCode.E))
        {
            // Aplicar knockback desactivando AIPath temporalmente
            knockbackVelocity = Vector2.left * 10f; // Fuerza del knockback
            path.enabled = false; // Desactivar AIPath durante el knockback
            Invoke(nameof(ReenablePath), knockbackDuration);
        }
    }

    void FixedUpdate()
    {
        // Aplicar knockback si está activo
        if (knockbackVelocity != Vector2.zero)
        {
            rb.velocity = knockbackVelocity;
        }
    }

    void ReenablePath()
    {
        path.enabled = true;
        knockbackVelocity = Vector2.zero;
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
