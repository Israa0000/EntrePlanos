using UnityEngine;

public class VHSPlayer : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] string playerTag = "Player";
    [SerializeField] float interactDistance = 2f;

    [Header("Efectos")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip insertSound;
    [SerializeField] Light tvLight;

    [Header("Susto")]
    [SerializeField] GameObject monster;
    [SerializeField] Light roomLight;
    [SerializeField] AudioClip bulbBreakSound;
    [SerializeField] float timeBeforeMonster = 5f;
    [SerializeField] float monsterVisibleTime = 0.5f;

    bool playerInRange = false;
    bool hasVHS = false;
    GameObject currentPlayer;

    void Update()
    {
        if (playerInRange && !hasVHS && Input.GetMouseButtonDown(0))
        {
            TryInsertVHS();
        }
    }

    void TryInsertVHS()
    {
        if (currentPlayer == null) return;

        // Busca el VHS en los hijos de la cámara del jugador
        Camera cam = currentPlayer.GetComponentInChildren<Camera>();
        Transform searchParent = cam != null ? cam.transform : currentPlayer.transform;

        VHSTape tape = searchParent.GetComponentInChildren<VHSTape>();

        if (tape != null && tape.IsPicked)
        {
            InsertVHS(tape);
        }
        else
        {
            Debug.Log("Necesitas el VHS para usar el reproductor.");
        }
    }

    void InsertVHS(VHSTape tape)
    {
        hasVHS = true;

        tape.InsertIntoPlayer();

        // Sonido
        if (audioSource != null && insertSound != null)
            audioSource.PlayOneShot(insertSound);

        // Luz
        if (tvLight != null)
            tvLight.enabled = true;

        if (monster != null)
            monster.SetActive(false);

        StartCoroutine(MonsterScare());

        Debug.Log("VHS insertado. Reproductor activado.");
    }

    private System.Collections.IEnumerator MonsterScare()
    {
        // Espera 5 segundos con la luz encendida
        yield return new WaitForSeconds(timeBeforeMonster);

        // Aparece el monstruo
        if (monster != null)
            monster.SetActive(true);

        // Espera 0.5 segundos
        yield return new WaitForSeconds(monsterVisibleTime);

        // Desaparece el monstruo y se apaga la luz
        if (monster != null)
            monster.SetActive(false);

        if (tvLight != null)
            tvLight.enabled = false;

        // Sonido de bombilla rota (de la sala)
        if (audioSource != null && bulbBreakSound != null)
            audioSource.PlayOneShot(bulbBreakSound);

        // Apagar la luz de la sala
        if (roomLight != null)
            roomLight.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = true;
            currentPlayer = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
            currentPlayer = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}