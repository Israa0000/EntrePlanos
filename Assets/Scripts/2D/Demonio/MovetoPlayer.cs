using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MovetoPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject player;
    [SerializeField] float speed;
    [SerializeField] int scene;
    [SerializeField] AudioClip demonSound;
    public bool shouldMove = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(shouldMove)
        {
            MoveToPlayer();
             AudioController.Instance.PlaySound(demonSound);
        }
    }

    public void MoveToPlayer()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
        print("Player hit");
            SceneManager.LoadScene(scene);
        }
    }

    
}
