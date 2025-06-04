using UnityEngine;

/// <summary>Static item definition loaded via Addressables/Resources.</summary>
[CreateAssetMenu(menuName = "Biotonic/Item Definition", fileName = "NewItem")]
public class ItemDefinition : ScriptableObject
{
    public int    ItemId;
    public string ItemName;
    [TextArea] public string Description;
    public int    BasePrice;
    // sprite reference goes here (Addressable key or Sprite directly)
}