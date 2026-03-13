
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SaveSystem
{
    const string KeyScene = "Save_Scene";
    const string KeyPosX = "Save_PosX";
    const string KeyPosY = "Save_PosY";
    const string KeyPosZ = "Save_PosZ";

    public static void SavePlayerPosition(Vector3 position)
    {
        PlayerPrefs.SetString(KeyScene, SceneManager.GetActiveScene().name);
        PlayerPrefs.SetFloat(KeyPosX, position.x);
        PlayerPrefs.SetFloat(KeyPosY, position.y);
        PlayerPrefs.SetFloat(KeyPosZ, position.z);
        PlayerPrefs.Save();
        Debug.Log($"SaveSystem: Guardado en escena '{SceneManager.GetActiveScene().name}' pos {position}");
    }

    public static bool HasSave()
    {
        return PlayerPrefs.HasKey(KeyScene);
    }

    public static string GetSavedScene()
    {
        return PlayerPrefs.GetString(KeyScene, string.Empty);
    }

    public static Vector3 GetSavedPlayerPosition()
    {
        float x = PlayerPrefs.GetFloat(KeyPosX, 0f);
        float y = PlayerPrefs.GetFloat(KeyPosY, 0f);
        float z = PlayerPrefs.GetFloat(KeyPosZ, 0f);
        return new Vector3(x, y, z);
    }

    public static void ClearSave()
    {
        PlayerPrefs.DeleteKey(KeyScene);
        PlayerPrefs.DeleteKey(KeyPosX);
        PlayerPrefs.DeleteKey(KeyPosY);
        PlayerPrefs.DeleteKey(KeyPosZ);
        PlayerPrefs.Save();
        Debug.Log("SaveSystem: Save cleared.");
    }
}