using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeDoorBehavior : MonoBehaviour
{
    [Tooltip("Posición absoluta para puerta abierta (X/Z usados). Y se sustituye por la Y inicial de la puerta.")]
    [SerializeField] Vector3 absoluteOpenPosition = new Vector3(-18.715f, 0f, 2.644f);

    [Tooltip("Posición absoluta para puerta cerrada (X/Z usados). Y se sustituye por la Y inicial de la puerta.")]
    [SerializeField] Vector3 absoluteClosedPosition = new Vector3(-18.06f, 0f, 1.834f);

    [Tooltip("Si true, interpola también la rotación (añade openAngle a la rotación inicial en Y).")]
    [SerializeField] bool lerpRotation = true;
    [SerializeField] float openAngle = 90f;

    [SerializeField] float doorSpeed = 2f;


    [Tooltip("El Keypad Aquí")]
    [SerializeField] GameObject KeyPad;
    KeyPad keypad;

    Vector3 openPos;
    Vector3 closedPos;
    Quaternion openRot;
    Quaternion closedRot;

    public bool isOpen { get; private set; } = false;
    public bool isAnimating { get; private set; } = false;
    // Start is called before the first frame update
    void Start()
    {
        keypad = KeyPad.GetComponent<KeyPad>();
        var startPos = transform.position;
        closedPos = new Vector3(absoluteClosedPosition.x, startPos.y, absoluteClosedPosition.z);
        openPos = new Vector3(absoluteOpenPosition.x, startPos.y, absoluteOpenPosition.z);

        closedRot = transform.rotation;
        openRot = lerpRotation ? Quaternion.Euler(closedRot.eulerAngles.x, closedRot.eulerAngles.y + openAngle, closedRot.eulerAngles.z) : closedRot;
    }

    public void Toggle()
    {

        if (keypad.openTheDoor)
        {
            KeyPad.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            if (isAnimating) return;
            StartCoroutine(ToggleDoor(!isOpen));
        }
    }

    public void Open()
    {
        if (isAnimating || isOpen) return;
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

        transform.position = toPos;
        if (lerpRotation) transform.rotation = toRot;

        isOpen = open;
        isAnimating = false;
    }
}
