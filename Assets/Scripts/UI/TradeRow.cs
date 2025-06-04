using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using BiotonicFrontiers.Data.DTO;

namespace BiotonicFrontiers.UI
{
    /// <summary>UI row shown in the Trade scene list.</summary>
    public class TradeRow : MonoBehaviour
    {
        [SerializeField] private TMP_Text creatorText;
        [SerializeField] private TMP_Text offerText;
        [SerializeField] private TMP_Text requestText;
        [SerializeField] private Button   acceptBtn;

        private TradeInfo _info;
        private Action<TradeInfo> _onAccept;

        /// <summary>
        /// Populate the row with <paramref name="info"/> and wire the Accept button.
        /// </summary>
        public void Bind(TradeInfo info, Action<TradeInfo> onAccept)
        {
            _info = info;

            creatorText.text = info.creator_id;
            offerText.text   = $"{info.offered_quantity} × {info.offered_item_id}";
            requestText.text = $"{info.requested_quantity} × {info.requested_item_id}";

            _onAccept = onAccept;
            acceptBtn.onClick.RemoveAllListeners();
            acceptBtn.onClick.AddListener(() => _onAccept?.Invoke(_info));
        }

        public string TradeId => _info.trade_id;
    }
}