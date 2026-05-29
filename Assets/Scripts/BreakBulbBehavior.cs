using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakBulbBehavior : MonoBehaviour
{
    GameObject Bulb;
    AudioClip breakSound; // Variable para almacenar el sonido de la bombilla rompiéndose
    // Start is called before the first frame update
    void Start()
    {
        Bulb = gameObject;  
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            Debug.Log("¡La bombilla se ha roto!");
            Destroy(Bulb);
        }
    }
}
