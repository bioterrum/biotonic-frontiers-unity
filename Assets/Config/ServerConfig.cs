using UnityEngine;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Biotonic/Server Config", fileName = "ServerConfig")]
#endif
public class ServerConfig : ScriptableObject
{
    // ────────────────────────────────────────────────────────────────
    // Backend HTTP & WebSocket endpoints
    // (NO trailing slash on httpBase / wsUrlBase)
    [Header("HTTP / WS End-points (no trailing slash)")]
    public string httpBase  = "http://127.0.0.1:8282/api";
    public string wsUrlBase = "ws://127.0.0.1:8282/ws/?player_id=";

    // ────────────────────────────────────────────────────────────────
    // Aptos blockchain endpoints (devnet defaults; edit per-build)
    // (NO trailing slash)
    [Header("Aptos Devnet")]
    public string aptosNodeUrl   = "https://fullnode.devnet.aptoslabs.com/v1";
    public string aptosFaucetUrl = "https://faucet.devnet.aptoslabs.com";
}
