using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;

public class HideBehavior : MonoBehaviour
{
    [SerializeField] Vector3 hideScale;
    [SerializeField] Transform hidePosition;
    [SerializeField] GameObject table;
    [SerializeField] GameObject player;
    bool insideCollider;
    Vector3 defaultPlayerScale;
    bool isHiding = false;
    void Start()
    {
        defaultPlayerScale = player.transform.localScale;
        isHiding = false;
    }

    // Update is called once per frame
    void Update()
    {
       
        // Si está dentro del collider, NO puede levantarse
        if (insideCollider)
        {
            if (Input.GetKey(KeyCode.C))
            {
                player.transform.localScale = hideScale;
            }
            // Si suelta C, NO se levanta
        }
        else
        {
            // Fuera del collider, comportamiento normal
            if (Input.GetKey(KeyCode.C))
                player.transform.localScale = hideScale;
            else
                player.transform.localScale = defaultPlayerScale;
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HideTable"))
        {
            insideCollider = true;
            print("hide");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("HideTable"))
        {
            insideCollider = false;
            player.transform.localScale = defaultPlayerScale; // por si sale agachado
        }
    }

}
