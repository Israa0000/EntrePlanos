using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSystem : MonoBehaviour
{
    [SerializeField] private string defaultSceneName = "2DStage1";

    [SerializeField] private VHSTransition vhsTransition;

    public void Continue()
    {
        string sceneToLoad = defaultSceneName;

        Debug.Log($"MenuSystem: Cargando escena '{sceneToLoad}' (save system desactivado).");

        if (vhsTransition != null)
        {
            vhsTransition.PlayTransition(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("MenuSystem: vhsTransition es null — cargando escena directamente.");
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    public void Exit()
    {
        Application.Quit();
        Debug.Log("Exiting the game...");
    }
}