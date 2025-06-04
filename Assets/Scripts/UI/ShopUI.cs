using System.Collections.Generic;
using UnityEngine;
using BiotonicFrontiers.Data;
using BiotonicFrontiers.Core;

namespace BiotonicFrontiers.UI
{
    public class ShopUI : MonoBehaviour
    {
        public static ShopUI Instance { get; private set; }

        [SerializeField] private Transform gridRoot;
        [SerializeField] private ShopItemTile tilePrefab;

        private void Awake() => Instance = this;

        private void OnEnable()
        {
            EventBus.Subscribe(NetworkEvents.ShopUpdated, OnShopUpdated);
            NetworkManager.Instance.RequestShopStock();
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe(NetworkEvents.ShopUpdated, OnShopUpdated);
        }

        private void OnShopUpdated(object payload)
        {
            var items = payload as List<ShopItem>;
            Refresh(items);
        }

        private void Refresh(List<ShopItem> items)
        {
            foreach (Transform child in gridRoot) Destroy(child.gameObject);
            foreach (var item in items)
            {
                var tile = Instantiate(tilePrefab, gridRoot);
                tile.Bind(item);
            }
        }

        // Inventory → Sell flow
        public void OpenSellDialog(ItemStack stack)
        {
            // TODO: Implement modal dialog asking quantity and confirming price
        }
    }
}