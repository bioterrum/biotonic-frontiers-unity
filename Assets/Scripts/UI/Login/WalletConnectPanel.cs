using UnityEngine;
using UnityEngine.UI;
using BiotonicFrontiers.Blockchain;
using Cysharp.Threading.Tasks;
using BiotonicFrontiers;

public class WalletConnectPanel : MonoBehaviour
{
    [SerializeField] Button connectBtn;
    [SerializeField] TMPro.TMP_Text status;

    void Awake() => connectBtn.onClick.AddListener(() => Connect().Forget());

    async UniTaskVoid Connect()
    {
        status.text = "Connecting wallet…";
        try
        {
            await WalletManager.Instance.ConnectAsync();
            status.text = $"Connected: {WalletManager.Instance.WalletAddress.Short()}";
            gameObject.SetActive(false);
        }
        catch (System.Exception e)
        {
            status.text = e.Message;
        }
    }
}
