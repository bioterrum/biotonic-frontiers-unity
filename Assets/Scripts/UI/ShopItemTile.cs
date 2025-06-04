using UnityEngine;
using UnityEngine.UI;
using BiotonicFrontiers.Data;
using BiotonicFrontiers.Core;

namespace BiotonicFrontiers.UI
{
    public class ShopItemTile : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Text nameText;
        [SerializeField] private Text priceText;

        private ShopItem _item;

        public void Bind(ShopItem item)
        {
            _item = item;
            icon.sprite = ItemDatabase.GetIcon(item.itemId);
            nameText.text = item.displayName;
            priceText.text = item.price.ToString();
        }

        public void OnBuyClicked()
        {
            NetworkManager.Instance.BuyItem(_item.itemId, 1);
        }
    }
}