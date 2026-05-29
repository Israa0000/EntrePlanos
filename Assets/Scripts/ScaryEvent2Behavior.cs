using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaryEvent2Behavior : MonoBehaviour
{
    [SerializeField] GameObject bulb;
    [SerializeField] AudioClip breakBulb;
    [SerializeField] AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            audioSource.PlayOneShot(breakBulb);
            Destroy(bulb);
            Destroy(gameObject); // Destruye el objeto del evento para que no se active nuevamente
        }
    }
}
