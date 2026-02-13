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
    Vector3 defaultPlayerPosition;
    Vector3 defaultPlayerScale;
    bool isHiding = false;
    bool interactingWithTable = false; 
    void Start()
    {
        defaultPlayerScale = player.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        // ENTRAR A ESCONDERSE
        if (interactingWithTable && !isHiding && Input.GetKeyDown(KeyCode.C))
        {
            isHiding = true;

            defaultPlayerScale = player.transform.localScale;
            defaultPlayerPosition = player.transform.position;

            player.transform.localScale = hideScale;
            //player.transform.position = hidePosition.position;
        }

        // SALIR DEL ESCONDITE
        else if (isHiding && Input.GetKeyDown(KeyCode.C))
        {
            isHiding = false;

            //player.transform.position = defaultPlayerPosition;
            player.transform.localScale = defaultPlayerScale;
        }

        if ( interactingWithTable == false)
        {
            player.transform.localScale = defaultPlayerScale;
        }
    }
    private void OnTriggerEnter(Collider interactCollider)
    {
        if (interactCollider.CompareTag("HideTable"))
        {
            interactingWithTable = true;
        }
    }
    private void OnTriggerExit(Collider interactCollider)
    {
        if (interactCollider.CompareTag("HideTable"))
        {
            interactingWithTable = false;
        }
    }
}
