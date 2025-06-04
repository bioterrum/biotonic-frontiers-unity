using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BiotonicFrontiers.Core;
using BiotonicFrontiers.Data.DTO;
using BiotonicFrontiers.Net;

namespace BiotonicFrontiers.UI
{
    /// <summary>Main entry‑point for the *Factions* scene: lists factions, lets player join/create, and displays chat.</summary>
    public sealed class FactionScreen : MonoBehaviour
    {
        [Header("UI Refs")]
        [SerializeField] private Transform listRoot;
        [SerializeField] private FactionRow rowPrefab;
        [SerializeField] private InputField createInput;
        [SerializeField] private Button createButton;
        [SerializeField] private Text statusLabel;

        private void Awake()
        {
            createButton.onClick.AddListener(OnCreateClicked);
            Refresh().Forget();
        }

        private async Cysharp.Threading.Tasks.UniTaskVoid Refresh()
        {
            try
            {
                var factions = await FactionService.ListAsync();
                foreach (Transform c in listRoot) Destroy(c.gameObject);
                foreach (var f in factions)
                {
                    var row = Instantiate(rowPrefab, listRoot);
                    row.Bind(f, () => Join(f));
                }
            }
            catch (Exception e)
            {
                statusLabel.text = e.Message;
            }
        }

        private async void Join(FactionInfo info)
        {
            statusLabel.text = "Joining…";
            var ok = await FactionService.JoinAsync(Guid.Parse(info.faction_id), Guid.Parse(GameManager.Instance.PlayerId));
            statusLabel.text = ok ? $"Joined {info.name}" : "Join failed";
            if (ok) GameManager.Instance.SetFaction(info.name);
        }

        private async void OnCreateClicked()
        {
            var name = createInput.text.Trim();
            if (string.IsNullOrEmpty(name)) { statusLabel.text = "Enter a name"; return; }
            statusLabel.text = "Creating…";
            var ok = await FactionService.CreateAsync(name, Guid.Parse(GameManager.Instance.PlayerId));
            statusLabel.text = ok ? $"Created {name}" : "Create failed";
            if (ok) GameManager.Instance.SetFaction(name);
        }
    }
}