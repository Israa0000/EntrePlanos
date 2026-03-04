using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSystem : MonoBehaviour
{
    const string DefaultSceneName = "MainScene";

    public void Continue()
    {
        string sceneToLoad = DefaultSceneName;

        if (SaveSystem.HasSave())
        {
            string savedScene = SaveSystem.GetSavedScene();
            if (!string.IsNullOrEmpty(savedScene))
            {
                sceneToLoad = savedScene;
                SceneManager.sceneLoaded += OnSceneLoaded_SetPlayer;
            }
        }

        Debug.Log($"MenuSystem: Cargando escena '{sceneToLoad}'");
        SceneManager.LoadScene(sceneToLoad);
    }

    private void OnSceneLoaded_SetPlayer(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded_SetPlayer;

        Vector3 savedPos = SaveSystem.GetSavedPlayerPosition();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = savedPos;
            Debug.Log($"MenuSystem: Jugador reposicionado en {savedPos} en escena '{scene.name}'.");
        }
        else
        {
            Debug.LogWarning("MenuSystem: No se encontró GameObject con tag 'Player'. Asegúrate de etiquetar al jugador.");
        }
    }

    public void Exit()
    {
        Application.Quit();
        Debug.Log("Exiting the game...");
    }
}
