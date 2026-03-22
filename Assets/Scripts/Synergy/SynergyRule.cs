using UnityEngine;
using MayorsBlueprint.Core;

namespace MayorsBlueprint.Synergy
{
    /// <summary>
    /// Defines a single synergy rule between building categories.
    /// </summary>
    [CreateAssetMenu(fileName = "NewSynergyRule", menuName = "Mayor's Blueprint/Synergy Rule")]
    public class SynergyRule : ScriptableObject
    {
        public string ruleId;
        [TextArea(2, 3)]
        public string description;

        [Header("Conditions")]
        public BuildingCategory sourceCategory;
        public BuildingCategory targetCategory;
        public SynergyCheckType checkType = SynergyCheckType.Adjacent;

        [Header("Effects")]
        public SynergyBonusType bonusType = SynergyBonusType.Score;
        public int scoreBonus;
        public int incomeBonus;

        [Header("Negative Synergy")]
        [Tooltip("If true, this is a penalty (e.g. factory next to housing).")]
        public bool isNegative;
    }

    /// <summary>
    /// Result of evaluating a synergy for a placed building.
    /// </summary>
    public struct SynergyResult
    {
        public SynergyRule rule;
        public Vector2Int sourceCell;
        public Vector2Int targetCell;
        public int scoreModifier;
        public int incomeModifier;
    }
}
