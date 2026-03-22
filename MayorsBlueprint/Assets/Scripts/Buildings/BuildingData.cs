using UnityEngine;
using MayorsBlueprint.Core;

namespace MayorsBlueprint.Buildings
{
    /// <summary>
    /// Immutable definition of a building type. Create instances via Assets > Create > Mayor's Blueprint.
    /// </summary>
    [CreateAssetMenu(fileName = "NewBuilding", menuName = "Mayor's Blueprint/Building Data")]
    public class BuildingData : ScriptableObject
    {
        [Header("Identity")]
        public string buildingId;
        public string displayName;
        [TextArea(2, 4)]
        public string description;
        public BuildingCategory category;
        public BuildingRarity rarity;

        [Header("Footprint")]
        public BuildingFootprint footprint;

        [Header("Stats")]
        [Tooltip("Score points this building contributes each end-of-day settlement.")]
        public int baseScore;
        [Tooltip("Income this building generates each end-of-day settlement.")]
        public int baseIncome;
        [Tooltip("Cost if purchased directly from the shop (0 = not directly purchasable).")]
        public int directPurchaseCost;

        [Header("Upgrade")]
        [Tooltip("The building this upgrades into when duplicates are merged. Null if not upgradable.")]
        public BuildingData upgradedVersion;
        [Tooltip("Number of duplicates needed to upgrade.")]
        public int duplicatesRequiredForUpgrade = 2;

        [Header("Visuals")]
        public Sprite icon;
        public GameObject prefab;
    }
}
