using UnityEngine;

public class Gear : MonoBehaviour, IPickable
{
    [SerializeField] string playerTag = "Player";
    [SerializeField] float dropDistance = 1f;
    [SerializeField] Vector3 holdLocalPosition = new Vector3(0f, -0.25f, 0.8f);

    bool isPicked = false;
    bool playerInRange = false;
    GameObject currentPicker;
    Rigidbody rb;
    Collider col;

    public bool IsPicked => isPicked;
    public string GearTag => gameObject.tag;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isPicked && playerInRange)
            {
                GameObject picker = currentPicker ?? GameObject.FindWithTag(playerTag);
                if (picker != null) OnPickUp(picker);
            }
            else if (isPicked)
            {
                OnDrop();
            }
        }
    }

    public void OnPickUp(GameObject picker)
    {
        if (isPicked) return;

        isPicked = true;
        currentPicker = picker;

        Transform holdParent = picker.GetComponentInChildren<Camera>()?.transform ?? picker.transform;
        transform.SetParent(holdParent, worldPositionStays: false);
        transform.localPosition = holdLocalPosition;
        transform.localRotation = Quaternion.identity;

        if (rb != null) rb.isKinematic = true;
        if (col != null) col.enabled = false;
    }

    public void OnDrop()
    {
        if (!isPicked) return;

        transform.SetParent(null);

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.velocity = Vector3.zero;
            rb.useGravity = true;
        }

        if (col != null) col.enabled = true;

        isPicked = false;
        currentPicker = null;
    }

    public void PlaceInPanel()
    {
        isPicked = false;
        currentPicker = null;
        if (rb != null) rb.isKinematic = true;
        if (col != null) col.enabled = false;
        enabled = false; // Desactiva el Update de este script
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = true;
            currentPicker = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
            if (!isPicked) currentPicker = null;
        }
    }
}
