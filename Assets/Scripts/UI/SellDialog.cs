using UnityEngine;
using UnityEngine.UI;
using BiotonicFrontiers.Core;

namespace BiotonicFrontiers.UI
{
    /// <summary>Simple confirmation dialog for selling an item stack.</summary>
    public class SellDialog : MonoBehaviour
    {
        [SerializeField] private Text itemNameText;
        [SerializeField] private InputField quantityInput;
        [SerializeField] private Text totalPriceText;
        [SerializeField] private Button confirmButton;

        private string _itemId;
        private int    _unitPrice;

        public void Open(string itemId, string displayName, int unitPrice)
        {
            _itemId    = itemId;
            _unitPrice = unitPrice;
            itemNameText.text = displayName;
            quantityInput.text = "1";
            UpdateTotalPrice();
            gameObject.SetActive(true);
        }

        public void OnQtyChanged(string _) => UpdateTotalPrice();

        private void UpdateTotalPrice()
        {
            int.TryParse(quantityInput.text, out int qty);
            totalPriceText.text = (qty * _unitPrice).ToString();
        }

        public void OnConfirmClicked()
        {
            int.TryParse(quantityInput.text, out int qty);
            NetworkManager.Instance.SellItem(_itemId, qty);
            gameObject.SetActive(false);
        }

        public void OnCancel() => gameObject.SetActive(false);
    }
}