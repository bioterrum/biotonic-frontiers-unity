using System;
using UnityEngine;
using TMPro;
using BiotonicFrontiers.Data.DTO;
using BiotonicFrontiers.Net;
using Cysharp.Threading.Tasks;

namespace BiotonicFrontiers.UI
{
    public class LeaderboardUI : MonoBehaviour
    {
        [Header("Setup (assigned via SceneGenerator)")]
        [SerializeField] private RectTransform listRoot;
        [SerializeField] private LeaderboardRow rowPrefab;
        [SerializeField] private TMP_Text statusText;

        [Header("Config")]
        [Tooltip("Max number of entries to fetch")]
        [SerializeField] private int limit = 20;

        void OnEnable()
        {
            LoadLeaderboard().Forget();
        }

        private async UniTaskVoid LoadLeaderboard()
        {
            statusText.text = "Loading leaderboard...";
            ClearList();

            try
            {
                var entries = await LeaderboardService.GetTopAsync(limit);
                if (entries == null || entries.Count == 0)
                {
                    statusText.text = "No entries found.";
                    return;
                }

                for (int i = 0; i < entries.Count; i++)
                {
                    var entry = entries[i];
                    var row = Instantiate(rowPrefab, listRoot);
                    row.Bind(i + 1, entry.nickname, entry.eloRating);
                }

                statusText.text = $"Top {entries.Count}";
            }
            catch (Exception ex)
            {
                Debug.LogError($"Leaderboard load failed: {ex}");
                statusText.text = "Error loading leaderboard.";
            }
        }

        private void ClearList()
        {
            for (int i = listRoot.childCount - 1; i >= 0; i--)
                Destroy(listRoot.GetChild(i).gameObject);
        }
    }
}
