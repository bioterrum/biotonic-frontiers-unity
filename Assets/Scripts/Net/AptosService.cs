using System;
using Cysharp.Threading.Tasks;

namespace BiotonicFrontiers.Net
{
    /// <summary>Thin REST helpers for the Aptos integration routes.</summary>
    public static class AptosService
    {
        public static async UniTask<int> GetBalanceAsync(Guid player) =>
            await HttpClient.GetAsync<int>($"/aptos/coins/{player}");

        public static async UniTask<bool> MintAsync(Guid player, string protoId)
        {
            var body = new { player_id = player, proto_id = protoId };
            return await HttpClient.PostAsync("/aptos/mint_prototype", body);
        }
    }
}
