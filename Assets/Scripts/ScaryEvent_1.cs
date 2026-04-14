using UnityEngine;
using System.Collections;

public class ScaryEvent_1 : MonoBehaviour
{
    [SerializeField] NormalDoorBehavior puerta;
    [SerializeField] float scaryDoorSpeed = 8f;
    [SerializeField] float normalDoorSpeed = 0.4651008f;
    [SerializeField] float lockTime = 10f;
    [SerializeField] GameObject light;
    [SerializeField] GameObject lamp;
    [SerializeField] AudioClip fastCloseSound;
    [SerializeField] AudioClip bulbBreakSound;
    [SerializeField] AudioClip runingSound;
    [SerializeField] AudioClip cryingSound;
    [SerializeField] Vector3 lampFinalPosition;
    [SerializeField] Quaternion lampFinalRotation;

    Transform lampTransform;
    private bool eventoActivado = false;
    private AudioSource audioSource;

    void Start()
    {
        lampTransform = lamp.transform;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!eventoActivado && other.CompareTag("Player"))
        {
            eventoActivado = true;
            light.SetActive(false); // Apaga la luz
            lampTransform.position = lampFinalPosition;
            lampTransform.rotation = lampFinalRotation;
            StartCoroutine(CerrarYPonerRapida());
        }
    }

    IEnumerator CerrarYPonerRapida()
    {
        puerta.doorSpeed = scaryDoorSpeed;
        puerta.isLocked = true;

        // Llamar directamente a la corrutina ToggleDoor con los parámetros correctos
        yield return StartCoroutine(puerta.ToggleDoor(false, false)); // Cerrar la puerta sin reproducir el sonido normal

        // Reproducir el sonido de cierre rápido
        if (fastCloseSound != null)
            audioSource.PlayOneShot(fastCloseSound);

        // Esperar a que termine el sonido de cierre rápido antes de romper la bombilla
        if (fastCloseSound != null)
            yield return new WaitForSeconds(fastCloseSound.length);
        else
            yield return null;

        // Reproducir el sonido de bombilla rota
        if (bulbBreakSound != null) {
            audioSource.PlayOneShot(bulbBreakSound);
            yield return new WaitForSeconds(1f);
        }
        // Reproducir el sonido de cryingSound durante 5 segundos
        if (cryingSound != null)
        {
            audioSource.clip = cryingSound;
            audioSource.loop = false; // No repetir el sonido
            audioSource.Play();
            yield return new WaitForSeconds(5f); // Esperar 5 segundos
        }

        // Reproducir el sonido de runingSound durante 5 segundos
        if (runingSound != null)
        {
            audioSource.clip = runingSound;
            audioSource.loop = false; // No repetir el sonido
            audioSource.Play();
            yield return new WaitForSeconds(5f); // Esperar 5 segundos
        }

        if (fastCloseSound != null)
            audioSource.PlayOneShot(fastCloseSound);
            yield return new WaitForSeconds(fastCloseSound.length);

        puerta.doorSpeed = normalDoorSpeed;
        puerta.isLocked = false;
    }
}

