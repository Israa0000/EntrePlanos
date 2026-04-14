using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SpritesBehavior : MonoBehaviour
{
    [SerializeField] GameObject CrossHair;
    [SerializeField] Sprite OpenDoorSprite;
    [SerializeField] Sprite LockedDoorSprite;
    [SerializeField] Sprite CodeDoorSprite;
    [SerializeField] Sprite PickUpSprite;
    DoorController lockedDoorScript;
    Image crosshairImage; 

    void Start()
    {
        crosshairImage = CrossHair.GetComponent<Image>();
    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider interactCollider)
    {
        if (interactCollider.CompareTag("LockedDoor"))
        {
            lockedDoorScript = interactCollider.gameObject.GetComponent<DoorController>();
            if (lockedDoorScript.isUnlocked == true)
            {
                crosshairImage.sprite = OpenDoorSprite;
            }
            if (lockedDoorScript.isUnlocked == false)
            {
                crosshairImage.sprite = LockedDoorSprite;
            }
        }

        if (interactCollider.CompareTag("Door"))
        {
            crosshairImage.sprite = OpenDoorSprite;
        }

        if (interactCollider.CompareTag("CodeDoor"))
        {
            CrossHair.GetComponent<RectTransform>().localScale = new Vector3(20f, 20f, 20f);
            crosshairImage.sprite = CodeDoorSprite;
        }
    }
    private void OnTriggerExit(Collider interactCollider)
    {
        if (interactCollider.CompareTag("CodeDoor"))
        {
            CrossHair.GetComponent<RectTransform>().localScale = new Vector3(10f, 10f, 10f);
            crosshairImage.sprite = CodeDoorSprite;
        }
    }
}