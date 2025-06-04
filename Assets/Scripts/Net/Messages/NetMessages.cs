// -----------------------------------------------------------------------------
//  Assets/Scripts/Net/Messages/NetMessages.cs
// -----------------------------------------------------------------------------
//  All strongly-typed JSON message definitions that the Unity client receives
//  from (and sends to) the Biotonic Frontiers server WebSocket, mirroring the
//  Rust `ServerMsg` / `ClientMsg` enums defined in server/src/protocol.rs.  Each
//  class exposes the same field names so Newtonsoft.Json can serialize/deserialize
//  the payloads without custom converters.
//
//  Namespace:  BiotonicFrontiers.Net.Messages
//  Dependencies: Newtonsoft.Json, Newtonsoft.Json.Linq
// -----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BiotonicFrontiers.Net.Messages
{
    /// <summary>
    /// Marker interface implemented by every inbound/outbound message that can
    /// flow across the WebSocket.  Enables the EventBus to handle them generically.
    /// </summary>
    public interface INetMessage
    {
        /// <summary>The discriminant tag exactly as present in the JSON "type" field.</summary>
        string Type { get; }
    }

    // ---------------------------------------------------------------------
    //  ────────────────  S E R V E R   →   C L I E N T  ────────────────
    // ---------------------------------------------------------------------

    /// <summary>
    /// Sent once both players have joined a new duel and the server has created
    /// the authoritative session.
    /// </summary>
    public class GameStartMessage : INetMessage
    {
        [JsonProperty("type")]
        public string Type => "GameStart";

        [JsonProperty("game_id")]
        public Guid GameId { get; set; }

        [JsonProperty("turn")]
        public uint Turn { get; set; }
    }

    /// <summary>Turn resolution results dispatched after the server simulates a turn.</summary>
    public class TurnResultMessage : INetMessage
    {
        [JsonProperty("type")]
        public string Type => "TurnResult";

        [JsonProperty("game_id")]
        public Guid GameId { get; set; }

        [JsonProperty("turn")]
        public uint Turn { get; set; }

        /// <remarks>
        /// Captures the raw combat-result payload.  Designers can map JToken to
        /// domain models if needed.
        /// </remarks>
        [JsonProperty("result")]
        public JToken ResultRaw { get; set; }
    }

    /// <summary>
    /// Declares the winner (or a null draw) and closes a game session.
    /// </summary>
    public class GameOverMessage : INetMessage
    {
        [JsonProperty("type")]
        public string Type => "GameOver";

        [JsonProperty("game_id")]
        public Guid GameId { get; set; }

        [JsonProperty("winner")]
        public Guid? Winner { get; set; }
    }

    /// <summary>
    /// Real-time faction chat broadcast.  Delivered to all online members.
    /// </summary>
    public class FactionChatMessage : INetMessage
    {
        [JsonProperty("type")]
        public string Type => "FactionChat";

        [JsonProperty("faction_id")]
        public Guid FactionId { get; set; }

        [JsonProperty("sender_id")]
        public Guid SenderId { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("ts")]
        public DateTime Timestamp { get; set; }
    }

    // ---------------------------------------------------------------------
    //  ────────────────  C L I E N T   →   S E R V E R  ────────────────
    // ---------------------------------------------------------------------

    /// <summary>Signals that the WS connection is ready to enter a specific game.</summary>
    public class ReadyMessage : INetMessage
    {
        [JsonProperty("type")]
        public string Type => "Ready";

        [JsonProperty("game_id")]
        public Guid GameId { get; set; }

        [JsonProperty("player_id")]
        public Guid PlayerId { get; set; }
    }

    /// <summary>Contains the player’s chosen actions for the current turn.</summary>
    public class TurnMessage : INetMessage
    {
        [JsonProperty("type")]
        public string Type => "Turn";

        [JsonProperty("game_id")]
        public Guid GameId { get; set; }

        [JsonProperty("player_id")]
        public Guid PlayerId { get; set; }

        [JsonProperty("turn")]
        public uint Turn { get; set; }

        [JsonProperty("actions")]
        public List<JToken> Actions { get; set; }

        public TurnMessage() { Actions = new List<JToken>(); }
    }

    /// <summary>Reconnect handshake: resume a duel after transient disconnect.</summary>
    public class ResumeMessage : INetMessage
    {
        [JsonProperty("type")]
        public string Type => "Resume";

        [JsonProperty("game_id")]
        public Guid GameId { get; set; }

        [JsonProperty("player_id")]
        public Guid PlayerId { get; set; }
    }

    /// <summary>
    /// INTERNAL ONLY – emitted by the WS layer when Unity notices the socket
    /// has closed.  Not sent over the wire but can be listened for by systems.
    /// </summary>
    public class DisconnectedMessage : INetMessage
    {
        public string Type => "Disconnected"; // not serialized
    }

    // ---------------------------------------------------------------------
    //  Central parser – decode raw JSON into the correct typed object.
    // ---------------------------------------------------------------------

    public static class NetMessageParser
    {
        private static readonly JsonSerializer _serializer = new JsonSerializer();

        /// <summary>
        /// Parse raw JSON payload into the proper INetMessage implementation.
        /// </summary>
        public static INetMessage Deserialize(string json)
        {
            var jObj = JObject.Parse(json);
            var tag  = jObj.Value<string>("type");
            if (string.IsNullOrEmpty(tag)) return null;

            switch (tag)
            {
                case "GameStart":   return jObj.ToObject<GameStartMessage>(_serializer);
                case "TurnResult":  return jObj.ToObject<TurnResultMessage>(_serializer);
                case "GameOver":    return jObj.ToObject<GameOverMessage>(_serializer);
                case "FactionChat": return jObj.ToObject<FactionChatMessage>(_serializer);
                case "Ready":       return jObj.ToObject<ReadyMessage>(_serializer);
                case "Turn":        return jObj.ToObject<TurnMessage>(_serializer);
                case "Resume":      return jObj.ToObject<ResumeMessage>(_serializer);
                default:             return null;
            }
        }

        /// <summary>
        /// Serialize an INetMessage into JSON for sending over the WebSocket.
        /// </summary>
        public static string Serialize(INetMessage msg)
        {
            return JsonConvert.SerializeObject(msg);
        }
    }
}