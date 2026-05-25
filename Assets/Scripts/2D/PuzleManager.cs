using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzleManager : MonoBehaviour
{
    // Start is called before the first frame update
    public bool blueIsPressed;
    public  bool redIsPressed;
    [SerializeField] public GameObject blueLight;
    [SerializeField] public GameObject redLight;
    [SerializeField] GameObject dorSprite;
    [SerializeField] GameObject roomchanguer;
    SpriteRenderer dorSr;
    Color openDor = new Color(1f, 1f, 1f, 1f); // quita transparencia

    void Start()
    {
        if(dorSprite != null)
        {
           dorSr = dorSprite.GetComponent<SpriteRenderer>();    
        }
           
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setBlueIsPressed(bool value)
    {
        this.blueIsPressed  = value;
        checkPuzzle();
    }

    public void setRedIsPressed(bool value)
    {
        this.redIsPressed = value;
         checkPuzzle();
    }

    public void checkPuzzle()
    {
        if(blueIsPressed == true && redIsPressed == true)
        {
            print("PUZZLE COMPLETED");
            if(dorSprite != null)
            {                
                dorSr.color = openDor;
            }
            roomchanguer.SetActive(true);
        }
    }
}
