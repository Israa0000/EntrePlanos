using UnityEngine;

public class movimiento : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] public GameObject cameraSwitcher;
    [SerializeField] private Animator animator;

    private CameraSwitcher cameraSwitcherScript;

    private Vector2 input;
    public Vector2 lastinput;

    private bool isAttacking = false;
    private bool isMoving;

    void Awake()
    {
        cameraSwitcherScript = cameraSwitcher.GetComponent<CameraSwitcher>();

        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    void Update()
    {
        // 🔒 Si está atacando, bloqueamos input
        if (isAttacking)
        {
            input = Vector2.zero;
            animationSistem();
            return;
        }

        input = Vector2.zero;

        if (cameraSwitcherScript.movement == MovementType.Movement2D)
        {
            if (Input.GetKey(KeyCode.W)) input.y += 1f;
            if (Input.GetKey(KeyCode.S)) input.y -= 1f;
            if (Input.GetKey(KeyCode.D)) input.x += 1f;
            if (Input.GetKey(KeyCode.A)) input.x -= 1f;
        }

        // Normalizar para evitar velocidad extra en diagonal
        input = input.normalized;

        // 🎯 Guardamos dirección solo si se mueve
        if (input != Vector2.zero)
        {
            lastinput = input;
        }

        // ⚔️ Ataque
        if (Input.GetKeyDown(KeyCode.Space) && !isAttacking)
        {
            isAttacking = true;

            // Asegura dirección correcta al atacar
            if (input != Vector2.zero)
                lastinput = input;

            animator.SetTrigger("Attack");
        }

        animationSistem();
    }

    void FixedUpdate()
    {
        // 🔒 Bloquear movimiento al atacar
        if (isAttacking)
        {
            rb.MovePosition(rb.position);
            return;
        }

        rb.MovePosition(rb.position + input * speed * Time.fixedDeltaTime);
    }

    private void animationSistem()
    {
        if (animator == null) return;

        if (!isAttacking && input != Vector2.zero)
        {
            animator.SetFloat("XMovement", input.x);
            animator.SetFloat("YMovement", input.y);
            isMoving = true;
        }
        else
        {
            // Mantiene dirección durante ataque o idle
            animator.SetFloat("XMovement", lastinput.x);
            animator.SetFloat("YMovement", lastinput.y);
            isMoving = false;
        }

        animator.SetBool("IsMoving", isMoving);
    }

    // 🎬 LLAMAR DESDE ANIMATION EVENT
    public void endAttack()
    {
        isAttacking = false;
    }
}