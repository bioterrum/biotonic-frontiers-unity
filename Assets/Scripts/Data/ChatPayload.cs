using System;

namespace BiotonicFrontiers.Data
{
    [Serializable]
    public struct ChatPayload
    {
        public string senderName;
        public string content;
        public long timestamp;
    }
}