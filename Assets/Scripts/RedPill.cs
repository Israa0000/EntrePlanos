using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RedPill : MonoBehaviour
{
    GameObject pill;
    [SerializeField] Transform nextPlayerPlace;
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

        // Decide qu� player mover: referencia serializada o el que entr� en trigger
        GameObject targetPlayer = player != null ? player : currentPlayerInRange;
        if (targetPlayer == null)
        {
            Debug.LogWarning("RedPill: no hay player para mover.", this);
            return;
        }

        if (nextPlayerPlace == null)
        {
            Debug.LogWarning("RedPill: 'nextPlayerPlace' no asignado.", this);
            return;
        }

        Vector3 targetPos = nextPlayerPlace.position;

        // MOVER: si tiene CharacterController, deshabil�talo temporalmente y usa transform
        var cc = targetPlayer.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
            targetPlayer.transform.position = targetPos;
            cc.enabled = true;
        }
        else
        {
            // Si no tiene CharacterController, intenta Rigidbody3D, si no, fallback a transform
            var rb3d = targetPlayer.GetComponent<Rigidbody>();
            if (rb3d != null)
            {
                rb3d.position = targetPos;
                rb3d.velocity = Vector3.zero;
            }
            else
            {
                targetPlayer.transform.position = targetPos;
            }
        }

        Destroy(pill);
    }
}