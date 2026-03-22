using UnityEngine;
using MayorsBlueprint.Events;

namespace MayorsBlueprint.Core
{
    /// <summary>
    /// Tracks Money (process resource) and Score (victory resource).
    /// </summary>
    public class ResourceManager : MonoBehaviour
    {
        private int money;
        private int score;

        public int Money => money;
        public int Score => score;

        public void Initialize(int startingMoney)
        {
            money = startingMoney;
            score = 0;
            GameEvents.FireMoneyChanged(money);
            GameEvents.FireScoreChanged(score);
        }

        // ── Money ──

        public bool CanAfford(int cost) => money >= cost;

        public bool SpendMoney(int amount)
        {
            if (amount < 0 || money < amount) return false;
            money -= amount;
            GameEvents.FireMoneyChanged(money);
            return true;
        }

        public void AddMoney(int amount)
        {
            if (amount <= 0) return;
            money += amount;
            GameEvents.FireMoneyChanged(money);
        }

        // ── Score ──

        public void AddScore(int amount)
        {
            if (amount == 0) return;
            score += amount;
            GameEvents.FireScoreChanged(score);
        }

        /// <summary>Check whether the player has reached the target score.</summary>
        public bool HasReachedTarget(int targetScore) => score >= targetScore;
    }
}
