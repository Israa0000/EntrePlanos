using Unity.VisualScripting;
using UnityEngine;

public class InteractWithObjects : MonoBehaviour
{
    [SerializeField] GameObject Key;
    [SerializeField] GameObject Door; // Referencia a la puerta (puede ser normal o bloqueada)
    [SerializeField] GameObject Keypad;
    [SerializeField] Canvas CodeCanvas;
    [SerializeField] FirstPersonController firstPersonController; // Ahora asignable desde el Inspector
    KeyPad keyPadScript;
    public bool interactionWithKey = false;
    public bool interactionWithDoor = false;

    bool playerGotKey = false;
    public void Start()
    {
        keyPadScript = Keypad.GetComponent<KeyPad>();
        CodeCanvas.enabled = false;
    }

    private void Update()
    {
        if (interactionWithKey && Input.GetKeyDown(KeyCode.E))
        {
            GotKeyMessage();
            playerGotKey = true;
            if (Key != null) Destroy(Key);
        }

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

        if (CodeCanvas.enabled && Input.GetKeyDown(KeyCode.F))
        {
            CodeCanvas.enabled = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            firstPersonController.EnableControls(true);
        }
    }

    private void HandleCodeDoor(CodeDoorBehavior codeDoor)
    {
        if (keyPadScript.openTheDoor == false)
        {
            CodeCanvas.enabled = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            firstPersonController.EnableControls(false);
        }
    }

    private void HandleLockedDoor(DoorController lockedDoor)
    {
        if (playerGotKey && !lockedDoor.isUnlocked)
        {
            UnlockDoorMessage();
            lockedDoor.Unlock();
            playerGotKey = false;
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

    void GotKeyMessage() { print("llave obtenida"); }
    void UnlockDoorMessage() { print("puerta desbloqueada"); }
    void OpenDoorMessage() { print("puerta abierta"); }
    void CloseDoorMessage() { print("puerta cerrada"); }

    private void OnTriggerEnter(Collider interactCollider)
    {
        Door = interactCollider.gameObject;
        if (interactCollider.CompareTag("Key"))
            interactionWithKey = true;

        if (interactCollider.CompareTag("LockedDoor"))
        {
            interactionWithDoor = true;
        }
        if (interactCollider.CompareTag("Door"))
        {
            interactionWithDoor = true;
        }
        if (interactCollider.CompareTag("CodeDoor"))
        {
            interactionWithDoor = true;
        }
    }

    private void OnTriggerExit(Collider interactCollider)
    {
        if (interactCollider.CompareTag("Key"))
            interactionWithKey = false;

        if (interactCollider.CompareTag("LockedDoor"))
        {
            interactionWithDoor = false;
        }
        if (interactCollider.CompareTag("Door"))
        {
            interactionWithDoor = false;
        }
        if (interactCollider.CompareTag("CodeDoor"))
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
}