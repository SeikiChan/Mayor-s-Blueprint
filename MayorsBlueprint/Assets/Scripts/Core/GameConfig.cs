using UnityEngine;

namespace MayorsBlueprint.Core
{
    /// <summary>
    /// Global run configuration. Tweak values per difficulty or map.
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Mayor's Blueprint/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Run Structure")]
        [Tooltip("Total days in a single run.")]
        public int totalDays = 10;

        [Tooltip("Money the player starts with on Day 1.")]
        public int startingMoney = 10;

        [Tooltip("Score the player must reach by the final day to win.")]
        public int targetScore = 100;

        [Header("Board")]
        public int boardWidth = 8;
        public int boardHeight = 8;

        [Header("Reserve")]
        [Tooltip("Maximum number of buildings the player can hold in reserve.")]
        public int reserveSlots = 5;

        [Header("Economy")]
        [Tooltip("Base income the player receives at the end of each day regardless of buildings.")]
        public int baseIncomePerDay = 3;

        [Tooltip("Money received when selling a building from reserve.")]
        public int sellValue = 1;
    }
}
