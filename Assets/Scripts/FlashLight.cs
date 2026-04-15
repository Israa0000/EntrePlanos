using UnityEngine;

public class FlashLight : MonoBehaviour
{
    [SerializeField] string playerTag = "Player";
    [SerializeField] Light flashlightLight; // Light en la cámara del jugador, no en este objeto

    bool isCollected = false;
    bool playerInRange = false;

    void Awake()
    {
        if (flashlightLight != null)
            flashlightLight.enabled = false;
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Collect();
        }

        if (isCollected && Input.GetKeyDown(KeyCode.F))
        {
            flashlightLight.enabled = !flashlightLight.enabled;
        }
    }

    void Collect()
    {
        FlashlightController controller = FindObjectOfType<FlashlightController>();
        if (controller != null) controller.Collect();
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}
