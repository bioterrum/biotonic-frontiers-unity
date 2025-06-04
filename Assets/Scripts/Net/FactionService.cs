using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using BiotonicFrontiers.Data.DTO;

namespace BiotonicFrontiers.Net
{
    /// <summary>Typed REST helpers for /api/factions.* routes.</summary>
    public static class FactionService
    {
        private const string Route = "/factions";

        public static async UniTask<List<FactionInfo>> ListAsync()
            => await HttpClient.GetAsync<List<FactionInfo>>($"{Route}/list");

        public static async UniTask<bool> CreateAsync(string name, Guid founder)
        {
            var body = new { name, founder_id = founder };
            return await HttpClient.PostAsync($"{Route}/create", body);
        }

        public static async UniTask<bool> JoinAsync(Guid factionId, Guid playerId)
        {
            var body = new { faction_id = factionId, player_id = playerId };
            return await HttpClient.PostAsync($"{Route}/join", body);
        }

        public static async UniTask<bool> LeaveAsync(Guid factionId, Guid playerId)
        {
            var body = new { faction_id = factionId, player_id = playerId };
            return await HttpClient.PostAsync($"{Route}/leave", body);
        }

        /// <summary>Returns extended info (incl. members) for the player’s current faction.</summary>
        public static async UniTask<FactionInfo> InfoForPlayerAsync(Guid playerId)
            => await HttpClient.GetAsync<FactionInfo>($"{Route}/of/{playerId}");
    }
}