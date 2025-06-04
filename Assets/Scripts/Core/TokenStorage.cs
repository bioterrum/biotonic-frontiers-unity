using UnityEngine;

/// <summary>Persist JWT & refresh tokens.</summary>
public static class TokenStorage
{
    private const string KEY_ACC = "bf.token.access";
    private const string KEY_REF = "bf.token.refresh";

    public static void Save(string acc, string refr)
    {
        PlayerPrefs.SetString(KEY_ACC, acc);
        PlayerPrefs.SetString(KEY_REF, refr);
        PlayerPrefs.Save();
    }

    public static bool TryLoad(out string acc, out string refr)
    {
        acc = PlayerPrefs.GetString(KEY_ACC, "");
        refr = PlayerPrefs.GetString(KEY_REF, "");
        return !string.IsNullOrEmpty(acc) && !string.IsNullOrEmpty(refr);
    }

    public static void Clear()
    {
        PlayerPrefs.DeleteKey(KEY_ACC);
        PlayerPrefs.DeleteKey(KEY_REF);
    }
}

