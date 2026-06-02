using UnityEngine;
using System.Collections.Generic;

public class GearPanel : MonoBehaviour
{
    [System.Serializable]
    public class GearSlot
    {
        public string requiredTag;           // Tag del engranaje correcto para esta ranura
        public Transform snapPosition;       // Posición donde se coloca visualmente el engranaje
        public float rotationSpeed = 90f;    // Velocidad de giro cuando está colocado
        [HideInInspector] public bool isPlaced = false;
        [HideInInspector] public GameObject placedGear = null; 
    }
    [SerializeField] GameObject soga;
    Vector3 sogaPosition = new Vector3(-80.0559998f, 1.57599998f, 1.12699997f);
    [SerializeField] string playerTag = "Player";
    [SerializeField] List<GearSlot> slots;

    bool playerInRange = false;
    GameObject currentPlayer;

    void Update()
    {
        if (playerInRange && Input.GetMouseButtonDown(0))
        {
            TryPlaceGear();
        }

        // Girar los engranajes colocados correctamente
        foreach (var slot in slots)
        {
            if (slot.isPlaced && slot.placedGear != null)
            {
                slot.placedGear.transform.Rotate(Vector3.up, slot.rotationSpeed * Time.deltaTime);
            }
        }
    }

    void TryPlaceGear()
    {
        if (currentPlayer == null) return;

        // Busca el engranaje que el jugador tiene en mano
        Camera cam = currentPlayer.GetComponentInChildren<Camera>();
        Transform searchParent = cam != null ? cam.transform : currentPlayer.transform;
        Gear heldGear = searchParent.GetComponentInChildren<Gear>();

        if (heldGear == null || !heldGear.IsPicked)
        {
            Debug.Log("No tienes ningún engranaje en mano.");
            return;
        }

        // Busca la ranura que corresponde al tag del engranaje
        GearSlot targetSlot = slots.Find(s => s.requiredTag == heldGear.GearTag && !s.isPlaced);

        if (targetSlot == null)
        {
            Debug.Log("Este engranaje no encaja aquí o ya está colocado.");
            return;
        }

        // Colocar el engranaje
        PlaceGear(heldGear, targetSlot);
    }

    void PlaceGear(Gear gear, GearSlot slot)
    {
        slot.isPlaced = true;
        slot.placedGear = gear.gameObject;

        gear.PlaceInPanel();

        // Mover el engranaje a la posición de la ranura
        gear.transform.SetParent(slot.snapPosition);
        gear.transform.localPosition = Vector3.zero;
        gear.transform.localRotation = Quaternion.identity;

        Debug.Log($"Engranaje {gear.GearTag} colocado correctamente. Girando...");

        // Comprobar si todos están colocados
        if (AllGearsPlaced())
        {
            OnPuzzleComplete();
        }
    }

    bool AllGearsPlaced()
    {
        foreach (var slot in slots)
        {
            if (!slot.isPlaced) return false;
        }
        return true;
    }

    void OnPuzzleComplete()
    {
        Debug.Log("¡Puzle completado! Todos los engranajes están en su sitio.");
        soga.transform.position = sogaPosition;

        // Activa el teléfono
        PhoneInteraction phone = FindAnyObjectByType<PhoneInteraction>();
        if (phone != null)
            phone.ActivatePhone();
        else
            Debug.LogWarning("No se encontró PhoneInteraction en la escena");
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
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 1.5f);
    }
}
