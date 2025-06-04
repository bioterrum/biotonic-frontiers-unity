using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using BiotonicFrontiers.Core;
using BiotonicFrontiers.Data;
using BiotonicFrontiers.Data.DTO;

namespace BiotonicFrontiers.UI
{
    public class FactionUI : MonoBehaviour
    {
        [SerializeField] private Text factionName;
        [SerializeField] private Transform chatContent;
        [SerializeField] private ChatMessage chatMessagePrefab;
        [SerializeField] private InputField inputField;

        private void OnEnable()
        {
            // Listen for raw server messages and filter for faction chat
            EventBus.Subscribe(NetworkEvent.RawServerMsg, OnRawServerMsg);
            factionName.text = GameManager.Instance.PlayerFactionName;
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe(NetworkEvent.RawServerMsg, OnRawServerMsg);
        }

        private void OnRawServerMsg(object rawJson)
        {
            var wrapper = JsonConvert.DeserializeObject<ServerMsgWrapper>(rawJson.ToString());
            if (wrapper.type != NetworkEvents.FactionChat)
                return;

            var msg = JsonConvert.DeserializeObject<ChatPayload>(rawJson.ToString());
            var go = Instantiate(chatMessagePrefab, chatContent);
            go.Set(msg.senderName, msg.content, msg.timestamp);
        }

        public void OnSendClicked()
        {
            if (string.IsNullOrWhiteSpace(inputField.text)) return;
            NetworkManager.Instance.SendFactionChat(inputField.text);
            inputField.text = string.Empty;
        }
    }
}
