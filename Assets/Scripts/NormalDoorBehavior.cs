using System.Collections;
using UnityEngine;

public class NormalDoorBehavior : MonoBehaviour
{
    [Tooltip("Posición absoluta para puerta abierta (X/Z usados). Y se sustituye por la Y inicial de la puerta.")]
    [SerializeField] Vector3 absoluteOpenPosition = new Vector3(-18.715f, 0f, 2.644f);

    [Tooltip("Posición absoluta para puerta cerrada (X/Z usados). Y se sustituye por la Y inicial de la puerta.")]
    [SerializeField] Vector3 absoluteClosedPosition = new Vector3(-18.06f, 0f, 1.834f);

    [Tooltip("Si true, interpola también la rotación (añade openAngle a la rotación inicial en Y).")]
    [SerializeField] bool lerpRotation = true;
    [SerializeField] float openAngle = 90f;
    [SerializeField] public float doorSpeed = 2f;

    [SerializeField] AudioClip openSound;
    [SerializeField] AudioClip closeSound;

    public bool isLocked = false;

    Vector3 openPos;
    Vector3 closedPos;
    Quaternion openRot;
    Quaternion closedRot;

    public bool isOpen { get; private set; } = false;
    public bool isAnimating { get; private set; } = false;

    AudioSource audioSource;
    float doorAnimationTime; // Tiempo total de la animación

    void Start()
    {
        var startPos = transform.position;
        closedPos = new Vector3(absoluteClosedPosition.x, startPos.y, absoluteClosedPosition.z);
        openPos = new Vector3(absoluteOpenPosition.x, startPos.y, absoluteOpenPosition.z);

        closedRot = transform.rotation;
        openRot = lerpRotation ? Quaternion.Euler(closedRot.eulerAngles.x, closedRot.eulerAngles.y + openAngle, closedRot.eulerAngles.z) : closedRot;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void Toggle()
    {
        if (isAnimating || isLocked) return; // Añade isLocked
        StartCoroutine(ToggleDoor(!isOpen, true));
    }

    public void Open()
    {
        if (isAnimating || isOpen || isLocked) return; // Añade isLocked
        StartCoroutine(ToggleDoor(true, true));
    }

    public void Close(bool force = false, bool playSound = true)
    {
        if (isAnimating || (!isOpen && !force)) return;
        StartCoroutine(ToggleDoor(false, playSound));
    }

    public IEnumerator ToggleDoor(bool open, bool playSound)
    {
        isAnimating = true;

        // Reproducir sonido correspondiente si playSound es true
        if (playSound && audioSource != null)
        {
            if (open && openSound != null)
                audioSource.PlayOneShot(openSound);
            else if (!open && closeSound != null)
                audioSource.PlayOneShot(closeSound);
        }

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
