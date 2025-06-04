using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using BiotonicFrontiers.Core;
using BiotonicFrontiers.Blockchain;
using BiotonicFrontiers.Net;          // ← fixes missing HttpClient symbol

[RequireComponent(typeof(Button))]
public class ClaimTestNftButton : MonoBehaviour
{
    [SerializeField] private Button        btn;
    [SerializeField] private TMPro.TMP_Text label;

    void Reset()
    {
        btn   = GetComponent<Button>();
        label = GetComponentInChildren<TMPro.TMP_Text>();
    }

    void Awake() => btn.onClick.AddListener(() => Claim().Forget());

    // ------------------------------------------------------------------ //
    // Mint a dummy prototype NFT via the backend relay + (optional) poll chain
    // ------------------------------------------------------------------ //
    private async UniTaskVoid Claim()
    {
        btn.interactable = false;
        label.text = "Minting…";

        // 1️⃣  Call the sponsored-tx relay – returns base-64 BCS bytes
        var body = new JObject { ["type"] = "claim_test_nft" };
        string b64 = await HttpClient.PostAsync<string>("/tx/sponsored", body);

        // 2️⃣  Submit the BCS blob through the connected wallet
        byte[] bcs = System.Convert.FromBase64String(b64);
        string hash = await WalletManager.Instance.SubmitBCSTransaction(bcs);

        // 3️⃣  Poll the chain until the tx lands (safe on devnet)
#if APTOS_SDK_PRESENT
        var client = new Aptos.Unity.Rest.Client(EnvironmentConfig.NodeUrl);
        while (true)
        {
            var tx = await client.GetTransaction(hash);
            if (tx.Success) break;
            await UniTask.Delay(500);
        }
#else
        // No SDK – assume success instantly to keep editor builds green
        await UniTask.Delay(500);
#endif

        // 4️⃣  Force an inventory refresh so the new NFT shows up
        EventBus.Publish(NetworkEvents.InventoryUpdated);

        label.text = "NFT minted!";
        await UniTask.Delay(1500);
        label.text = "Claim Test NFT";
        btn.interactable = true;
    }
}
