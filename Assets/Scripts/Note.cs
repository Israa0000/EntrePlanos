using Unity.VisualScripting;
using UnityEngine;

public class Note : MonoBehaviour
{
    [SerializeField] float bobSpeed = 2f;
    [SerializeField] float bobHeight = 0.1f;
    [SerializeField] string playerTag = "Player";
    [TextArea(3, 10)]
    [SerializeField] public string noteText = "Escribe aquí el texto de la nota...";

    Vector3 startPos;

    bool playerInRange = false;
    private void Start()
    {    
        startPos = transform.position;
    }
    void Update()
    {
        transform.position = startPos + Vector3.up * Mathf.Sin(Time.time * bobSpeed) * bobHeight;

        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            NoteManager noteManager = Object.FindAnyObjectByType<NoteManager>();
            if (noteManager != null)
                noteManager.OpenNote(noteText);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
            playerInRange = false;
    }
}
