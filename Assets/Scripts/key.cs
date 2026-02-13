using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class key : MonoBehaviour, IPickable
{
    public void PickUp()
    {
        if(Input.GetKeyDown(KeyCode.E))
            gameObject.SetActive(false);
        Debug.Log("Has recogido la llave");
    }

    public void Drop()
    {
        gameObject.SetActive(true);
        Debug.Log("Has soltado la llave");
    }
}
