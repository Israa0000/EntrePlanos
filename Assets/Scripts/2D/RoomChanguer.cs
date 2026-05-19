using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class RoomChanguer : MonoBehaviour
{
    public enum Direction { Up, Down, Left, Right }
    public Direction direction;

    [SerializeField] GameObject player;
    [SerializeField] Transform spawnPoint;
    [SerializeField] int cameraDistance = 20;

    [SerializeField] List<GameObject> enemies;   // Lista de enemigos
    [SerializeField] bool toOut = false;
    [SerializeField] bool activateDemon;
    GameObject cameraa;
    CameraPosChanguer cameraPosChanguer;
    [SerializeField] MovetoPlayer moveToPlayer;

    void Start()
    {
        cameraa = GameObject.Find("Main Camera");
        cameraPosChanguer = cameraa.GetComponent<CameraPosChanguer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        // ACTIVAR / DESACTIVAR TODOS LOS ENEMIGOS
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
                enemy.SetActive(toOut);
        }

        // MOVER JUGADOR Y CÁMARA
        player.transform.position = spawnPoint.position;

        switch (direction)
        {
            case Direction.Up:
                cameraPosChanguer.ChangeCameraPos(
                    new Vector3(cameraa.transform.position.x, cameraa.transform.position.y + cameraDistance, cameraa.transform.position.z));
                break;

            case Direction.Down:
                cameraPosChanguer.ChangeCameraPos(
                    new Vector3(cameraa.transform.position.x, cameraa.transform.position.y - cameraDistance, cameraa.transform.position.z));
                break;

            case Direction.Left:
                cameraPosChanguer.ChangeCameraPos(
                    new Vector3(cameraa.transform.position.x - cameraDistance, cameraa.transform.position.y, cameraa.transform.position.z));
                break;

            case Direction.Right:
                cameraPosChanguer.ChangeCameraPos(
                    new Vector3(cameraa.transform.position.x + cameraDistance, cameraa.transform.position.y, cameraa.transform.position.z));
                break;
        }

        if(activateDemon == true)
        {
            if(moveToPlayer != null)
            {
                moveToPlayer.shouldMove = true;
            }
        }
    }

    // LLAMADO POR CADA ENEMIGO AL MORIR
    public void NotifyEnemyDied(GameObject enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
        }
    }
}
