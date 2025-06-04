// Assets/Scripts/Net/ChainEventsListener.cs
using Newtonsoft.Json.Linq;
using UnityEngine;
using BiotonicFrontiers.Core;               // NetworkEvents
using EventHub = global::EventBus;         // ← explicit alias to the global bus

namespace BiotonicFrontiers.Net
{
    /// <summary>Listens for on-chain events pushed by the server WebSocket.</summary>
    public sealed class ChainEventsListener : MonoBehaviour
    {
        void OnEnable()  => EventHub.Subscribe(NetworkEvents.ChainEvent, OnChainEvent);
        void OnDisable() => EventHub.Unsubscribe(NetworkEvents.ChainEvent, OnChainEvent);

        void OnChainEvent(object payload)
        {
            var evt = (JObject)payload;
            Debug.Log($"🔗 Chain event: {evt}");
            // TODO: surface to UI (toast/feed)
        }
    }
}
