using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSystem : MonoBehaviour
{
    const string DefaultSceneName = "MainScene";

    public VHSTransition vhsTransition;

    public void Continue()
    {
        string sceneToLoad = DefaultSceneName;

        if (SaveSystem.HasSave())
        {
            string savedScene = SaveSystem.GetSavedScene();
            if (!string.IsNullOrEmpty(savedScene))
            {
                sceneToLoad = savedScene;
            }
        }

        Debug.Log($"MenuSystem: Cargando escena '{sceneToLoad}' con transición VHS");
        vhsTransition.PlayTransition(sceneToLoad);

        SceneManager.sceneLoaded += OnSceneLoaded_SetPlayer;
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
            Debug.LogWarning("MenuSystem: No se encontró GameObject con tag 'Player'.");
        }
    }

    public void Exit()
    {
        Application.Quit();
        Debug.Log("Exiting the game...");
    }
}