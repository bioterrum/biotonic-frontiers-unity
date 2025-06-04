using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using BiotonicFrontiers.Core;
using BiotonicFrontiers.Data.DTO;
using BiotonicFrontiers.Events;

namespace BiotonicFrontiers.UI
{
    /// <summary>Handles turn-based duel UI: receives server events and sends turn requests.</summary>
    public class DuelUI : MonoBehaviour
    {
        [SerializeField] private Button endTurnButton;
        [SerializeField] private Transform unitsContainer;

        private uint currentTurn;

        private void OnEnable()
        {
            EventBus.Subscribe(EventNames.Server_GameStart, OnGameStart);
            EventBus.Subscribe(EventNames.Server_TurnResult, OnTurnResult);
            EventBus.Subscribe(EventNames.Server_GameOver, OnGameOver);
            endTurnButton.onClick.AddListener(OnEndTurnClicked);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe(EventNames.Server_GameStart, OnGameStart);
            EventBus.Unsubscribe(EventNames.Server_TurnResult, OnTurnResult);
            EventBus.Unsubscribe(EventNames.Server_GameOver, OnGameOver);
            endTurnButton.onClick.RemoveListener(OnEndTurnClicked);
        }

        private void OnGameStart(object payload)
        {
            var gs = (GameStart)payload;
            currentTurn = gs.turn;
            // TODO: initialize unit visuals based on gs.turn or reset UI
        }

        private void OnTurnResult(object payload)
        {
            var tr = (TurnResult)payload;
            // TODO: animate spawned/destroyed units using tr.spawned/tr.destroyed
            currentTurn = tr.turn;
            endTurnButton.interactable = true;
        }

        private void OnGameOver(object payload)
        {
            var goMsg = (GameOver)payload;
            // TODO: display win/lose UI based on goMsg.winner_id
        }

        private void OnEndTurnClicked()
        {
            var req = new TurnRequest
            {
                game_id   = GameManager.Instance.CurrentGameId,
                player_id = GameManager.Instance.PlayerId,
                turn      = currentTurn,
                actions   = new List<TurnAction>() // TODO: collect player actions
            };

            GameManager.Instance.Net.SendJson(req);
            endTurnButton.interactable = false;
        }
    }
}
