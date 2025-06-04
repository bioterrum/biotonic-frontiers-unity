// Assets/Scripts/UI/InventoryItemWidget.cs
// -----------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;

namespace BiotonicFrontiers.UI
{
    /// <summary>
    /// Thin wrapper over <see cref="InventorySlot"/> that guarantees the
    /// embedded *Use* and *Sell* buttons are always hooked up to the correct
    /// click-handlers, even if the prefab lost its serialized links.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Biotonic/UI/Inventory Item Widget")]
    public sealed class InventoryItemWidget : InventorySlot
    {
        // Optional manual refs; leave blank to auto-find by child name.
        [Header("Optional – will auto-find if null")]
        [SerializeField] private Button useButton;
        [SerializeField] private Button sellButton;

#if UNITY_EDITOR
        // Runs whenever the prefab is modified in the Editor.
        private void OnValidate() => WireButtons();
#endif
        private void Awake() => WireButtons();

        // ------------------------------- helpers -------------------------- //
        /// <summary>Finds the two child buttons and binds inherited handlers.</summary>
        private void WireButtons()
        {
            // 1️⃣  Locate buttons if not assigned.
            if (useButton  == null) useButton  = transform.Find("UseButton") ?.GetComponent<Button>();
            if (sellButton == null) sellButton = transform.Find("SellButton")?.GetComponent<Button>();

            // 2️⃣  Bind click events (idempotent).
            if (useButton != null)
            {
                useButton.onClick.RemoveListener(OnUseClicked);
                useButton.onClick.AddListener   (OnUseClicked);
            }

            if (sellButton != null)
            {
                sellButton.onClick.RemoveListener(OnSellClicked);
                sellButton.onClick.AddListener   (OnSellClicked);
            }
        }
    }
}
