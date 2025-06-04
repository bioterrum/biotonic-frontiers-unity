#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Scans every <see cref="Scene"/> currently loaded in the editor and reports — or optionally
/// fixes — any <b>missing MonoBehaviour script</b> references.  Place this file anywhere under an
/// <c>Editor/</c> folder so it is excluded from player builds.
/// </summary>
public static class SceneValidator
{
    /// <summary>Validate all open scenes and print a summary to the Console window.</summary>
    [MenuItem("Tools/Scenes/Validate Missing Scripts", priority = 500)]
    public static void ValidateOpenScenes()
    {
        int missingTotal = 0;

        // Iterate over every scene the user currently has open in the Hierarchy.
        for (int i = 0; i < EditorSceneManager.sceneCount; ++i)
        {
            var scene = EditorSceneManager.GetSceneAt(i);
            if (!scene.isLoaded) continue;

            foreach (var root in scene.GetRootGameObjects())
                missingTotal += CountMissingScriptsRecursive(root);
        }

        if (missingTotal == 0)
            Debug.Log("<color=green>✅ No missing scripts found in open scenes.</color>");
        else
            Debug.LogWarning($"<color=yellow>⚠  Found {missingTotal} missing script reference(s) in open scenes.</color>");
    }

    /// <summary>
    /// Recursively counts <i>all</i> missing‑script references on <paramref name="go"/> and its children.
    /// </summary>
    private static int CountMissingScriptsRecursive(GameObject go)
    {
        // NOTE: This is the correct usage – the API *requires* the GameObject parameter.
        int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);

        // Drill down through the hierarchy.
        foreach (Transform child in go.transform)
            count += CountMissingScriptsRecursive(child.gameObject);

        return count;
    }

    /// <summary>
    /// Removes every missing‑script component from the currently loaded scenes.  Useful when
    /// cleaning up imported packages or broken prefab links.
    /// </summary>
    [MenuItem("Tools/Scenes/Fix Missing Scripts", priority = 501)]
    public static void FixMissingScripts()
    {
        int removedTotal = 0;

        for (int i = 0; i < EditorSceneManager.sceneCount; ++i)
        {
            var scene = EditorSceneManager.GetSceneAt(i);
            if (!scene.isLoaded) continue;

            foreach (var root in scene.GetRootGameObjects())
                removedTotal += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(root);
        }

        if (removedTotal == 0)
            Debug.Log("<color=green>✅ No missing scripts were found to remove.</color>");
        else
            Debug.LogWarning($"<color=orange>🩹 Removed {removedTotal} missing script component(s) in open scenes.</color>");
    }
}
#endif // UNITY_EDITOR
