using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanguer : MonoBehaviour
{
    
    public void changuerScene(string salaDestino)
    {
        PlayerPrefs.SetString("Sala3D", salaDestino);
        SceneManager.LoadScene("isra_scene");
    }
}
