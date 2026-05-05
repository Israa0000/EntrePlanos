using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedPill : MonoBehaviour
{
    [SerializeField] CameraSwitcher cameraSwitcher;
    GameObject pill;
    [SerializeField] Transform nextPlayerPlace;
    [SerializeField] GameObject player;

    bool playerInRange = false;
    GameObject currentPlayerInRange;
    bool taken = false;

    void Start()
    {
        pill = this.gameObject;

        if (cameraSwitcher == null)
        {
            Debug.LogError("RedPill: 'cameraSwitcher' no está asignado en el Inspector.", this);
        }
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
        if (taken) return;
        taken = true;

        Debug.Log("Pill taken");

        // Decide qué player mover: referencia serializada o el que entró en trigger
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

        // Mover primero: usa Rigidbody 3D si existe (tienes Rigidbody 3D)
        Vector3 targetPos = nextPlayerPlace.position;
        var rb3d = targetPlayer.GetComponent<Rigidbody>();
        if (rb3d != null)
        {
            // Si el controlador 3D estaba deshabilitado, activarlo antes de mover para asegurar coherencia
            var fps = targetPlayer.GetComponent<MonoBehaviour>();
            rb3d.position = targetPos;
            rb3d.velocity = Vector3.zero;
        }
        else
        {
            targetPlayer.transform.position = targetPos;
        }

        // Ahora alternamos cámaras y controles.
        // Importante: si CameraSwitcher gestiona controladores, llamar a ToggleControls o ForceMovement
        if (cameraSwitcher != null)
        {
            // Si quieres forzar modo 2D: cameraSwitcher.ForceMovement(MovementType.Movement2D);
            cameraSwitcher.ToggleCameras();
            cameraSwitcher.ToggleControls();
        }
        else
        {
            Debug.LogWarning("RedPill: cameraSwitcher null.", this);
        }

        Destroy(pill);
    }
}