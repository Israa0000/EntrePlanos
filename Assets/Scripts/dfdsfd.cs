using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class dfdsfd : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject player;
    [SerializeField] float speed = 1;
    [SerializeField] movimiento movimientoScript;

    Vector3 direccionbola;
    bool a = false;
    void Start()
    {
         
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")){

            direccionbola = movimientoScript.lastinput;
            a = true;
            transform.position = player.transform.position;
            
        }
       
    }

    private void FixedUpdate()
    {
        if (!a)
        {
            gameObject.transform.position += Vector3.left * speed * Time.fixedDeltaTime;
        }
        else {
            gameObject.transform.position += direccionbola * speed * Time.fixedDeltaTime;
        }
    }


}
