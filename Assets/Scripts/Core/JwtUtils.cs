using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BiotonicFrontiers.Core
{
    /// <summary>Small helper for decoding claims from a JWT.</summary>
    public static class JwtUtils
    {
        /// <returns>The value of <paramref name="claim"/> or <c>null</c> if missing/invalid.</returns>
        public static string ExtractClaim(string jwt, string claim)
        {
            if (string.IsNullOrEmpty(jwt)) return null;

            string[] parts = jwt.Split('.');
            if (parts.Length < 2) return null;

            try
            {
                string body = parts[1].PadRight(parts[1].Length + (4 - parts[1].Length % 4) % 4, '=')
                                       .Replace('-', '+').Replace('_', '/');
                string json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(body));
                var dict    = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

                return dict != null && dict.TryGetValue(claim, out var val) ? val.ToString() : null;
            }
            catch { return null; }
        }

        public static string GetPlayerId(string jwt) => ExtractClaim(jwt, "pid");
    }
}
