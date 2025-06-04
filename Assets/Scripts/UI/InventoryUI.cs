// Assets/Scripts/UI/Inventory/InventoryUI.cs
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using BiotonicFrontiers.Core;
using BiotonicFrontiers.Data;
using BiotonicFrontiers.Net;
using BiotonicFrontiers.Events;

namespace BiotonicFrontiers.UI
{
    /// <summary>
    /// Combines server-side items with on-chain NFTs and rebuilds the grid
    /// whenever <see cref="NetworkEvents.InventoryUpdated"/> fires.
    /// </summary>
    public sealed class InventoryUI : MonoBehaviour
    {
        [SerializeField] private Transform     gridRoot;
        [SerializeField] private InventorySlot slotPrefab;

        // ──────────────────────────────────────────────────────────────── //
        void OnEnable()
        {
            EventBus.Subscribe(NetworkEvents.InventoryUpdated, OnInventoryUpdated);
            Refresh().Forget();
        }

        void OnDisable() =>
            EventBus.Unsubscribe(NetworkEvents.InventoryUpdated, OnInventoryUpdated);

        private void OnInventoryUpdated(object _) => Refresh().Forget();

        // ──────────────────────────────────────────────────────────────── //
        /// <summary>Fetches Web-2 inventory and NFTs, then rebuilds the UI grid.</summary>
        private async UniTaskVoid Refresh()
        {
            var stacks = new List<ItemStack>();

            // 1️⃣  Web-2 inventory (backend)
            if (!string.IsNullOrEmpty(GameManager.Instance.PlayerId))
            {
                Guid pid = Guid.Parse(GameManager.Instance.PlayerId);
                var netStacks = await InventoryService.GetAsync(pid);
                foreach (var s in netStacks)
                    stacks.Add(new ItemStack { itemId = s.item_id.ToString(), quantity = s.quantity });
            }

            // 2️⃣  On-chain NFTs
            string wallet = GameManager.Instance.WalletAddress;
            if (!string.IsNullOrEmpty(wallet))
                stacks.AddRange(await FetchNftsAsync(wallet));

            // 3️⃣  Rebuild grid
            foreach (Transform child in gridRoot)
                Destroy(child.gameObject);

            foreach (var stack in stacks)
                Instantiate(slotPrefab, gridRoot).Bind(stack);
        }

        // ──────────────────────────────────────────────────────────────── //
        /// <summary>Queries Aptos REST to enumerate NFTs owned by <paramref name="addr"/>.</summary>
        private async UniTask<List<ItemStack>> FetchNftsAsync(string addr)
        {
            var list = new List<ItemStack>();

            string baseUrl = NetworkManager.Instance.Config.aptosNodeUrl.TrimEnd('/');
            string url     = $"{baseUrl}/v1/accounts/{addr}/tokens?limit=200";

            using var req = UnityWebRequest.Get(url);
            await req.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (req.result != UnityWebRequest.Result.Success)
#else
            if (req.isNetworkError || req.isHttpError)
#endif
            {
                Debug.LogWarning($"NFT fetch failed: {req.error}");
                return list;
            }

            try
            {
                JArray tokens = JArray.Parse(req.downloadHandler.text);
                foreach (JObject tok in tokens)
                {
                    string id  = tok["token_data_id"]?.ToString() ??
                                 tok["token_data_id_hash"]?.ToString();
                    string uri = tok["metadata_uri"]?.ToString() ??
                                 tok["uri"]?.ToString();

                    if (string.IsNullOrEmpty(id)) continue;

                    list.Add(new ItemStack { itemId = id, quantity = 1 });

                    if (!ItemDatabase.HasIcon(id) && !string.IsNullOrEmpty(uri))
                        ItemDatabase.CacheIcon(id, await DownloadSpriteAsync(uri));
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"NFT JSON parse error: {e.Message}");
            }

            return list;
        }

        // ──────────────────────────────────────────────────────────────── //
        /// <summary>Downloads an image and converts it to a sprite.</summary>
        private static async UniTask<Sprite> DownloadSpriteAsync(string url)
        {
            using var req = UnityWebRequestTexture.GetTexture(url);
            await req.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (req.result != UnityWebRequest.Result.Success)
#else
            if (req.isNetworkError || req.isHttpError)
#endif
            {
                Debug.LogWarning($"Icon download failed: {req.error}");
                return null;
            }

            var tex = DownloadHandlerTexture.GetContent(req);
            tex.filterMode = FilterMode.Bilinear;

            return Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f),
                100); // PPU
        }
    }
}
