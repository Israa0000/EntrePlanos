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
    private LensDistortion distortion;
    private Vignette vignette;
    private ColorAdjustments colorAdjust;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        transitionVolume.profile.TryGet(out grain);
        transitionVolume.profile.TryGet(out chroma);
        transitionVolume.profile.TryGet(out distortion);
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
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * transitionSpeed;

            if (grain != null) grain.intensity.value = Mathf.PingPong(Time.time * 2f, grainIntensityMax);

            if (chroma != null) chroma.intensity.value = Mathf.PingPong(Time.time * 0.5f, 0.3f);

            if (vignette != null) vignette.intensity.value = 0.6f;

            if (colorAdjust != null) colorAdjust.postExposure.value = -1.2f;

            yield return null;
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        t = 1;
        while (t > 0)
        {
            t -= Time.deltaTime * transitionSpeed;
            fadeImage.color = new Color(0, 0, 0, t);

            grain.intensity.value = Mathf.PingPong(Time.time * 2f, grainIntensityMax);
            chroma.intensity.value = t * 0.3f;

            colorAdjust.postExposure.value = t * -1.2f;
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