using System;

namespace BiotonicFrontiers.Core
{
    /// <summary>Canonical event name strings shared across the client.</summary>
    public static class NetworkEvents
    {
        // Gameplay
        public const string GameStart = "GameStart";
        public const string TurnResult = "TurnResult";
        public const string GameOver = "GameOver";

        // Economy
        public const string ShopUpdated = "ShopUpdated";
        public const string InventoryUpdated = "InventoryUpdated";
        public const string CoinBalanceUpdated = "CoinBalanceUpdated";

        // Social
        public const string FactionChat = "FactionChat";
        public const string FactionListUpdated = "FactionListUpdated";
        // Land
        public const string LandOwnershipUpdated = "LandOwnershipUpdated";

        // Chain
        public const string ChainEvent = "ChainEvent";

    }
}