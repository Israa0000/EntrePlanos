using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtackSystem : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] movimiento movimiento;
    [SerializeField] GameObject hitBoxGO;
    BoxCollider2D boxColider;
    [SerializeField] Vector2 up;
    [SerializeField] Vector2 down;
    [SerializeField] Vector2 right;
    [SerializeField] Vector2 left;
    void Start()
    {
        boxColider = hitBoxGO.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(movimiento.isAttacking == true)
        {
            if(movimiento.lastinput == Vector2.left)
            {
                hitBoxGO.SetActive(true);
                boxColider.size = new Vector2(1f, 1.57f);
                boxColider.offset = new Vector2(-0.7f, 0.67f);
            }
            if(movimiento.lastinput == Vector2.right)
            {
                hitBoxGO.SetActive(true);
                boxColider.size = new Vector2(1f, 1.57f);
                boxColider.offset = new Vector2(0.7f, 0.67f);
            }
             if(movimiento.lastinput == Vector2.down)
            {
                hitBoxGO.SetActive(true);
                boxColider.size = new Vector2(1.4f, 1.19f);
                boxColider.offset = new Vector2(0.07f, -0.22f);
            }
             if(movimiento.lastinput == Vector2.up)
            {
                hitBoxGO.SetActive(true);
                boxColider.size = new Vector2(1.4f, 1.19f);
                boxColider.offset = new Vector2(0.07f, 0.95f);          
            }
        }
        else
        {
            hitBoxGO.SetActive(false);
        }  
    }
}
