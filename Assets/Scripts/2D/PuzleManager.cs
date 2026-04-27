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
    void Start()
    {
        
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
        }
    }
}
