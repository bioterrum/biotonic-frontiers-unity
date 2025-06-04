using System;
using System.Collections.Generic;

namespace BiotonicFrontiers.Data.DTO
{
    [Serializable] public struct ServerMsgWrapper { public string type; }

    // === Gameplay ============================================
    [Serializable] public struct GameStart  { public string type; public string game_id; public uint turn; }
    [Serializable] public struct TurnAction { public string actionType; public Dictionary<string, object> parameters; }
    [Serializable] public struct TurnRequest { public string game_id; public string player_id; public uint turn; public List<TurnAction> actions; }
    [Serializable] public struct TurnResult { public string type; public uint turn; public List<TurnAction> spawned; public List<TurnAction> destroyed; }
    [Serializable] public struct GameOver   { public string type; public string winner_id; }
    [Serializable] public struct MatchFound { public string game_id; public string opponent_id; }
    [Serializable] public struct MatchmakingRequest { public string player_id; public int elo_rating; }

    // === Economy =============================================
    [Serializable] public struct ClaimReq   { public string faction_id; public int x; public int y; public string biome_type; }
    [Serializable] public struct BuyReq     { public string player_id; public int item_id; public int quantity; }
    [Serializable] public struct UseItemReq { public string player_id; public string item_id; }

    // === Social / Factions ===================================
    [Serializable] public struct FactionInfo      { public string faction_id; public string name; public int member_count; }
    [Serializable] public struct FactionCreateReq { public string name; }
    [Serializable] public struct FactionJoinReq   { public string faction_id; }
    [Serializable] public struct FactionLeaveReq  { public string faction_id; }

    // === Land ================================================
    [Serializable] public struct LandParcel { public int x; public int y; public string owner_faction_id; public string biome_type; }
}