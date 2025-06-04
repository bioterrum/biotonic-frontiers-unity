using UnityEngine;
using TMPro;

namespace BiotonicFrontiers.UI
{
    /// <summary>
    /// UI row for displaying a single leaderboard entry.
    /// </summary>
    public class LeaderboardRow : MonoBehaviour
    {
        [SerializeField] private TMP_Text rankText;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text eloText;

        /// <summary>
        /// Populates this row with the given data.
        /// </summary>
        /// <param name="rank">1-based rank of the player.</param>
        /// <param name="nickname">Player's display name.</param>
        /// <param name="elo">Player's ELO rating.</param>
        public void Bind(int rank, string nickname, int elo)
        {
            if (rankText) rankText.text = rank.ToString();
            if (nameText) nameText.text = nickname;
            if (eloText)  eloText.text  = elo.ToString();
        }
    }
}
