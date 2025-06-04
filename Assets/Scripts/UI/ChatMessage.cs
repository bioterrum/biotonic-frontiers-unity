using UnityEngine;
using UnityEngine.UI;
using BiotonicFrontiers.Data;

namespace BiotonicFrontiers.UI
{
    public class ChatMessage : MonoBehaviour
    {
        [SerializeField] private Text senderText;
        [SerializeField] private Text contentText;
        [SerializeField] private Text timestampText;

        /// <summary>
        /// Initialize the chat message UI.
        /// </summary>
        public void Set(string senderName, string content, long timestamp)
        {
            senderText.text = senderName;
            contentText.text = content;
            timestampText.text = FormatTimestamp(timestamp);
        }

        private string FormatTimestamp(long ts)
        {
            // Assumes ts is Unix milliseconds
            var dt = System.DateTimeOffset.FromUnixTimeMilliseconds(ts)
                         .ToLocalTime()
                         .DateTime;
            return dt.ToString("HH:mm");
        }
    }
}