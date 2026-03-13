using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using System.Collections;

public class VHSTransition : MonoBehaviour
{
    [Header("Canvas References")]
    public Canvas menuCanvas; 
    public Canvas vhsCanvas;
    public Image fadeImage;

    [Header("Volume References")]
    public Volume transitionVolume;

    [Header("Transition Settings")]
    public float transitionSpeed = 0.5f; 
    public float grainIntensityMax = 1.5f;

    private FilmGrain grain;
    private ChromaticAberration chroma;
    private Vignette vignette;
    private ColorAdjustments colorAdjust;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        transitionVolume.profile.TryGet(out grain);
        transitionVolume.profile.TryGet(out chroma);
        transitionVolume.profile.TryGet(out vignette);
        transitionVolume.profile.TryGet(out colorAdjust);
    }

    public void PlayTransition(string sceneName)
    {
        if (menuCanvas != null)
            menuCanvas.gameObject.SetActive(false);
        else print("Menu canvas no esta asignado");

        if (vhsCanvas != null)
            vhsCanvas.gameObject.SetActive(true);
        else print("Menu de transicion no esta asignado");

        StartCoroutine(DoTransition(sceneName));
    }

    private IEnumerator DoTransition(string sceneName)
    {
        float time = 0;

        if (vignette != null) vignette.intensity.value = 0.6f;
        if (colorAdjust != null) colorAdjust.postExposure.value = -1.2f;

        while (time < 1)
        {
            time += Time.deltaTime * transitionSpeed;
            print(time);

            if (grain != null) grain.intensity.value = Mathf.PingPong(Time.time * 2f, grainIntensityMax);

            if (chroma != null) chroma.intensity.value = Mathf.PingPong(Time.time * 0.5f, 0.3f);


            yield return null;
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        time = 1;
        while (time > 0)
        {
            time -= Time.deltaTime * transitionSpeed;

            grain.intensity.value = Mathf.PingPong(Time.time * 2f, grainIntensityMax);
            chroma.intensity.value = time * 0.3f;

            colorAdjust.postExposure.value = time * -1.2f;
            colorAdjust.saturation.value = 0f;
            colorAdjust.colorFilter.value = Color.white;

            yield return null;
        }

        grain.intensity.value = 0;
        chroma.intensity.value = 0;
        vignette.intensity.value = 0;
        colorAdjust.postExposure.value = 0f;
        colorAdjust.saturation.value = 0f;
        colorAdjust.colorFilter.value = Color.white;

    }
}