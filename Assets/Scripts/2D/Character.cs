using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    public int damage;
    protected Animator animator;
    protected bool isKnockedBack;
    protected float knockbackDuration = 0.5f;


    protected virtual void Update() //protected para que solo se pueda llamar desde clases hijas, virtual para que cada personaje pueda implementar su propio sistema de movimiento
    {
       
    }
    public virtual void TakeDamage(int amount)
    {
        currentHealth -= amount;
      
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("Hit");
            print("daño");
        }
    }

    protected virtual void animationSistem(Vector2 vel, bool isMoving)   //Sistema de animacion
    {
        if (animator != null)
        {                           
            animator.SetBool("IsMoving", isMoving);

            if (isMoving)
            {
                animator.SetFloat("XMovement", vel.x);
                animator.SetFloat("YMovement", vel.y);
            }

        }

    }

    public abstract void Attack(); //Cada personaje implementa su propio metodo de ataque
    void StopKnockback()
    {
        isKnockedBack = false;
    }

    protected virtual void Die() 
    {

        Destroy(gameObject);
    }

}
