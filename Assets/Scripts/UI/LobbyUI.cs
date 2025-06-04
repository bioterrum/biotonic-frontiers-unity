using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using BiotonicFrontiers.Core;
using BiotonicFrontiers.Data.DTO;
using BiotonicFrontiers.Events;

namespace BiotonicFrontiers.UI
{
    public class LobbyUI : MonoBehaviour
    {
        [Header("Profile Display")]
        [SerializeField] private TMP_Text playerNameText;
        [SerializeField] private TMP_Text factionText;

        [Header("Matchmaking UI")]
        [SerializeField] private Button findMatchButton;
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private GameObject waitingIndicator;

        void OnEnable()
        {
            findMatchButton.onClick.AddListener(OnFindMatchClicked);
            EventBus.Subscribe(EventNames.Server_MatchFound, OnMatchFound);

            UpdateProfileDisplay();
            waitingIndicator.SetActive(false);
        }

        void OnDisable()
        {
            findMatchButton.onClick.RemoveListener(OnFindMatchClicked);
            EventBus.Unsubscribe(EventNames.Server_MatchFound, OnMatchFound);
        }

        private void UpdateProfileDisplay()
        {
            // Display player ID as a proxy for name; replace with actual name when available
            playerNameText.text = GameManager.Instance.PlayerId;
            factionText.text = string.IsNullOrEmpty(GameManager.Instance.PlayerFactionName)
                ? "No Faction" 
                : GameManager.Instance.PlayerFactionName;
        }

        private void OnFindMatchClicked()
        {
            findMatchButton.interactable = false;
            statusText.text = "Searching...";
            waitingIndicator.SetActive(true);

            var req = new MatchmakingRequest {
                player_id  = GameManager.Instance.PlayerId,
                elo_rating = 1500
            };
            StartCoroutine(
                GameManager.Instance.Net.Post<MatchmakingRequest, object>(
                    "matchmaking/join", req,
                    _ => { /* queued */ },
                    err => {
                        statusText.text = "Error: " + err;
                        findMatchButton.interactable = true;
                        waitingIndicator.SetActive(false);
                    }
                )
            );
        }

        private void OnMatchFound(object payload)
        {
            var msg = (MatchFound)payload;
            statusText.text = "Match found! Loading duel…";
            GameManager.Instance.SetCurrentGame(msg.game_id);
            waitingIndicator.SetActive(false);
            SceneManager.LoadScene("Duel");
        }
    }
}