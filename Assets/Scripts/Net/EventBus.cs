// Assets/Scripts/Net/EventBus.cs
using System;
using BiotonicFrontiers.Net.Messages;

namespace BiotonicFrontiers.Net
{
    /// <summary>
    /// Lightweight static aggregator for propagating server→client messages.
    /// Systems subscribe to the typed events; the WebSocket layer just calls
    /// Publish for every decoded INetMessage it receives.
    /// </summary>
    public static class EventBus
    {
        public static event Action<GameStartMessage>   GameStart;
        public static event Action<TurnResultMessage>  TurnResult;
        public static event Action<GameOverMessage>    GameOver;
        public static event Action<FactionChatMessage> FactionChat;

        public static void Publish(INetMessage msg)
        {
            switch (msg)
            {
                case GameStartMessage m:   GameStart?.Invoke(m);   break;
                case TurnResultMessage m:  TurnResult?.Invoke(m);  break;
                case GameOverMessage m:    GameOver?.Invoke(m);    break;
                case FactionChatMessage m: FactionChat?.Invoke(m); break;
                // ignore client→server or internal messages
            }
        }
    }
}
