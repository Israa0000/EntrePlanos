using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickyRoomChanguer : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] GameObject player;
    [SerializeField] Transform spawnPoint;
    
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        player.transform.position = spawnPoint.transform.position;
    }
   
}
