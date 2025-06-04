namespace BiotonicFrontiers.Events
{
    /// <summary>Strongly-typed event keys for EventBus → replaces string literals.</summary>
    public static class EventNames
    {
        // network domain
        public const string Server_GameStart        = "srv.game_start";
        public const string Server_TurnResult       = "srv.turn_result";
        public const string Server_GameOver         = "srv.game_over";
        public const string Server_MatchFound       = "srv.match_found";
        public const string Server_FactionChat      = "srv.faction_chat";
        public const string Server_LandOwnership    = "srv.land_ownership";

        // client domain
        public const string Client_InventoryUpdated = "cli.inventory_updated";
        public const string Client_ShopUpdated      = "cli.shop_updated";
        public const string Client_FactionList      = "cli.faction_list";
    }
}
