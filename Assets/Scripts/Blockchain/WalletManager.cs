// Assets/Scripts/Blockchain/WalletManager.cs
// -----------------------------------------------------------------------------
//  Cross‑platform Aptos wallet helper. Integrates the Aptos Unity SDK when it is
//  present, but still compiles (in Editor/CI) without the package by providing
//  stub types behind the APTOS_SDK_PRESENT define.
// -----------------------------------------------------------------------------
using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using BiotonicFrontiers.Core;              // → GameManager

#if APTOS_SDK_PRESENT
using Aptos.Accounts;                     // Account
using Aptos.Unity.Rest;                   // Client
using Aptos.Unity.WalletAdapter;          // WebWalletAdapter (UPM‑installed)
#endif

namespace BiotonicFrontiers.Blockchain
{
    /// <summary>
    /// Handles wallet creation / connection across all targets and exposes the
    /// connected Aptos address to the rest of the client.  A singleton that is
    /// created in the Login scene (or boot‑strap) and survives scene loads.
    /// </summary>
    public sealed class WalletManager : MonoBehaviour
    {
        //---------------------------------------------------------------------
        // Singleton plumbing
        //---------------------------------------------------------------------
        public static WalletManager Instance { get; private set; }

        /// <summary>Player’s Aptos address once a wallet is connected.</summary>
        public string WalletAddress { get; private set; }

        [Header("Behaviour")]
        [SerializeField] private bool      autoCreateOnMobile = true;
        [SerializeField] private GameObject connectPanel;          // optional UI

#if APTOS_SDK_PRESENT
        private Account _account;         // local keypair (mobile)
        private Client  _client;          // lightweight REST client
#endif

        //---------------------------------------------------------------------
        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

#if APTOS_SDK_PRESENT
            _client = new Client(EnvironmentConfig.NodeUrl);
#endif
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Opens a wallet‑adapter popup on desktop/WebGL or silently creates/loads
        /// a keypair on mobile builds.  Stores the resolved address and notifies
        /// <see cref="GameManager"/> so the rest of the client can start loading
        /// on‑chain data.
        /// </summary>
        public async UniTask ConnectAsync()
        {
#if UNITY_WEBGL || UNITY_STANDALONE
#if !APTOS_SDK_PRESENT
            throw new PlatformNotSupportedException(
                "Aptos Unity SDK is not present – import it via UPM (git URL).");
#else
            if (!WebWalletAdapter.IsConnected)
            {
                bool ok = await WebWalletAdapter.Connect();   // opens wallet UI
                if (!ok) throw new Exception("Wallet connection cancelled by user.");
            }
            WalletAddress = WebWalletAdapter.AccountAddress;
#endif
#elif UNITY_IOS || UNITY_ANDROID
#if !APTOS_SDK_PRESENT
            throw new PlatformNotSupportedException(
                "Aptos Unity SDK is not present – import it via UPM (git URL).");
#else
            string storedPriv = PlayerPrefs.GetString("bf.aptos.priv", null);
            _account = string.IsNullOrEmpty(storedPriv)
                ? Account.Generate()
                : new Account(Convert.FromHexString(storedPriv));

            if (string.IsNullOrEmpty(storedPriv))
                PlayerPrefs.SetString("bf.aptos.priv", BitConverter.ToString(_account.PrivateKey)
                    .Replace("-", string.Empty).ToLower());

            WalletAddress = _account.Address;
#endif
#else
            throw new PlatformNotSupportedException("Unsupported build target.");
#endif
            // Persist + broadcast
            GameManager.Instance.SetWallet(WalletAddress);
            if (connectPanel) connectPanel.SetActive(false);
        }

#if APTOS_SDK_PRESENT
        //------------------------------------------------------------------
        /// <summary>
        /// Submits a raw **BCS‑encoded** transaction already signed by the backend
        /// gas‑station. Returns the resulting transaction hash.
        /// </summary>
        public async UniTask<string> SubmitBCSTransaction(byte[] txnBytes)
        {
            var resp = await _client.SubmitBCSTransaction(txnBytes);
            return resp.Hash;
        }
#endif
    }

    // ======================================================================
    //  Fallback stubs – compiled only when the Aptos Unity SDK is missing so
    //  that CI and non‑blockchain devs can still open the project without
    //  errors.  Once the SDK is imported these are ignored.
    // ======================================================================
#if !APTOS_SDK_PRESENT
    // Minimal no‑op replacement for the Aptos SDK’s Account type.
    internal sealed class Account
    {
        public static Account Generate() => new();
        public string Address    => "0x00000000000000000000000000000000";
        public byte[] PrivateKey => Array.Empty<byte>();
        public Account() {}
        public Account(byte[] _) {}
    }

    // Lightweight stub of the REST Client so other scripts still compile. The
    // real client lives in the SDK; this one is used only when the SDK is gone.
    internal sealed class Client
    {
        public Client(string _) {}
        public UniTask<string> SubmitBCSTransaction(byte[] _) =>
            UniTask.FromResult("0xDEADBEEF");
    }

    // Stub WebWalletAdapter with the same API surface.
    internal static class WebWalletAdapter
    {
        public static bool   IsConnected    => false;
        public static string AccountAddress => string.Empty;
        public static UniTask<bool> Connect() => UniTask.FromResult(false);
    }

    // Provide a dummy SubmitBCSTransaction so other scripts can compile even when
    // the SDK is absent (e.g. in CI or on a designer’s PC).
    internal static class WalletManagerFallbackExtensions
    {
        public static UniTask<string> SubmitBCSTransaction(this WalletManager _, byte[] __)
            => UniTask.FromResult("0xDEADBEEF");
    }
#endif
}
