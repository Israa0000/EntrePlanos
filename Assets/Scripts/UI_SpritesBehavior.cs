using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SpritesBehavior : MonoBehaviour
{
    //[SerializeField] List<GameObject> sprites;
    [SerializeField] GameObject CrossHair;
    [SerializeField] Sprite OpenDoorSprite;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider interactCollider)
    {
        if (interactCollider.CompareTag("LockedDoor"))
        {
            
        }

        if (interactCollider.CompareTag("Door"))
        {

        }
        if (interactCollider.CompareTag("CodeDoor"))
        {

        }

    }

}
