using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraPosChanguer : MonoBehaviour
{
    



    public void ChangeCameraPos(Vector3 newPos)
    {
        transform.position = newPos;
    }
    
}
