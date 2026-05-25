using UnityEngine;
using UnityEngine.WSA;

public class FlashlightController : MonoBehaviour
{
    [SerializeField] Light flashlightLight;
    bool isCollected = false;

    void Awake()
    {
        if (flashlightLight != null)
            flashlightLight.enabled = false;
    }

    void Update()
    {
        if (isCollected && Input.GetKeyDown(KeyCode.F))
        {
            flashlightLight.enabled = !flashlightLight.enabled;
        }
    }

    public void Collect()
    {
        isCollected = true;
    }

   
}
