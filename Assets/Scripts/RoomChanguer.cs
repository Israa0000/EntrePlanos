using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomChanguer : MonoBehaviour

{
    // Start is called before the first frame update

    public enum Direction { Up, Down, Left, Right }
    public Direction direction;
    [SerializeField] GameObject player;
    GameObject camera;
    CameraPosChanguer cameraPosChanguer;

    void Start()
    {
        camera = GameObject.Find("Main Camera");
        cameraPosChanguer = camera.GetComponent<CameraPosChanguer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        switch (direction)
        {
            case Direction.Up:
                player.transform.position += new Vector3(0, 1, 0);
                break;

            case Direction.Down:
                player.transform.position += new Vector3(0, -1, 0);
                break;

            case Direction.Left:
                player.transform.position += new Vector3(-1, 0, 0);
                cameraPosChanguer.ChangeCameraPos(new Vector3(camera.transform.position.x - 20, camera.transform.position.y, camera.transform.position.z));
                break;

            case Direction.Right:
                player.transform.position += new Vector3(2, 0, 0);
                cameraPosChanguer.ChangeCameraPos(new Vector3(camera.transform.position.x + 27, camera.transform.position.y, camera.transform.position.z));
                break;
        }
    }
}
