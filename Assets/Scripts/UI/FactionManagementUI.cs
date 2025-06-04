using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using BiotonicFrontiers.Core;
using BiotonicFrontiers.Data.DTO;
using TMPro;

namespace BiotonicFrontiers.UI
{
    /// <summary>UI panel for viewing/creating/joining/leaving factions.</summary>
    public class FactionManagementUI : MonoBehaviour
    {
        [Header("Current Faction")]
        [SerializeField] private TMP_Text currentFactionText;
        [SerializeField] private Button leaveButton;

        [Header("Browse Factions")]
        [SerializeField] private Transform listRoot;
        [SerializeField] private FactionRow rowPrefab;
        [SerializeField] private Button refreshButton;

        [Header("Create Faction")]
        [SerializeField] private TMP_InputField createNameInput;
        [SerializeField] private Button createButton;
        [SerializeField] private TMP_Text createStatus;

        private void Awake()
        {
            refreshButton.onClick.AddListener(RequestList);
            createButton.onClick.AddListener(OnCreateClicked);
            leaveButton.onClick.AddListener(OnLeaveClicked);
        }

        private void OnEnable()
        {
            EventBus.Subscribe(NetworkEvents.FactionListUpdated, OnListUpdated);
            UpdateCurrentFactionDisplay();
            RequestList();
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe(NetworkEvents.FactionListUpdated, OnListUpdated);
        }

        private void UpdateCurrentFactionDisplay()
        {
            var fid = GameManager.Instance.PlayerFactionName;
            if (string.IsNullOrEmpty(fid))
            {
                currentFactionText.text = "No Faction";
                leaveButton.interactable = false;
            }
            else
            {
                currentFactionText.text = $"Faction: {fid}";
                leaveButton.interactable = true;
            }
        }

        private void RequestList()
        {
            NetworkManager.Instance.RequestFactions();
        }

        private void OnListUpdated(object payload)
        {
            // Clear existing list
            foreach (Transform child in listRoot)
                Destroy(child.gameObject);

            // Populate with fresh data
            foreach (var info in (List<FactionInfo>)payload)
            {
                var row = Instantiate(rowPrefab, listRoot);
                row.Bind(info, () => OnJoinClicked(info.faction_id));
            }
        }

        private void OnCreateClicked()
        {
            var name = createNameInput.text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                createStatus.text = "Enter a name.";
                return;
            }

            NetworkManager.Instance.CreateFaction(
                name,
                // onSuccess
                () =>
                {
                    createStatus.text = "Created!";
                    RequestList();
                },
                // onError
                err => createStatus.text = err
            );
        }

        private void OnJoinClicked(string factionId)
        {
            NetworkManager.Instance.JoinFaction(
                factionId,
                // onSuccess
                () =>
                {
                    GameManager.Instance.SetFaction(factionId);
                    UpdateCurrentFactionDisplay();
                    SceneManager.LoadScene("Faction");
                },
                // onError
                err => createStatus.text = err
            );
        }

        private void OnLeaveClicked()
        {
            var factionId = GameManager.Instance.PlayerFactionName;
            if (string.IsNullOrEmpty(factionId)) return;

            NetworkManager.Instance.LeaveFaction(
                factionId,
                // onSuccess
                () =>
                {
                    GameManager.Instance.SetFaction("");
                    UpdateCurrentFactionDisplay();
                    RequestList();
                },
                // onError
                err => createStatus.text = err
            );
        }
    }
}
