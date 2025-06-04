using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using BiotonicFrontiers.Core;
using BiotonicFrontiers.Data;
using BiotonicFrontiers.Data.DTO;
using BiotonicFrontiers.Events;

namespace BiotonicFrontiers.Core
{
    /// <summary>
    /// Central JSON→typed-message dispatcher.
    /// Converts raw WS text into C# structs and publishes typed events.
    /// </summary>
    public static class MessageRouter
    {
        public static void Dispatch(string json)
        {
            var wrap = JsonConvert.DeserializeObject<ServerMsgWrapper>(json);
            switch (wrap.type)
            {
                case NetworkEvents.GameStart:
                    EventBus.Publish(EventNames.Server_GameStart,
                        JsonConvert.DeserializeObject<GameStart>(json));
                    break;

                case NetworkEvents.TurnResult:
                    EventBus.Publish(EventNames.Server_TurnResult,
                        JsonConvert.DeserializeObject<TurnResult>(json));
                    break;

                case NetworkEvents.GameOver:
                    EventBus.Publish(EventNames.Server_GameOver,
                        JsonConvert.DeserializeObject<GameOver>(json));
                    break;

                case "MatchFound":
                    var mf = JsonConvert.DeserializeObject<MatchFound>(json);
                    EventBus.Publish(EventNames.Server_MatchFound, mf);
                    // Persist game ID so Turn messages use the correct GUID
                    GameManager.Instance.SetCurrentGame(mf.game_id);
                    break;

                case NetworkEvents.FactionChat:
                    EventBus.Publish(EventNames.Server_FactionChat,
                        JsonConvert.DeserializeObject<ChatPayload>(json));
                    break;

                case NetworkEvents.LandOwnershipUpdated:
                    EventBus.Publish(EventNames.Server_LandOwnership,
                        JsonConvert.DeserializeObject<List<LandParcel>>(json));
                    break;

                default:
                    Debug.LogWarning($"[MessageRouter] Unknown message type '{wrap.type}'");
                    break;
            }
        }
    }
}
