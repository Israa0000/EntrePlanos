using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CameraMovementMenu : MonoBehaviour
{
    [Header("Output")]
    [SerializeField] private RenderTexture renderTexture; // RenderTexture que pinta la cámara
    [SerializeField] private RawImage backgroundRawImage; // UI RawImage donde se mostrará el fondo

    [Header("Path")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private bool loop = true;
    [SerializeField, Min(0.01f)] private float segmentDuration = 5f; // tiempo en segundos por segmento (lerp)
    [SerializeField, Min(0f)] private float waitAtWaypoint = 1f; // tiempo que espera en cada waypoint
    [SerializeField] private bool lookAtNextWaypoint = true; // si true, rota mirando al siguiente punto

    [Header("Playback")]
    [SerializeField] private bool playOnEnable = false;

    private Camera cam;
    private Coroutine playbackCoroutine;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    void Awake()
    {
        cam = GetComponent<Camera>();
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    void OnEnable()
    {
        if (cam != null && renderTexture != null)
            cam.targetTexture = renderTexture;

        if (backgroundRawImage != null)
            backgroundRawImage.texture = renderTexture;

        if (playOnEnable)
            StartPlayback();
    }

    void OnDisable()
    {
        StopPlayback();

        if (cam != null)
            cam.targetTexture = null;
    }

    public void StartPlayback()
    {
        if (playbackCoroutine != null) return;
        if (waypoints == null || waypoints.Length == 0) return;
        playbackCoroutine = StartCoroutine(PlaybackRoutine());
    }

    public void StopPlayback()
    {
        if (playbackCoroutine != null)
        {
            StopCoroutine(playbackCoroutine);
            playbackCoroutine = null;
        }

        // Restaurar a la posición/rotación inicial
        transform.position = initialPosition;
        transform.rotation = initialRotation;
    }

    private IEnumerator PlaybackRoutine()
    {
        if (waypoints == null || waypoints.Length == 0)
            yield break;

        do
        {
            for (int i = 0; i < waypoints.Length; i++)
            {
                Transform target = waypoints[i];
                if (target == null) continue;

                Vector3 startPos = transform.position;
                Quaternion startRot = transform.rotation;
                Vector3 endPos = target.position;

                // Determinar hacia qué punto mirar: siguiente waypoint o el propio
                Vector3 lookTargetPos = endPos;
                if (lookAtNextWaypoint && waypoints.Length > 1)
                {
                    int nextIndex = (i + 1) % waypoints.Length;
                    if (waypoints[nextIndex] != null)
                        lookTargetPos = waypoints[nextIndex].position;
                }

                float elapsed = 0f;
                float duration = Mathf.Max(0.001f, segmentDuration);

                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsed / duration);
                    float smoothT = Mathf.SmoothStep(0f, 1f, t);

                    // Posición: Lerp
                    transform.position = Vector3.Lerp(startPos, endPos, smoothT);

                    // Rotación: Slerp hacia la orientación deseada
                    if (lookAtNextWaypoint)
                    {
                        Vector3 dir = (lookTargetPos - transform.position);
                        if (dir.sqrMagnitude > 0.0001f)
                        {
                            Quaternion targetRot = Quaternion.LookRotation(dir.normalized);
                            transform.rotation = Quaternion.Slerp(startRot, targetRot, smoothT);
                        }
                    }
                    else
                    {
                        // Si no mirar al siguiente, interpolar hacia la rotación del waypoint (si tiene)
                        Quaternion targetRot = target.rotation;
                        transform.rotation = Quaternion.Slerp(startRot, targetRot, smoothT);
                    }

                    yield return null;
                }

                // Asegurar exactitud final
                transform.position = endPos;
                if (lookAtNextWaypoint)
                {
                    Vector3 finalDir = (lookTargetPos - transform.position);
                    if (finalDir.sqrMagnitude > 0.0001f)
                        transform.rotation = Quaternion.LookRotation(finalDir.normalized);
                }
                else
                {
                    transform.rotation = target.rotation;
                }

                // Espera en el waypoint
                float waitTimer = 0f;
                while (waitTimer < waitAtWaypoint)
                {
                    waitTimer += Time.deltaTime;
                    yield return null;
                }
            }

            // Si no hacemos loop, salir después de una pasada completa
            if (!loop) break;

        } while (true);

        playbackCoroutine = null;
    }
}