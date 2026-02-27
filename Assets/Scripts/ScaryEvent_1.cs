using UnityEngine;
using System.Collections;

public class ScaryEvent_1 : MonoBehaviour
{
    [SerializeField] NormalDoorBehavior puerta;
    [SerializeField] float scaryDoorSpeed = 8f;
    [SerializeField] float normalDoorSpeed = 0.4651008f;
    [SerializeField] float lockTime = 10f;
    [SerializeField] GameObject light;

    private bool eventoActivado = false;

    void OnTriggerEnter(Collider other)
    {
        if (!eventoActivado && other.CompareTag("Player"))
        {
            eventoActivado = true;
            light.SetActive(false); // Apaga la luz
            StartCoroutine(CerrarYPonerRapida());
        }
    }

    IEnumerator CerrarYPonerRapida()
    {
        puerta.doorSpeed = scaryDoorSpeed;
        puerta.isLocked = true;
        puerta.StartCoroutine("ToggleDoor", false); // Fuerza el cierre

        yield return new WaitForSeconds(lockTime);

        puerta.doorSpeed = normalDoorSpeed;
        puerta.isLocked = false;
    }
}

