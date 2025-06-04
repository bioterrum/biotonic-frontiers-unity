// Assets/Scripts/Net/HttpClient.cs
using System;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using Newtonsoft.Json;
using BiotonicFrontiers.Core;

namespace BiotonicFrontiers.Net
{
    /// <summary>Very small helper around UnityWebRequest for REST calls.</summary>
    public static class HttpClient
    {
        private static string ApiBase => NetworkManager.Instance.Config.httpBase;

        private static void AddAuth(UnityWebRequest req)
        {
            var tok = GameManager.Instance?.AccessToken;
            if (!string.IsNullOrEmpty(tok))
                req.SetRequestHeader("Authorization", "Bearer " + tok);
        }

        // ------------------------------------------------------------------ //
        // GET
        // ------------------------------------------------------------------ //
        public static async UniTask<T> GetAsync<T>(string relative)
        {
            using var req = UnityWebRequest.Get(ApiBase + relative);
            AddAuth(req);
            await req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
                throw new Exception($"GET {relative} failed: {req.error}");

            return JsonConvert.DeserializeObject<T>(req.downloadHandler.text);
        }

        // ------------------------------------------------------------------ //
        // POST – no response body
        // ------------------------------------------------------------------ //
        public static async UniTask<bool> PostAsync(string relative, object body)
        {
            var json = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body));
            using var req = new UnityWebRequest(ApiBase + relative, UnityWebRequest.kHttpVerbPOST)
            {
                uploadHandler   = new UploadHandlerRaw(json),
                downloadHandler = new DownloadHandlerBuffer()
            };
            req.SetRequestHeader("Content-Type", "application/json");
            AddAuth(req);
            await req.SendWebRequest();
            return req.result == UnityWebRequest.Result.Success;
        }

        // ------------------------------------------------------------------ //
        // POST – typed response body (generic overload added)
        // ------------------------------------------------------------------ //
        public static async UniTask<T> PostAsync<T>(string relative, object body)
        {
            var json = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body));
            using var req = new UnityWebRequest(ApiBase + relative, UnityWebRequest.kHttpVerbPOST)
            {
                uploadHandler   = new UploadHandlerRaw(json),
                downloadHandler = new DownloadHandlerBuffer()
            };
            req.SetRequestHeader("Content-Type", "application/json");
            AddAuth(req);
            await req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
                throw new Exception($"POST {relative} failed: {req.error}");

            return JsonConvert.DeserializeObject<T>(req.downloadHandler.text);
        }
    }
}
