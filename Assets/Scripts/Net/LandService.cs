// Assets/Scripts/Net/LandService.cs
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using BiotonicFrontiers.Data.DTO;
using BiotonicFrontiers.Core;

namespace BiotonicFrontiers.Net
{
    /// <summary>Convenience wrapper around /api/land endpoints.</summary>
    public static class LandService
    {
        private const string Route = "/land";

        /// <summary>List every parcel owned by the supplied player.</summary>
        public static async UniTask<List<LandParcel>> GetOwnedParcelsAsync(Guid playerId)
            => await HttpClient.GetAsync<List<LandParcel>>($"{Route}/owned/{playerId}");

        /// <summary>Fetch a single land parcel at (x,y).</summary>
        public static async UniTask<LandParcel> GetParcelAsync(int x, int y)
            => await HttpClient.GetAsync<LandParcel>($"{Route}/at/{x}/{y}");

        /// <summary>Claims a tile for the supplied faction.</summary>
        public static async UniTask<bool> ClaimAsync(Guid factionId, int x, int y, string biome)
        {
            var body = new ClaimReq { faction_id = factionId.ToString(), x = x, y = y, biome_type = biome };
            return await HttpClient.PostAsync($"{Route}/claim", body);
        }
    }
}
