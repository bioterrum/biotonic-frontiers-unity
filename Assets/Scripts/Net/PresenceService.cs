using Cysharp.Threading.Tasks;

namespace BiotonicFrontiers.Net
{
    /// <summary>Stub presence checker — replace with a real API later.</summary>
    public static class PresenceService
    {
        public static async UniTask<bool> IsOnlineAsync(string playerId)
        {
            await UniTask.CompletedTask;     // fake latency
            return true;                     // always “online” for now
        }
    }
}
