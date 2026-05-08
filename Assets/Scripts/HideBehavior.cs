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
        // ENTRAR A ESCONDERSE
        if (Input.GetKey(KeyCode.C))
        {
            player.transform.localScale = hideScale;
        }
        else { player.transform.localScale = defaultPlayerScale; }
            
    }
}
