using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using NativeWebSocket;
using Newtonsoft.Json;
using BiotonicFrontiers.Data;
using BiotonicFrontiers.Data.DTO;
using BiotonicFrontiers.Events;

namespace BiotonicFrontiers.Core
{
    /// <summary>Centralized HTTP + WebSocket helper.</summary>
    public class NetworkManager : MonoBehaviour
    {
        public ServerConfig Config => config;
        public static NetworkManager Instance { get; private set; }

        [Header("Server Configuration")]
        [SerializeField] private ServerConfig config;

        private WebSocket _ws;

        // ---------------------- Singleton plumbing -----------------------
        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        #region WebSocket ---------------------------------------------------
        public IEnumerator ConnectWebSocket(string playerId)
        {
            _ws = new WebSocket(config.wsUrlBase + playerId);

            _ws.OnOpen += () => Debug.Log("WS connected");
            _ws.OnError += e => Debug.LogError("WS error " + e);
            _ws.OnClose += c => Debug.LogWarning("WS closed – code " + c);
            _ws.OnMessage += bytes =>
            {
                string json = Encoding.UTF8.GetString(bytes);
                MessageRouter.Dispatch(json);
            };

            yield return _ws.Connect();
        }

        void Update() => _ws?.DispatchMessageQueue();
        async void OnApplicationQuit() { if (_ws != null) await _ws.Close(); }

        public async void SendJson(object payload)
        {
            if (_ws == null || _ws.State != WebSocketState.Open) return;
            string json = JsonConvert.SerializeObject(payload);
            await _ws.SendText(json);
        }
        #endregion

        #region Generic REST wrappers --------------------------------------
        IEnumerator Req(UnityWebRequest req, Action<string> ok, Action<string> err)
        {
            req.SetRequestHeader("Authorization", "Bearer " + GameManager.Instance.AccessToken);
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
                ok?.Invoke(req.downloadHandler.text);
            else
                err?.Invoke(req.error);
        }

        IEnumerator Get<TRes>(string path, Action<TRes> ok, Action<string> err)
        {
            using var req = UnityWebRequest.Get(config.httpBase + "/" + path);
            return Req(req, txt => ok(JsonConvert.DeserializeObject<TRes>(txt)), err);
        }

        public IEnumerator Post<TReq, TRes>(string path, TReq body, Action<TRes> ok, Action<string> err)
        {
            var req = new UnityWebRequest(config.httpBase + "/" + path, UnityWebRequest.kHttpVerbPOST)
            {
                uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body))),
                downloadHandler = new DownloadHandlerBuffer()
            };
            req.SetRequestHeader("Content-Type", "application/json");
            return Req(req, txt => ok(typeof(TRes) == typeof(object) ? default : JsonConvert.DeserializeObject<TRes>(txt)), err);
        }
        #endregion

        #region High-level wrappers ------------------------------------------
        // ---- Economy ------------
        public void RequestShopStock() =>
            StartCoroutine(Get<List<ShopItem>>("shop/items",
                items => EventBus.Publish(EventNames.Client_ShopUpdated, items),
                err => Debug.LogError("Shop req failed: " + err)));

        public void BuyItem(string itemId, int qty) =>
            StartCoroutine(Post<BuyReq, object>("shop/buy",
                new BuyReq { player_id = GameManager.Instance.PlayerId, item_id = int.Parse(itemId), quantity = qty },
                _ => RequestShopStock(),
                err => Debug.LogError(err)));

        public void UseItem(string itemId) =>
            StartCoroutine(Post<UseItemReq, object>("inventory/use",
                new UseItemReq { player_id = GameManager.Instance.PlayerId, item_id = itemId },
                _ => RequestInventory(),
                err => Debug.LogError(err)));

        public void RequestInventory() =>
            StartCoroutine(Get<List<ItemStack>>($"inventory/{GameManager.Instance.PlayerId}",
                items =>
                {
                    GameManager.Instance.SetInventory(items);
                    EventBus.Publish(EventNames.Client_InventoryUpdated, items);
                },
                err => Debug.LogError("Inventory req failed: " + err)));

        // Adds SellItem to match SellDialog usage
        public void SellItem(string itemId, int qty) =>
            StartCoroutine(Post<SellReq, object>("shop/sell",
                new SellReq { player_id = GameManager.Instance.PlayerId, item_id = itemId, quantity = qty },
                _ =>
                {
                    RequestInventory();
                    RequestShopStock();
                },
                err => Debug.LogError(err)));

        // ---- Social / Factions ----
        public void RequestFactions() =>
            StartCoroutine(Get<List<FactionInfo>>("factions/list",
                list => EventBus.Publish(EventNames.Client_FactionList, list),
                err => Debug.LogError("Factions req failed: " + err)));

        public void CreateFaction(string name, Action onSuccess, Action<string> onErr) =>
            StartCoroutine(Post<FactionCreateReq, object>("factions/create",
                new FactionCreateReq { name = name },
                _ => onSuccess?.Invoke(),
                err => onErr?.Invoke(err)));

        public void JoinFaction(string factionId, Action onSuccess, Action<string> onErr) =>
            StartCoroutine(Post<FactionJoinReq, object>("factions/join",
                new FactionJoinReq { faction_id = factionId },
                _ => onSuccess?.Invoke(),
                err => onErr?.Invoke(err)));

        public void LeaveFaction(string factionId, Action onSuccess, Action<string> onErr) =>
            StartCoroutine(Post<FactionLeaveReq, object>("factions/leave",
                new FactionLeaveReq { faction_id = factionId },
                _ => onSuccess?.Invoke(),
                err => onErr?.Invoke(err)));

        // ---- Faction Chat ----
        public void SendFactionChat(string message)
        {
            var payload = new { type = NetworkEvents.FactionChat, faction_id = GameManager.Instance.PlayerFactionName, content = message };
            SendJson(payload);
        }

        // ---- Claim Land -------------------------------------------------------
        public void ClaimLand(int x, int y)
        {
            StartCoroutine(Post<ClaimReq, object>(
                "land/claim",
                new ClaimReq { faction_id = GameManager.Instance.PlayerFactionName, x = x, y = y, biome_type = "plains" },
                _ => Debug.Log($"Claimed land ({x},{y})"),
                err => Debug.LogError(err)
            ));
        }

        // ---- Land Ownership ---------------------------------------------
        /// <summary>Returns every parcel owned by the current player/faction.</summary>
        public void RequestLandOwnership() =>
            StartCoroutine(Get<List<LandParcel>>(
                $"land/owned/{GameManager.Instance.PlayerId}",
                parcels => EventBus.Publish(EventNames.Server_LandOwnership, parcels),
                err => Debug.LogError("Land ownership req failed: " + err)));
        #endregion

        [Serializable]
        struct SellReq { public string player_id; public string item_id; public int quantity; }
        
        // ---- Aptos coin balance ---------------------------------------------------
        public void RequestCoinBalance()
        {
            StartCoroutine(Get<int>($"aptos/coins/{GameManager.Instance.PlayerId}",
                bal => EventBus.Publish(NetworkEvents.CoinBalanceUpdated, bal),
                err => Debug.LogError("Coin balance req failed: " + err)));
        }

        // ---- Mint on-chain NFT ----------------------------------------------------
        public void MintPrototypeUnit(string protoId)
        {
            var body = new { player_id = GameManager.Instance.PlayerId, proto_id = protoId };
            StartCoroutine(Post<object, object>("aptos/mint_prototype", body,
                _   => Debug.Log($"Mint tx submitted for {protoId}"),
                err => Debug.LogError(err)));
        }

    }
}