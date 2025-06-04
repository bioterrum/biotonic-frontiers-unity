using System;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using BiotonicFrontiers.Core;
using BiotonicFrontiers.Data;
using BiotonicFrontiers.Net;

namespace BiotonicFrontiers.UI
{
    /// <summary>Handles faction chat: history + live messages + sending.</summary>
    public class FactionChatUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Text factionIdText;
        [SerializeField] private Transform chatContent;
        [SerializeField] private ChatMessage chatMessagePrefab;
        [SerializeField] private InputField messageInput;
        [SerializeField] private Button sendButton;

        private void OnEnable()
        {
            // Display current faction
            var fid = GameManager.Instance.PlayerFactionName;
            factionIdText.text = $"Chat – Faction: {fid}";

            // Load history then subscribe to live updates
            LoadChatHistory().Forget();
            EventBus.Subscribe(NetworkEvents.FactionChat, OnFactionChatReceived);
            sendButton.onClick.AddListener(OnSendClicked);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe(NetworkEvents.FactionChat, OnFactionChatReceived);
            sendButton.onClick.RemoveListener(OnSendClicked);
        }

        private async UniTaskVoid LoadChatHistory()
        {
            try
            {
                Guid factionGuid = Guid.Parse(GameManager.Instance.PlayerFactionName);
                var history = await ChatService.HistoryAsync(factionGuid);

                // Clear any old messages
                foreach (Transform child in chatContent)
                    Destroy(child.gameObject);

                // Populate with history
                foreach (var msg in history)
                {
                    var entry = Instantiate(chatMessagePrefab, chatContent);
                    entry.Set(msg.senderName, msg.content, msg.timestamp);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load faction chat history: {e}");
            }
        }

        private void OnFactionChatReceived(object payload)
        {
            // payload is a boxed ChatPayload (a struct), so use pattern matching
            if (!(payload is ChatPayload msg))
                return;

            var entry = Instantiate(chatMessagePrefab, chatContent);
            entry.Set(msg.senderName, msg.content, msg.timestamp);
        }

        private void OnSendClicked()
        {
            var text = messageInput.text.Trim();
            if (string.IsNullOrEmpty(text)) return;

            // Send via WebSocket
            NetworkManager.Instance.SendFactionChat(text);
            messageInput.text = string.Empty;
        }
    }
}
