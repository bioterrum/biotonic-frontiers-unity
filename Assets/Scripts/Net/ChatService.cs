using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using BiotonicFrontiers.Data;

namespace BiotonicFrontiers.Net
{
    /// <summary>Thin wrapper for faction chat history endpoint.</summary>
    public static class ChatService
    {
        public static async UniTask<List<ChatPayload>> HistoryAsync(Guid factionId, int limit = 50)
        {
            return await HttpClient.GetAsync<List<ChatPayload>>($"/chat/faction/history/{factionId}?limit={limit}");
        }
    }
}