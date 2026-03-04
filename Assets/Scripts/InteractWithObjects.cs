using System;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class InteractWithObjects : MonoBehaviour
{

    [SerializeField] GameObject pickUpOrigin;
    [SerializeField] GameObject Door;
    [SerializeField] GameObject Keypad;
    [SerializeField] Canvas CodeCanvas;
    [SerializeField] FirstPersonController firstPersonController; // Ahora asignable desde el Inspector
    KeyPad keyPadScript;
    [SerializeField] float pickUpRadius = 2f;
    public bool interactionWithDoor = false;

    public void Start()
    {
        if (Keypad != null) keyPadScript = Keypad.GetComponent<KeyPad>();
        if (CodeCanvas != null) CodeCanvas.enabled = false;
    }

    private void Update()
    {

        if (interactionWithDoor && Input.GetKeyDown(KeyCode.E))
        {
            CodeDoorBehavior codeDoor = Door.GetComponent<CodeDoorBehavior>();
            if (codeDoor)
            {
                HandleCodeDoor(codeDoor);
                return;
            }

            DoorController lockedDoor = Door.GetComponent<DoorController>();
            if (lockedDoor)
            {
                HandleLockedDoor(lockedDoor);
                return;
            }

            NormalDoorBehavior normalDoor = Door.GetComponent<NormalDoorBehavior>();
            if (normalDoor != null)
            {
                normalDoor.Toggle();
                return;
            }
        }


        RaycastHit hit;


        if (Physics.SphereCast(pickUpOrigin.transform.position, pickUpRadius, pickUpOrigin.transform.TransformDirection(Vector3.forward), out hit, pickUpRadius) && Input.GetKey(KeyCode.E))
        {
            try //Debug.Log("Hit object: " + hit.collider.name);
            {
                if (hit.transform != null)
                {
                    var pickable = hit.transform.gameObject.GetComponent<IPickable>();
                    if (pickable != null)
                    {
                        pickable.OnPickUp(gameObject);
                        Debug.Log("IPICKABLE EN INVENTARIO");
                    }
                }else print("No hit");
            }   
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }
    }


    private void HandleCodeDoor(CodeDoorBehavior codeDoor)
    {
        if (keyPadScript != null && keyPadScript.openTheDoor == false)
        {
            if (CodeCanvas != null) CodeCanvas.enabled = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            firstPersonController.EnableControls(false);
        }
    }

    private void HandleLockedDoor(DoorController lockedDoor)
    {
        if (!lockedDoor.isUnlocked)
        {
            UnlockDoorMessage();
            lockedDoor.Unlock();
            return;
        }

        if (lockedDoor.isUnlocked && !lockedDoor.isAnimating)
        {
            if (lockedDoor.isOpen)
            {
                CloseDoorMessage();
                lockedDoor.Close();
            }
            else
            {
                OpenDoorMessage();
                lockedDoor.Open();
            }
        }
    }

    void UnlockDoorMessage() { print("puerta desbloqueada"); }
    void OpenDoorMessage() { print("puerta abierta"); }
    void CloseDoorMessage() { print("puerta cerrada"); }

    private void OnTriggerEnter(Collider interactCollider)
    {
        if (interactCollider.CompareTag("LockedDoor") || interactCollider.CompareTag("Door") || interactCollider.CompareTag("CodeDoor"))
        {
            Door = interactCollider.gameObject;
            interactionWithDoor = true;
        }
    }

    private void OnTriggerExit(Collider interactCollider)
    {
        if (interactCollider.CompareTag("LockedDoor") || interactCollider.CompareTag("Door") || interactCollider.CompareTag("CodeDoor"))
        {
            interactionWithDoor = false;
        }
    }

    public void ReactivatePlayerControls()
    {
        CodeCanvas.enabled = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Reactivar los controles del jugador
        firstPersonController.EnableControls(true);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pickUpOrigin.transform.position, pickUpRadius);
    }
}