using System;
using System.Threading;
using System.Text;
using Cysharp.Threading.Tasks;
using NativeWebSocket;
using UnityEngine;
using BiotonicFrontiers.Net.Messages;

namespace BiotonicFrontiers.Net
{
    /// <summary>JSON WebSocket bridge: <c>INetMessage</c> ⇆ server.</summary>
    public sealed class WSConnection
    {
        private readonly string _url;
        private WebSocket       _socket;
        private readonly CancellationTokenSource _cts = new();

        public WSConnection(string url) => _url = url;

        // ------------------------------------------------------------------
        public async UniTask ConnectAsync()
        {
            _socket = new WebSocket(_url);
            _socket.OnMessage += OnMessage;
            _socket.OnError   += e   => Debug.LogError($"WS error: {e}");
            _socket.OnClose   += c   => Debug.LogWarning($"WS closed – code {c}");
            await _socket.Connect();
        }

        public async UniTask DisconnectAsync()
        {
            _cts.Cancel();
            if (_socket != null) await _socket.Close();
        }

        public async UniTask SendAsync(INetMessage msg)
        {
            var json = NetMessageParser.Serialize(msg);
            await _socket.SendText(json);
        }

        // ------------------------------------------------------------------
        private void OnMessage(byte[] data)
        {
            var json = Encoding.UTF8.GetString(data);
            if (NetMessageParser.Deserialize(json) is { } parsed)
                EventBus.Publish(parsed);
            else
                Debug.LogWarning($"[WS] Unknown payload:\n{json}");
        }

        /// <summary>Pumps <see cref="WebSocket.DispatchMessageQueue"/> every frame.</summary>
        public async UniTaskVoid StartPump()
        {
            try
            {
                while (!_cts.IsCancellationRequested)
                {
                    _socket.DispatchMessageQueue();
                    await UniTask.DelayFrame(1, PlayerLoopTiming.Update, _cts.Token);
                }
            }
            catch (OperationCanceledException) { /* normal shutdown */ }
        }
    }
}
