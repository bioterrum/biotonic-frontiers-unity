using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using BiotonicFrontiers.Net.Models;

namespace BiotonicFrontiers.Net
{
    /// <summary>REST helper around /api/inventory routes.</summary>
    public static class InventoryService
    {
        private const string Route = "/inventory";

        public static async UniTask<List<ItemStack>> GetAsync(Guid playerId)
            => await HttpClient.GetAsync<List<ItemStack>>($"{Route}/{playerId}");

        public static async UniTask<bool> UseAsync(Guid playerId, int itemId, int qty = 1)
        {
            var body = new { player_id = playerId, item_id = itemId, quantity = qty };
            return await HttpClient.PostAsync($"{Route}/use", body);
        }
    }
}