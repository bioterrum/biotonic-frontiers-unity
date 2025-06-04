// Assets/Scripts/UI/AuthManager.cs
// -----------------------------------------------------------------------------
// Unified magic-link authentication manager (public UI refs)
// -----------------------------------------------------------------------------

using System;
using System.Collections;
using System.Text;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using BiotonicFrontiers.Core;

namespace BiotonicFrontiers.UI
{
    public sealed class AuthManager : MonoBehaviour
    {
        // ───────────────────────────── UI references (must be PUBLIC for generator)
        [Header("UI References")]
        public TMP_InputField emailInput;
        public Button         sendLinkButton;
        public TMP_InputField tokenInput;
        public Button         verifyButton;
        public TMP_Text       statusLabel;

        // Convenience accessor for current ServerConfig
        private ServerConfig Config => NetworkManager.Instance.Config;

        // ───────────────────────────────────────────────────────── lifecycle ──
        private void Awake()
        {
            // Ensure the buttons are wired exactly once
            sendLinkButton.onClick.RemoveAllListeners();
            sendLinkButton.onClick.AddListener(SendMagicLink);

            verifyButton.onClick.RemoveAllListeners();
            verifyButton.onClick.AddListener(VerifyToken);
        }

        // ------------------------------------------------------------------ DTO
        [Serializable]
        private struct TokenResponse
        {
            public string access_token;
            public string refresh_token;
            public long   expires_in;
        }

        // ─────────────────────────────────────────────────────────── handlers ──
        public void SendMagicLink()
        {
            string email = emailInput.text.Trim();
            if (string.IsNullOrEmpty(email))
            {
                statusLabel.text = "Enter a valid e-mail.";
                return;
            }
            StartCoroutine(CoSendMagicLink(email));
        }

        public void VerifyToken()
        {
            string token = tokenInput.text.Trim();
            if (string.IsNullOrEmpty(token))
            {
                statusLabel.text = "Paste the token you received.";
                return;
            }
            StartCoroutine(CoVerify(token));
        }

        // ───────────────────────────────────────────────────────── coroutines ──
        private IEnumerator CoSendMagicLink(string email)
        {
            statusLabel.text = "Sending…";
            string uri  = $"{Config.httpBase}/magic_link";
            string body = JsonConvert.SerializeObject(new { email });

            using var req = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbPOST)
            {
                uploadHandler   = new UploadHandlerRaw(Encoding.UTF8.GetBytes(body)),
                downloadHandler = new DownloadHandlerBuffer()
            };
            req.SetRequestHeader("Content-Type", "application/json");
            yield return req.SendWebRequest();

            statusLabel.text = req.result == UnityWebRequest.Result.Success
                ? "Magic link sent – check console/e-mail."
                : $"Error: {req.error}";
        }

        private IEnumerator CoVerify(string rawToken)
        {
            statusLabel.text = "Verifying…";
            // /verify lives at root, not under /api
            string uri = $"{Config.httpBase.Replace("/api", "")}/verify?token={rawToken}";
            using var req = UnityWebRequest.Get(uri);
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                statusLabel.text = $"Verify failed: {req.error}";
                yield break;
            }

            var res = JsonConvert.DeserializeObject<TokenResponse>(req.downloadHandler.text);
            string playerId = JwtUtils.GetPlayerId(res.access_token);

            if (string.IsNullOrEmpty(playerId))
            {
                statusLabel.text = "JWT missing pid claim.";
                yield break;
            }

            GameManager.Instance.SetAuth(res.access_token, res.refresh_token, playerId);
            statusLabel.text = "Login OK! Loading menu…";
            yield return new WaitForSeconds(0.4f);

            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }
    }
}
