using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Camera))]
public class FixedAspectRatio : MonoBehaviour 
{
    public float targetAspect = 4f / 3f;
    private Camera cam;
    private int lastWidth;
    private int lastHeight;

    void Start()
    {
        cam = GetComponent<Camera>();
        // Forzamos la resolución al inicio
        Screen.SetResolution(800, 600, FullScreenMode.FullScreenWindow);
        UpdateCameraRect();
    }

    void Update()
    {
        // Si la resolución cambia durante el juego, recalculamos
        if (Screen.width != lastWidth || Screen.height != lastHeight)
        {
            UpdateCameraRect();
        }
    }

    void UpdateCameraRect()
    {
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        Rect rect = cam.rect;

        if (scaleHeight < 1.0f)
        {
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
        }
        else
        {
            float scaleWidth = 1.0f / scaleHeight;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;
        }

        cam.rect = rect;
        
        lastWidth = Screen.width;
        lastHeight = Screen.height;
    }
}

