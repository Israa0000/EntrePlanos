using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RespawnAfterEnemy : MonoBehaviour
{
    [Header("Respawn")]
    [SerializeField] Transform respawnPoint;
    [SerializeField] string enemyTag = "3DENEMY";

    [Header("Screamer")]
    [SerializeField] GameObject screamerImage;
    [SerializeField] AudioClip screamerSound;
    [SerializeField] float screamerDuration = 1f;
    [SerializeField] Canvas screamerCanvas; // ? añade este campo

    private Transform playerTransform;
    private CharacterController cc;
    private Rigidbody rb;
    private AudioSource audioSource;
    private bool isRespawning = false; // evita triggerearlo dos veces

    private void Awake()
    {
        playerTransform = transform.root;
        cc = playerTransform.GetComponent<CharacterController>();
        rb = playerTransform.GetComponent<Rigidbody>();

        // Obtiene el Canvas padre de la imagen
        if (screamerImage != null)
        {
            screamerCanvas = screamerImage.GetComponentInParent<Canvas>();
            if (screamerCanvas != null)
            {
                DontDestroyOnLoad(screamerCanvas.gameObject); // sobrevive cambios de escena
            }
        }

        GameObject audioObj = new GameObject("ScreamerAudio");
        DontDestroyOnLoad(audioObj);
        audioSource = audioObj.AddComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(enemyTag))
        {
            // Para todo lo que esté en curso y empieza de cero
            StopAllCoroutines();
            audioSource.Stop();

            if (screamerImage != null)
                screamerImage.SetActive(false);

            StartCoroutine(ScreamerThenRespawn());
        }
    }

    private IEnumerator ScreamerThenRespawn()
    {
        isRespawning = true;

        // Fuerza el Canvas activo antes de todo
        if (screamerCanvas != null)
            screamerCanvas.gameObject.SetActive(true);

        if (screamerImage != null)
            screamerImage.SetActive(true);

        if (audioSource != null && screamerSound != null)
        {
            audioSource.clip = screamerSound;
            audioSource.PlayScheduled(AudioSettings.dspTime + 0.01);
        }

        yield return new WaitForSeconds(screamerDuration);

        if (audioSource != null)
            audioSource.Stop();

        if (screamerImage != null)
            screamerImage.SetActive(false);

        // NO desactives el Canvas, solo la imagen

        Respawn();

        isRespawning = false;
    }


    private void Respawn()
    {
        if (respawnPoint == null)
        {
            Debug.LogWarning("¡Respawn Point no asignado!");
            return;
        }

        if (cc != null)
        {
            cc.enabled = false;
            playerTransform.position = respawnPoint.position;
            cc.enabled = true;
        }
        else if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.MovePosition(respawnPoint.position);
        }
        else
        {
            playerTransform.position = respawnPoint.position;
        }
    }
}