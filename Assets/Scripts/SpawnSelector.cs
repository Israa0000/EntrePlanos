using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSelector : MonoBehaviour
{
    [SerializeField] private Transform spawnSalaA;
    [SerializeField] private Transform spawnSalaB;
    [SerializeField] private Transform spawnSalaC;

    [SerializeField] private GameObject player;

    void Start()
    {
        string sala = PlayerPrefs.GetString("Sala3D", "SalaA");

        switch (sala)
        {
            case "SalaA":
                player.transform.position = spawnSalaA.position;
                break;

            case "SalaB":
                player.transform.position = spawnSalaB.position;
                break;

            case "SalaC":
                player.transform.position = spawnSalaC.position;
                break;
        }
    }
}
