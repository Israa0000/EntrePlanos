using System.Collections;
using UnityEngine;

public class RespawnAfterEnemy : MonoBehaviour
{
    [Header("Respawn")]
    [SerializeField] Transform respawnPoint;
    [SerializeField] string enemyTag = "3DENEMY";

    [Header("Screamer")]
    [SerializeField] GameObject screamerImage;
    [SerializeField] AudioClip screamerSound;
    [SerializeField] float screamerDuration = 1f;
    [SerializeField] Canvas screamerCanvas;

    private Transform playerTransform;
    private CharacterController cc;
    private Rigidbody rb;
    private AudioSource audioSource;

    // ? Tag único para no duplicar el objeto de audio
    private const string AUDIO_OBJ_NAME = "ScreamerAudio_Persistent";

    private void Awake()
    {
        playerTransform = transform.root;
        cc = playerTransform.GetComponent<CharacterController>();
        rb = playerTransform.GetComponent<Rigidbody>();

        if (screamerCanvas == null && screamerImage != null)
            screamerCanvas = screamerImage.transform.parent?.GetComponent<Canvas>();

        // ? Solo crea el objeto de audio si NO existe ya
        GameObject audioObj = GameObject.Find(AUDIO_OBJ_NAME);
        if (audioObj == null)
        {
            audioObj = new GameObject(AUDIO_OBJ_NAME);
            DontDestroyOnLoad(audioObj);
        }
        audioSource = audioObj.GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = audioObj.AddComponent<AudioSource>();

        // Estado inicial limpio
        if (screamerCanvas != null)
            screamerCanvas.gameObject.SetActive(true);
        if (screamerImage != null)
            screamerImage.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(enemyTag)) return;

        StopAllCoroutines();

        // ? Verifica que audioSource sigue vivo antes de usarlo
        if (audioSource != null)
            audioSource.Stop();

        if (screamerImage != null)
            screamerImage.SetActive(false);

        StartCoroutine(ScreamerThenRespawn());
    }

    private IEnumerator ScreamerThenRespawn()
    {
        if (screamerCanvas != null)
            screamerCanvas.gameObject.SetActive(true);

        yield return null; // frame para que Unity procese el Canvas

        if (screamerImage != null)
            screamerImage.SetActive(true);

        if (audioSource != null && screamerSound != null)
        {
            audioSource.clip = screamerSound;
            audioSource.Play();
        }

        yield return new WaitForSeconds(screamerDuration);

        if (screamerImage != null)
            screamerImage.SetActive(false);

        if (audioSource != null)
            audioSource.Stop();

        Respawn();
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