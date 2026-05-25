using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class PuzleActivator : MonoBehaviour
{
    [SerializeField] Sprite buttonPressedBlue;
     [SerializeField] Sprite buttonPressedRed;
    Sprite buttonUnpressed;
    [SerializeField] bool isBlue;
    [SerializeField] GameObject puzleManagerGO;
    [SerializeField] MovetoPlayer demonScript;
    [SerializeField] bool demonTime;
    [SerializeField] AudioClip activateBtn;
    [SerializeField] GameObject screamColider;
    private PuzleManager puzleManager;
    private LampManager blueLampManager;
    private LampManager redLampManager;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        buttonUnpressed = spriteRenderer.sprite;
        puzleManager = puzleManagerGO.GetComponent<PuzleManager>();
        blueLampManager = puzleManager.blueLight.GetComponent<LampManager>();
        redLampManager = puzleManager.redLight.GetComponent<LampManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    { 
        if (collision.CompareTag("Player"))
        {
            if(isBlue == true)
            {
                spriteRenderer.sprite = buttonPressedBlue;
                puzleManager.setBlueIsPressed(true);
                blueLampManager.LightOn();
            }
            else {
                spriteRenderer.sprite = buttonPressedRed;
                puzleManager.setRedIsPressed(true);
                redLampManager.LightOn();
            }
            if (demonTime)
            {
                demonScript.shouldMove = true;
                print("activado");
            }
            if(screamColider != null)
            {
                screamColider.SetActive(true);
            }
            AudioController.Instance.PlaySound(activateBtn);
            
        }
    }
    
}
