using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField] Transform target;
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] private float stoppingDistance = 1f;
    [SerializeField] float knockbackForce = 5f;
    [SerializeField] int stage;
    [SerializeField] GameObject deadBody;
    private bool isMoving;
    private Vector2 playerDir;
    private float lastDamageTakenTime;
    private float damageCooldown = 0.5f;
    private float distanceToTarget;
    private AIPath path;
    private Vector2 knockbackVelocity;
    [SerializeField] private float knockbackDuration = 0.2f;
    Rigidbody2D rb;

    void Start()
    {
        path = GetComponent<AIPath>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        
    }

    protected override void Update()
    {
       movement();
       
    }
    void FixedUpdate()
    {
        // Aplicar knockback si está activo
        if (knockbackVelocity != Vector2.zero)
        {
            rb.velocity = knockbackVelocity;
        }
    }
    protected override void animationSistem(Vector2 vel, bool isMoving)  
    {
        Vector2 playerDir = (target.position - transform.position).normalized;  //Vector que apunta hacia el jugador
        animator.SetFloat("XPlayerDirection", playerDir.x);  //Se aplica al enemigo la direccion para que mire al jugador cuando se le aplique el knockback
        animator.SetFloat("YPlayerDirection", playerDir.y);      

        base.animationSistem(vel, isMoving);  //Funicon base de clase character

    }

    public override void Attack()
    {
        animator.SetTrigger("Attack");
    }

    private void OnTriggerEnter2D(Collider2D collision)   //se aplica el knockback cuando el enemigo es golpeado
    {
        if (collision.CompareTag("PlayerAttack") && Time.time >= lastDamageTakenTime + damageCooldown)
        {
            Player player = collision.GetComponentInParent<Player>();      

            if (player != null)
            {
                TakeDamage(player.damage);           
                KnockBack();                
                lastDamageTakenTime = Time.time;
                print("ay");
            }
        }
    }

    private void movement()
    {
        path.maxSpeed = moveSpeed;      

        distanceToTarget = Vector3.Distance(transform.position, target.position); // Calcula la distancia al objetivo   

        if(distanceToTarget < stoppingDistance)
        {
            path.destination = transform.position; 
            Attack();
        }
        else
        {
            path.destination = target.position;
        }

        

        Vector2 vel = path.velocity;
        isMoving = vel != Vector2.zero;
        animationSistem(vel, isMoving); 
    }

    
    void ReenablePath()  //Activa el ayPATH Y reinicia el knockback
    {
        path.enabled = true;
        knockbackVelocity = Vector2.zero;
    }

    private void KnockBack() //Provoca knockback al enemigo, desactivando temporalmente el AIPath y aplicando una fuerza en la dirección opuesta al jugador
    {
        Vector2 knockbackDirection = (transform.position - target.position).normalized;
        knockbackVelocity = knockbackDirection * knockbackForce; // Fuerza del knockback
        path.enabled = false; // Desactivar AIPath para que no interceda en el knockback
        Invoke(nameof(ReenablePath), knockbackDuration); //reactiva el AIPath después de la duración del knockback
    }

    protected override void Die() 
    {
        if(stage == 2)
        {
            deadBody.transform.position = transform.position;          
        }
        base.Die();
    }
    
}