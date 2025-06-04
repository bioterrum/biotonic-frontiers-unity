using UnityEngine;

/// <summary>
/// Guarantees that exactly one <see cref="AudioListener"/> is active after
/// every scene load.  Any additional listeners are disabled to silence
/// Unity’s “There are 2 audio listeners…” warning.
/// </summary>
public static class AudioListenerBootstrap
{
    // Runs automatically each time a scene finishes loading.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureSingleListener()
    {
        var listeners = Object.FindObjectsOfType<AudioListener>();
        if (listeners.Length <= 1) return;          // already fine

        bool keepFirst = true;
        foreach (var al in listeners)
        {
            if (keepFirst) { keepFirst = false; continue; }
            al.enabled = false;                     // disable extras
        }

        Debug.LogWarning(
            $"[AudioListenerBootstrap] Disabled {listeners.Length - 1} extra AudioListener(s).");
    }
}
