using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSaver : MonoBehaviour
{
    // Start is called before the first frame update
    bool isInArea;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if(isInArea == true)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                print("EN area");
            }
            
        }
        if(isInArea == false)
        {
             if(Input.GetKeyDown(KeyCode.E))
            {
                print("NO EN area");
            }
        }
    }


     private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInArea = true;
            
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInArea = false;
        }
    }

}
