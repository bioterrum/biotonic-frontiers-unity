using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BiotonicFrontiers.Core;
using BiotonicFrontiers.Net;
using BiotonicFrontiers.Data.DTO;
using Cysharp.Threading.Tasks;
using System;

namespace BiotonicFrontiers.UI
{
    public class TradeUI : MonoBehaviour
    {
        [Header("Trade List")]
        [SerializeField] private Transform listRoot;
        [SerializeField] private TradeRow  rowPrefab;

        [Header("Create Trade")]
        [SerializeField] private TMP_InputField offerItemInput;
        [SerializeField] private TMP_InputField offerQtyInput;
        [SerializeField] private TMP_InputField requestItemInput;
        [SerializeField] private TMP_InputField requestQtyInput;
        [SerializeField] private Button         createTradeButton;
        [SerializeField] private TMP_Text       statusText;

        void OnEnable()
        {
            createTradeButton.onClick.AddListener(OnCreateTradeClicked);
            RefreshTrades().Forget();
        }

        void OnDisable()
        {
            createTradeButton.onClick.RemoveListener(OnCreateTradeClicked);
        }

        private async UniTaskVoid RefreshTrades()
        {
            // Clear old entries
            foreach (Transform child in listRoot)
                Destroy(child.gameObject);

            try
            {
                var trades = await TradeService.GetOpenTradesAsync();
                foreach (var t in trades)
                {
                    var row = Instantiate(rowPrefab, listRoot);
                    row.Bind(t, OnAcceptTrade);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load trades: {e}");
                statusText.text = "Error loading trades.";
            }
        }

        private async void OnAcceptTrade(TradeInfo trade)
        {
            statusText.text = "Accepting…";
            bool ok = await TradeService.AcceptTradeAsync(
                GameManager.Instance.PlayerId,
                trade.trade_id
            );

            if (ok)
            {
                statusText.text = "Trade accepted!";
                // Refresh inventory and trades
                GameManager.Instance.Net.RequestInventory();
                RefreshTrades().Forget();
            }
            else
            {
                statusText.text = "Accept failed.";
            }
        }

        private async void OnCreateTradeClicked()
        {
            var pid = GameManager.Instance.PlayerId;
            var offeredItem   = offerItemInput.text.Trim();
            var requestedItem = requestItemInput.text.Trim();
            if (!int.TryParse(offerQtyInput.text, out int oQty) ||
                !int.TryParse(requestQtyInput.text, out int rQty) ||
                string.IsNullOrEmpty(offeredItem) ||
                string.IsNullOrEmpty(requestedItem))
            {
                statusText.text = "Enter valid trade details.";
                return;
            }

            statusText.text = "Creating…";
            bool ok = await TradeService.CreateTradeAsync(
                pid,
                offeredItem, oQty,
                requestedItem, rQty
            );

            if (ok)
            {
                statusText.text = "Trade created!";
                // Clear inputs
                offerItemInput.text   = "";
                offerQtyInput.text    = "";
                requestItemInput.text = "";
                requestQtyInput.text  = "";
                // Update inventory and refresh list
                GameManager.Instance.Net.RequestInventory();
                RefreshTrades().Forget();
            }
            else
            {
                statusText.text = "Create failed.";
            }
        }
    }
}
