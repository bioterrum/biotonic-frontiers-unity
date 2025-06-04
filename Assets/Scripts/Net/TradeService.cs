using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using BiotonicFrontiers.Data.DTO;

namespace BiotonicFrontiers.Net
{
    /// <summary>Convenience wrapper around /api/trades endpoints.</summary>
    public static class TradeService
    {
        private const string Route = "/trades";

        /// <summary>List all open trades.</summary>
        public static async UniTask<List<TradeInfo>> GetOpenTradesAsync()
        {
            return await HttpClient.GetAsync<List<TradeInfo>>($"{Route}/open");
        }

        /// <summary>Create a new trade offer.</summary>
        public static async UniTask<bool> CreateTradeAsync(
            string playerId,
            string offeredItemId,
            int    offeredQty,
            string requestedItemId,
            int    requestedQty)
        {
            var body = new TradeCreateReq {
                player_id          = playerId,
                offered_item_id    = offeredItemId,
                offered_quantity   = offeredQty,
                requested_item_id  = requestedItemId,
                requested_quantity = requestedQty
            };
            return await HttpClient.PostAsync($"{Route}/create", body);
        }

        /// <summary>Accept an existing trade.</summary>
        public static async UniTask<bool> AcceptTradeAsync(string playerId, string tradeId)
        {
            var body = new AcceptTradeReq {
                player_id = playerId,
                trade_id  = tradeId
            };
            return await HttpClient.PostAsync($"{Route}/accept", body);
        }
    }
}
