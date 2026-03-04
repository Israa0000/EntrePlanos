using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform; // arrastra la Camera (o el pivot de la cabeza) aquí
    public GameObject cameraSwitcher;
    CameraSwitcher cameraSwitcherScript;

    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    public bool allowRun = true;
    public bool moveRelativeToCamera = true; // <- si true, WASD sigue la cámara

    [Header("Mouse Look")]
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 80f; // clamp para pitch
    public bool smoothLook = false;
    public float lookSmoothSpeed = 10f; // mayor = más rápido al suavizar

    [Header("Jump & Gravity")]
    public float gravity = 20f;
    public float jumpSpeed = 7f;

    // [Header("Bools")]
    // public bool useControls;

    CharacterController cc;
    float cameraPitch = 0f;
    float verticalVelocity = 0f;

    private bool controlsEnabled = true; // Nueva variable para habilitar/deshabilitar controles

    void Start()
    {
        cc = GetComponent<CharacterController>();
        cameraSwitcherScript = cameraSwitcher.GetComponent<CameraSwitcher>();
        if (cameraTransform == null)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null) cameraTransform = cam.transform;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void EnableControls(bool enable)
    {
        controlsEnabled = enable;

        // Bloquear o desbloquear el cursor
        Cursor.lockState = enable ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !enable;
    }

    void Update()
    {
        if (!controlsEnabled) return; // Si los controles están deshabilitados, no hacer nada

        if (cameraSwitcherScript.movement == MovementType.Movement3D)
        {
            HandleMouseLook();
            HandleMovement();
        }
    }

    void HandleMouseLook()
    {
        if (!controlsEnabled) return; // Evitar que la cámara se mueva si los controles están deshabilitados

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Yaw: rotar el jugador (eje Y)
        transform.Rotate(Vector3.up * mouseX);

        // Pitch: rotar la cámara localmente (eje X)
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -maxLookAngle, maxLookAngle);

        if (cameraTransform != null)
        {
            Quaternion targetRot = Quaternion.Euler(cameraPitch, 0f, 0f);
            if (smoothLook)
                cameraTransform.localRotation = Quaternion.Slerp(cameraTransform.localRotation, targetRot, lookSmoothSpeed * Time.deltaTime);
            else
                cameraTransform.localRotation = targetRot;
        }
    }

    void HandleMovement()
    {
        if (!controlsEnabled) return; // Evitar que el jugador se mueva si los controles están deshabilitados

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 moveDir;

        if (moveRelativeToCamera && cameraTransform != null)
        {
            // Tomamos forward/right de la cámara pero eliminamos la componente Y
            Vector3 camForward = cameraTransform.forward;
            camForward.y = 0f;
            camForward.Normalize();

            Vector3 camRight = cameraTransform.right;
            camRight.y = 0f;
            camRight.Normalize();

            moveDir = camForward * z + camRight * x;
        }
        else
        {
            // Movimiento relativo al transform del jugador (comportamiento clásico FPS)
            moveDir = transform.forward * z + transform.right * x;
        }

        if (moveDir.sqrMagnitude > 1f) moveDir.Normalize();

        bool running = allowRun && Input.GetKey(KeyCode.LeftShift);
        float speed = running ? runSpeed : walkSpeed;

        // Vertical movement (jump & gravity)
        if (cc.isGrounded)
        {
            if (verticalVelocity < 0f) verticalVelocity = -1f;

            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = jumpSpeed;
            }
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        Vector3 velocity = moveDir * speed + Vector3.up * verticalVelocity;
        cc.Move(velocity * Time.deltaTime);
    }
}
