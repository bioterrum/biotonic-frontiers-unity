using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Creates a default Unity *EventSystem* so UI buttons work even in scenes that
/// were committed without one.
/// </summary>
public sealed class EventSystemBootstrap : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>() != null) return;

        var go = new GameObject("EventSystem");
        go.AddComponent<EventSystem>();
        go.AddComponent<StandaloneInputModule>();
        DontDestroyOnLoad(go);
    }
}