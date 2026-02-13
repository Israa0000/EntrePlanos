using Unity.VisualScripting;
using UnityEngine;

public class InteractWithObjects : MonoBehaviour
{
    [SerializeField] GameObject Key;
    [SerializeField] GameObject Door; // Referencia a la puerta (puede ser normal o bloqueada)
    [SerializeField] GameObject Keypad;
    [SerializeField] Canvas CodeCanvas;
    KeyPad keyPadScript;
    FirstPersonController firstPersonController;
    public bool interactionWithKey = false;
    public bool interactionWithDoor = false;

    bool playerGotKey = false;
    public void Start()
    {
        keyPadScript = Keypad.GetComponent<KeyPad>();
        firstPersonController = GetComponent<FirstPersonController>();
        CodeCanvas.enabled = false;
    }
    private void Update()
    {
       //print(Door);
        if (interactionWithKey && Input.GetKeyDown(KeyCode.E))
        {
            GotKeyMessage();
            playerGotKey = true;
            if (Key != null) Destroy(Key);
        }

        if (interactionWithDoor && Input.GetKeyDown(KeyCode.E))
        {
            // Verificar si la puerta tiene el componente DoorController (puerta bloqueada)
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

            // Verificar si la puerta tiene el componente NormalDoorBehavior (puerta normal)
            NormalDoorBehavior normalDoor = Door.GetComponent<NormalDoorBehavior>();
            if (normalDoor != null)
            {
                normalDoor.Toggle(); // Alternar entre abrir y cerrar
                return;
            }
        }
    }
    private void HandleCodeDoor(CodeDoorBehavior codeDoor)
    {
        if(keyPadScript.openTheDoor == false)
        {
            CodeCanvas.enabled = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    private void HandleLockedDoor(DoorController lockedDoor)
    {
        // Desbloquear la puerta si el jugador tiene la llave
        if (playerGotKey && !lockedDoor.isUnlocked)
        {
            UnlockDoorMessage();
            lockedDoor.Unlock();
            playerGotKey = false; // La llave se consume
            return;
        }

        // Alternar entre abrir y cerrar la puerta si está desbloqueada y no está animando
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
        Door = interactCollider.gameObject; // Actualizar la referencia a la puerta
        if (interactCollider.CompareTag("Key"))
            interactionWithKey = true;

        if (interactCollider.CompareTag("LockedDoor") || interactCollider.CompareTag("Door")|| interactCollider.CompareTag("CodeDoor"))
        {
            interactionWithDoor = true;         
        }
    }

    private void OnTriggerExit(Collider interactCollider)
    {
        if (interactCollider.CompareTag("Key"))
            interactionWithKey = false;

        if (interactCollider.CompareTag("LockedDoor") || interactCollider.CompareTag("Door") || interactCollider.CompareTag("CodeDoor"))
        {
            print("B");
            
            interactionWithDoor = false;
            
        }
    }
}