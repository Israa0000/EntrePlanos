using UnityEngine;

public class movimiento : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] public GameObject cameraSwitcher;
    [SerializeField] Animator animator;
    CameraSwitcher cameraSwitcherScript;
    private Vector2 input;
    public Vector2 lastinput;

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
        input = Vector2.zero;

        if (cameraSwitcherScript.movement == MovementType.Movement2D) 
        {
            if (Input.GetKey(KeyCode.W))
            { 
                input.y += 1f;
                lastinput = new Vector3(0, 1, 0);
            }

            if (Input.GetKey(KeyCode.S))
            {
                input.y -= 1f;
                lastinput = new Vector3(0, -1, 0);
            }


            if (Input.GetKey(KeyCode.D))
            {
                input.x += 1f;
                lastinput = new Vector3(1, 0, 0);
            }

            if (Input.GetKey(KeyCode.A))
            {
                input.x -= 1f;
                lastinput = new Vector3(-1, 0, 0);
            }
        } 
        animationSistem();

        // Normalizar para evitar velocidad extra en diagonal
        input = input.normalized;
        animator.SetFloat("XMovement", lastinput.x);
        animator.SetFloat("YMovement", lastinput.y);

        if (input != Vector2.zero)
        {
            animator.SetBool("IsMoving", true);
        }
        else { 
            animator.SetBool("IsMoving", false);
        }
    }

    void FixedUpdate()
    {
        // Movimiento con Rigidbody2D
        rb.MovePosition(rb.position + input * speed * Time.fixedDeltaTime);
    }

    private void animationSistem()   //Sistema de animacion
    {
        if (animator != null)
            {
                Vector2 lastInput = new Vector2();
                if (input != Vector2.zero)
                {
                    lastInput = input;
                    animator.SetFloat("MoveX", input.x);
                    animator.SetFloat("moveY", input.y);
                    isMoving = true;
                 
                                
                }
                else
                {
                    animator.SetFloat("moveX", lastInput.x);
                    animator.SetFloat("moveY", lastInput.y);
                    isMoving = false;
                }

                animator.SetBool("isMoving", isMoving);
            }

    }

    

    
}