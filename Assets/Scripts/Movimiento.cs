using UnityEngine;

public class movimiento : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] public GameObject cameraSwitcher;
<<<<<<< Updated upstream
    [SerializeField] Animator animator;
=======
    [SerializeField] public Animator animator;
>>>>>>> Stashed changes
    CameraSwitcher cameraSwitcherScript;
    public Vector2 input;
    public Vector2 lastinput;

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

        // Normalizar para evitar velocidad extra en diagonal
        input = input.normalized;
<<<<<<< Updated upstream
        animator.SetFloat("XMovement", lastinput.x);
        animator.SetFloat("YMovement", lastinput.y);

        if (input != Vector2.zero)
        {
            animator.SetBool("IsMoving", true);
        }
        else { 
            animator.SetBool("IsMoving", false);
        }
=======
        animatorControler();
>>>>>>> Stashed changes
    }

    void FixedUpdate()
    {
        // Movimiento con Rigidbody2D
        rb.MovePosition(rb.position + input * speed * Time.fixedDeltaTime);
    }

    private void animatorControler() {


        if (input != Vector2.zero)
        {

            animator.SetFloat("XInput", lastinput.x);
            animator.SetFloat("YInput", lastinput.y);
            animator.SetBool("IsWalking", true);
        }
        else {
            animator.SetBool("IsWalking", false);
        }

    }

    

    
}