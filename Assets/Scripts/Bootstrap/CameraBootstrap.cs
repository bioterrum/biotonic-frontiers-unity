using UnityEngine;

/// <summary>
/// Ensures there is always an active *Main Camera* in scenes that forgot to ship one.
/// The object is created at runtime **before the first scene** loads, so it also
/// covers Login → MainMenu transitions.
/// </summary>
public sealed class CameraBootstrap : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateFallbackCamera()
    {
        // Already have one?  Nothing to do.
        if (Camera.main != null) return;

        var go = new GameObject("Main Camera");
        go.tag = "MainCamera";          // Unity’s magic tag looked‑up by Camera.main

        var cam = go.AddComponent<Camera>();
        cam.clearFlags      = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;
        cam.nearClipPlane   = 0.3f;
        cam.farClipPlane    = 1_000f;

        go.AddComponent<AudioListener>(); // so 3D audio keeps working in empty scenes

        // Make sure it survives additive‑scene loading – don’t destroy twice.
        DontDestroyOnLoad(go);
    }
}