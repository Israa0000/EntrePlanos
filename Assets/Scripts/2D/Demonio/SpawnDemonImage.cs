using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DemonTrigger : MonoBehaviour
{
    [SerializeField] private GameObject demonImage;
    [SerializeField] private AudioClip demonScream;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        demonImage.SetActive(true);

        // Reproducir sonido
        AudioController.Instance.PlaySound(demonScream);

        // Iniciar corrutina para cambiar de escena
        StartCoroutine(ChangeSceneAfterAudio());
    }

    private IEnumerator ChangeSceneAfterAudio()
    {
        // Espera a que termine el audio
        yield return new WaitForSeconds(demonScream.length);
        // Cambiar de escena
        SceneManager.LoadScene(4);
    }
}


