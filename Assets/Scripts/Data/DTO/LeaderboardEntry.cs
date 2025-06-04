using System;

namespace BiotonicFrontiers.Data.DTO
{
    [Serializable]
    public struct LeaderboardEntry
    {
        public Guid playerId;
        public string nickname;
        public int eloRating;
    }
}
