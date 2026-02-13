using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Tooltip("Posición absoluta para puerta abierta (X/Z usados). Y se sustituye por la Y inicial de la puerta.")]
    [SerializeField] Vector3 absoluteOpenPosition = new Vector3(-18.715f, 0f, 2.644f);

    [Tooltip("Posición absoluta para puerta cerrada (X/Z usados). Y se sustituye por la Y inicial de la puerta.")]
    [SerializeField] Vector3 absoluteClosedPosition = new Vector3(-18.06f, 0f, 1.834f);

    [Tooltip("Si true, interpola también la rotación (añade openAngle a la rotación inicial en Y).")]
    [SerializeField] bool lerpRotation = true;
    [SerializeField] float openAngle = 90f;

    [SerializeField] float doorSpeed = 2f;

    Vector3 startPos;
    Vector3 openPos;
    Vector3 closedPos;

    Quaternion startRot;
    Quaternion openRot;
    Quaternion closedRot;

    public bool isUnlocked { get; private set; } = false;
    public bool isOpen { get; private set; } = false;
    public bool isAnimating { get; private set; } = false;

    void Start()
    {
        startPos = transform.position;

        // Mantener la Y original, usar X/Z de las posiciones absolutas
        openPos = new Vector3(absoluteOpenPosition.x, startPos.y, absoluteOpenPosition.z);
        closedPos = new Vector3(absoluteClosedPosition.x, startPos.y, absoluteClosedPosition.z);

        startRot = transform.rotation;
        closedRot = startRot;
        openRot = lerpRotation ? Quaternion.Euler(startRot.eulerAngles.x, startRot.eulerAngles.y + openAngle, startRot.eulerAngles.z) : startRot;
    }

    public void Unlock()
    {
        isUnlocked = true;
        Debug.Log("puerta desbloqueada (DoorController)");
    }

    public void Toggle()
    {
        if (!isUnlocked || isAnimating) return;
        StartCoroutine(ToggleDoor(!isOpen));
    }

    public void Open()
    {
        if (!isUnlocked || isAnimating || isOpen) return;
        StartCoroutine(ToggleDoor(true));
    }

    public void Close()
    {
        if (isAnimating || !isOpen) return;
        StartCoroutine(ToggleDoor(false));
    }

    IEnumerator ToggleDoor(bool open)
    {
        isAnimating = true;

        Vector3 fromPos = transform.position;
        Quaternion fromRot = transform.rotation;

        Vector3 toPos = open ? openPos : closedPos;
        Quaternion toRot = open ? openRot : closedRot;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * doorSpeed;
            transform.position = Vector3.Lerp(fromPos, toPos, t);
            if (lerpRotation)
                transform.rotation = Quaternion.Lerp(fromRot, toRot, t);
            yield return null;
        }

        // Asegurar valores finales exactos
        transform.position = toPos;
        if (lerpRotation) transform.rotation = toRot;

        isOpen = open;
        isAnimating = false;
        Debug.Log(open ? "puerta abierta (DoorController)" : "puerta cerrada (DoorController)");
    }
}