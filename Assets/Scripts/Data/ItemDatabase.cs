// Assets/Scripts/Data/ItemDatabase.cs
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Runtime icon repository.  
/// * Looks in memory first (NFTs downloaded at runtime).  
/// * Falls back to <c>Resources/Icons/{itemId}.png</c>.  
/// * Uses a 1×1 white sprite as the global fallback so the UI never breaks.
/// </summary>
public static class ItemDatabase
{
    // ------------------------------------------------------------------ //
    // Cached assets
    // ------------------------------------------------------------------ //
    private static readonly Dictionary<string, Sprite> _cache = new();
    private static          Sprite                     _defaultSprite;

    // ------------------------------------------------------------------ //
    // Public API
    // ------------------------------------------------------------------ //

    /// <summary>Returns a sprite for <paramref name="itemId"/>.</summary>
    public static Sprite GetIcon(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
            return GetDefault();

        // 1️⃣ In-memory cache — covers downloaded NFT icons
        if (_cache.TryGetValue(itemId, out var sprite))
            return sprite;

        // 2️⃣ Resources folder — classic built-in icons
        sprite = Resources.Load<Sprite>($"Icons/{itemId}");
        if (sprite != null)
        {
            _cache[itemId] = sprite; // memoise for next time
            return sprite;
        }

        // 3️⃣ Final fallback
        return GetDefault();
    }

    /// <summary>Stores <paramref name="sprite"/> in the runtime cache.</summary>
    public static void CacheIcon(string itemId, Sprite sprite)
    {
        if (string.IsNullOrEmpty(itemId) || sprite == null) return;
        _cache[itemId] = sprite;
    }

    /// <summary>Returns <c>true</c> if an icon is already loaded in memory.</summary>
    public static bool HasIcon(string itemId) => _cache.ContainsKey(itemId);

    /// <summary>Clears every runtime-loaded icon (does not touch Resources).</summary>
    public static void ClearCache() => _cache.Clear();

    // ------------------------------------------------------------------ //
    // Helpers
    // ------------------------------------------------------------------ //
    private static Sprite GetDefault()
    {
        if (_defaultSprite == null)
        {
            var tex = Texture2D.whiteTexture;
            _defaultSprite = Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f),
                100);                             // PPU
        }
        return _defaultSprite;
    }
}
