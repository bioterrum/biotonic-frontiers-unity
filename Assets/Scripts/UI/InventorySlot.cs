using UnityEngine;
using UnityEngine.UI;
using BiotonicFrontiers.Data;
using BiotonicFrontiers.Core;

namespace BiotonicFrontiers.UI
{
    public class InventorySlot : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Text quantityText;
        private ItemStack _stack;

        public void Bind(ItemStack stack)
        {
            _stack = stack;
            icon.sprite = ItemDatabase.GetIcon(stack.itemId);
            quantityText.text = stack.quantity.ToString();
        }

        // Called by Button component
        public void OnUseClicked()
        {
            NetworkManager.Instance.UseItem(_stack.itemId);
        }

        public void OnSellClicked()
        {
            ShopUI.Instance.OpenSellDialog(_stack);
        }
    }
}