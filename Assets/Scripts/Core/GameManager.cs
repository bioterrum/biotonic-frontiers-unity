// Assets/Scripts/Core/GameManager.cs
using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;                    // ← UniTask
using BiotonicFrontiers.Data;
using BiotonicFrontiers.Events;

namespace BiotonicFrontiers.Core
{
    /// <summary>Singleton that owns global session-state and survives scene loads.</summary>
    public sealed class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        // ----------------------------------------------------------------- //
        // Account / blockchain
        // ----------------------------------------------------------------- //
        public string WalletAddress  { get; private set; }          // Aptos account
        public string AccessToken    { get; private set; }          // JWT
        public string RefreshToken   { get; private set; }
        public string PlayerId       { get; private set; }          // GUID

        // ----------------------------------------------------------------- //
        // Gameplay / meta
        // ----------------------------------------------------------------- //
        public string               CurrentGameId    { get; private set; }
        public List<ItemStack>      PlayerInventory  { get; private set; } = new();
        public string               PlayerFactionName{ get; private set; }

        [HideInInspector] public NetworkManager Net;

        // ----------------------------------------------------------------- //
        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Net = gameObject.GetComponent<NetworkManager>() ?? gameObject.AddComponent<NetworkManager>();
        }

        /// <summary>Try silent re-login on boot if tokens are cached.</summary>
        void Start()
        {
            if (TokenStorage.TryLoad(out var acc, out var refres))
            {
                string pid = JwtUtils.GetPlayerId(acc);
                if (!string.IsNullOrEmpty(pid))
                    SetAuth(acc, refres, pid);
            }
        }

        // ================================================================= //
        // Mutators
        // ================================================================= //

        /// <summary>Stores freshly issued JWTs and initiates the session.</summary>
        public void SetAuth(string access, string refresh, string playerId)
        {
            AccessToken  = access;
            RefreshToken = refresh;
            PlayerId     = playerId;

            TokenStorage.Save(access, refresh);

            // Delay WebSocket connect until the wallet has been resolved
            async void OpenWsWhenReady()
            {
                while (string.IsNullOrEmpty(WalletAddress))
                    await UniTask.DelayFrame(1);

                StartCoroutine(Net.ConnectWebSocket(playerId));
            }
            OpenWsWhenReady();

            // Prime client-side state
            Net.RequestInventory();
            Net.RequestShopStock();
            Net.RequestFactions();
        }

        /// <summary>Persists the player’s Aptos address once the wallet connects.</summary>
        public void SetWallet(string addr)
        {
            WalletAddress = addr;
            PlayerPrefs.SetString("bf.wallet.addr", addr);
        }

        public void SetFaction(string factionName)         => PlayerFactionName = factionName;
        public void SetCurrentGame(string gameId)
        {
            CurrentGameId = gameId;
            EventBus.Publish(EventNames.Server_GameStart, gameId);
        }

        public void SetInventory(List<ItemStack> items)
        {
            PlayerInventory = items;
            EventBus.Publish(NetworkEvents.InventoryUpdated, items);
        }
    }
}
