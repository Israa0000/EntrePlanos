using UnityEngine;

public enum MovementType
{
    Movement3D,
    Movement2D
}

public class CameraSwitcher : MonoBehaviour
{
    public Camera cameraA;
    public Camera cameraB;
    public KeyCode switchKey = KeyCode.V;
    public MovementType movement = MovementType.Movement2D;
    bool useA;

    // Referencias a los controladores: asigna en el Inspector
    [Header("Controladores")]
    [Tooltip("Componente de movimiento 2D (ej. script 'movimiento')")]
    public MonoBehaviour controller2D;
    [Tooltip("Componente de movimiento 3D (ej. FirstPersonController)")]
    public MonoBehaviour controller3D;

    void Start()
    {
        if (cameraA != null) cameraA.enabled = true;
        if (cameraB != null) cameraB.enabled = false;

        // Asegura estado inicial de controladores
        ApplyControlState();
    }

    void Update()
    {
        if (Input.GetKeyDown(switchKey))
        {
            ToggleCameras();
            ToggleControls();
        }
    }

    public void ToggleCameras()
    {
        if (cameraA == null && cameraB == null) return;

        if (cameraA == null)
        {
            cameraB.enabled = !cameraB.enabled;
            return;
        }
        if (cameraB == null)
        {
            cameraA.enabled = !cameraA.enabled;
            return;
        }

        useA = !cameraA.enabled;
        cameraA.enabled = useA;
        cameraB.enabled = !useA;
    }

    public void ToggleControls()
    {
        if (cameraA != null && cameraA.enabled)
        {
            movement = MovementType.Movement2D;
        }
        else if (cameraB != null && cameraB.enabled)
        {
            movement = MovementType.Movement3D;
        }

        ApplyControlState();
    }

    void ApplyControlState()
    {
        // Activa/desactiva componentes asignados según movement
        if (controller2D != null) controller2D.enabled = (movement == MovementType.Movement2D);
        if (controller3D != null) controller3D.enabled = (movement == MovementType.Movement3D);
    }

    // Método público para forzar un modo (útil desde RedPill)
    public void ForceMovement(MovementType mode)
    {
        movement = mode;

        if (cameraA != null && cameraB != null)
        {
            bool useAflag = (mode == MovementType.Movement2D);
            cameraA.enabled = useAflag;
            cameraB.enabled = !useAflag;
        }

        ApplyControlState();
    }
}