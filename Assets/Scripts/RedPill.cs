using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RedPill : MonoBehaviour
{
    GameObject pill;
    [SerializeField] GameObject player;
    [SerializeField] string nextSceneName;

    bool playerInRange = false;
    GameObject currentPlayerInRange;
    bool taken = false;

    void Start()
    {
        pill = this.gameObject;

    }

    void Update()
    {
        if (playerInRange && Input.GetMouseButtonDown(0))
        {
            takePill();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInRange = true;
            currentPlayerInRange = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInRange = false;
            currentPlayerInRange = null;
        }
    }

    void takePill()
    {

        SceneManager.LoadScene(nextSceneName);
        if (taken) return;
        taken = true;

        Debug.Log("Pill taken");


        Destroy(pill);
    }
}