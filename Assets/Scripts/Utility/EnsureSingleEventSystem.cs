using UnityEngine;
using UnityEngine.EventSystems;

[DefaultExecutionOrder(-1000)]
public class EnsureSingleEventSystem : MonoBehaviour
{
    void Awake()
    {
        var systems = FindObjectsOfType<EventSystem>();
        if (systems.Length > 1)
        {
            // Keep the first, destroy extras
            for (int i = 1; i < systems.Length; i++)
                DestroyImmediate(systems[i].gameObject);
        }
    }
}
