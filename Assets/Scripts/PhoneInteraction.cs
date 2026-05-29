using System.Collections;
using UnityEngine;

public class PhoneInteraction : MonoBehaviour
{
    [Header("Audios")]
    [SerializeField] AudioClip ringSound;
    [SerializeField] AudioClip conversationSound;

    [Header("Configuración")]
    [SerializeField] float interactDistance = 2f;
    [SerializeField] string promptMessage = "Pulsa E para contestar";

    private AudioSource audioSource;
    private Transform playerTransform;
    private bool hasBeenAnswered = false;
    private bool playerNearby = false;
    private bool isActive = false; // empieza desactivado

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // No suena hasta que se completen los engranajes
        audioSource.loop = true;
        audioSource.clip = ringSound;
    }

    // Lo llama GearPanel cuando se completa el puzle
    public void ActivatePhone()
    {
        isActive = true;
        audioSource.Play();
        Debug.Log("¡Teléfono activado! Sonando...");
    }

    private void Update()
    {
        if (!isActive || hasBeenAnswered) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        playerNearby = distance <= interactDistance;

        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            AnswerPhone();
        }
    }

    private void OnGUI()
    {
        if (playerNearby && isActive && !hasBeenAnswered)
        {
            GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 50, 200, 30), promptMessage);
        }
    }

    private void AnswerPhone()
    {
        hasBeenAnswered = true;

        audioSource.loop = false;
        audioSource.Stop();
        audioSource.clip = conversationSound;
        audioSource.Play();

        Debug.Log("Teléfono contestado");
    }
}
