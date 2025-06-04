using UnityEngine;
using UnityEngine.UI;
using System;
using Cysharp.Threading.Tasks;
using BiotonicFrontiers.Net;

namespace BiotonicFrontiers.UI
{
    /// <summary>Colours an icon green when the target player is online.</summary>
    public sealed class PresenceIndicator : MonoBehaviour
    {
        [SerializeField] private Image  icon;
        [SerializeField] private string playerId;

        private void OnEnable() => UpdateLoop().Forget();

        private async UniTaskVoid UpdateLoop()
        {
            while (isActiveAndEnabled)
            {
                bool online = await PresenceService.IsOnlineAsync(playerId);
                icon.color  = online ? Color.green : Color.grey;
                await UniTask.Delay(TimeSpan.FromSeconds(5));
            }
        }
    }
}
