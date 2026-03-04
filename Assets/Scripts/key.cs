using UnityEngine;


public class Key : MonoBehaviour, IPickable
{
    [SerializeField] string playerTag = "Player";
    [SerializeField] float dropDistance = 1f;
    [SerializeField] Vector3 holdLocalPosition;

    bool isPicked = false;
    bool playerInRange = false;
    GameObject currentPicker;
    Rigidbody rb;
    Collider col;

    public bool IsPicked => isPicked;

    void Awake()
    {
        holdLocalPosition = new Vector3(0f, -0.25f, 0.8f);
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isPicked)
            {
                if (playerInRange)
                {
                    GameObject picker = currentPicker != null ? currentPicker : FindPlayerInRange();
                    if (picker != null) OnPickUp(picker);
                }
            }
            else
            {
                OnDrop();
            }
        }
    }

    GameObject FindPlayerInRange()
    {
        // fallback: busca la primera GameObject con la etiqueta playerTag
        return GameObject.FindWithTag(playerTag);
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

        Transform holdParent = transform.parent;
        transform.SetParent(null);

        Vector3 forward = Vector3.forward;
        if (holdParent != null) forward = holdParent.forward;
        else if (Camera.main != null) forward = Camera.main.transform.forward;

        Vector3 dropPosition = (holdParent != null ? holdParent.position : transform.position) 
            + forward.normalized 
            * dropDistance;
        transform.position = dropPosition;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.velocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.None;
        }

        

        if (col != null) col.enabled = true;

        isPicked = false;
        currentPicker = null;
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