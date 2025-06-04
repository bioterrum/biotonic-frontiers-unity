using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using BiotonicFrontiers.Data.DTO;    // for LeaderboardEntry
using BiotonicFrontiers.Net;         // for HttpClient

namespace BiotonicFrontiers.Net
{
    /// <summary>
    /// Convenience wrapper around /api/leaderboard endpoint.
    /// </summary>
    public static class LeaderboardService
    {
        private const string Route = "/leaderboard";

        /// <summary>
        /// Fetches the top <paramref name="limit"/> players.
        /// </summary>
        public static async UniTask<List<LeaderboardEntry>> GetTopAsync(int limit)
        {
            return await HttpClient.GetAsync<List<LeaderboardEntry>>(
                $"{Route}?limit={limit}"
            );
        }
    }
}
