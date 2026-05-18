using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private float speed = 5f;
    Rigidbody2D rb;
    private Vector2 input;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

      input = Vector2.zero;

        
        if (Input.GetKey(KeyCode.W))
        { 
            input.y += 1f;
        }

        if (Input.GetKey(KeyCode.S))
        {
            input.y -= 1f;
        }


        if (Input.GetKey(KeyCode.D))
        {
            input.x += 1f;
        }

        if (Input.GetKey(KeyCode.A))
        {
            input.x -= 1f;
        }

        input = input.normalized;
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + input * speed * Time.fixedDeltaTime);
    }
}
