using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using BiotonicFrontiers.Data.DTO;

namespace BiotonicFrontiers.UI
{
    /// <summary>Row used in the Faction scene list.</summary>
    public class FactionRow : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text membersText;
        [SerializeField] private Button   joinBtn;

        private string _factionId;
        private Action _onJoin;

        public void Bind(FactionInfo info, Action onJoin)
        {
            _factionId      = info.faction_id;
            nameText.text   = info.name;
            membersText.text= info.member_count.ToString();

            _onJoin = onJoin;
            joinBtn.onClick.RemoveAllListeners();
            joinBtn.onClick.AddListener(() => _onJoin?.Invoke());
        }

        public string FactionId => _factionId;
    }
}