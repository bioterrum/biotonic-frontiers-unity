using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Creates the absolute minimum singletons every scene needs *before* any
/// of your own MonoBehaviours run: a main camera, EventSystem, audio listener,
/// and the global GameManager prefab.  Drop this on a single empty GameObject
/// in the new *Bootstrap* scene (first in Build Settings).
/// </summary>
public sealed class BootstrapStartup : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void PreloadSingletons()
    {
        // ------------------------------------------------------------------ Main Camera
        if (Camera.main == null)
        {
            var camGO = new GameObject("Main Camera");
            camGO.tag = "MainCamera";
            var cam   = camGO.AddComponent<Camera>();
            cam.clearFlags      = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.black;
            cam.nearClipPlane   = 0.3f;
            cam.farClipPlane    = 1000f;
            DontDestroyOnLoad(camGO);
        }

        // ------------------------------------------------------------------ EventSystem
        if (FindObjectOfType<EventSystem>() == null)
        {
            var esGO = new GameObject("EventSystem",
                                      typeof(EventSystem),
                                      typeof(StandaloneInputModule));
            DontDestroyOnLoad(esGO);
        }

        // ------------------------------------------------------------------ AudioListener
        if (FindObjectOfType<AudioListener>() == null)
        {
            var alGO = new GameObject("AudioListener", typeof(AudioListener));
            DontDestroyOnLoad(alGO);
        }

        // ------------------------------------------------------------------ GameManager prefab
        if (FindObjectOfType<BiotonicFrontiers.Core.GameManager>() == null)
        {
            var gm = new GameObject("GameManager");
            gm.AddComponent<BiotonicFrontiers.Core.GameManager>();
            DontDestroyOnLoad(gm);
        }
    }
}
