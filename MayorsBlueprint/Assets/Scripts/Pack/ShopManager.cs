using System.Collections.Generic;
using UnityEngine;
using MayorsBlueprint.Buildings;
using MayorsBlueprint.Core;
using MayorsBlueprint.Events;

namespace MayorsBlueprint.Pack
{
    /// <summary>
    /// Manages the shop where players buy building packs each day.
    /// </summary>
    public class ShopManager : MonoBehaviour
    {
        [SerializeField] private PackData[] availablePacks;
        [SerializeField] private BuildingData[] directPurchaseBuildings;

        private ResourceManager resourceManager;
        private readonly List<PackData> dailyPackOffering = new();
        private readonly List<BuildingData> dailyDirectOffering = new();

        public IReadOnlyList<PackData> DailyPacks => dailyPackOffering;
        public IReadOnlyList<BuildingData> DirectBuildings => dailyDirectOffering;

        public void Initialize()
        {
            resourceManager = FindAnyObjectByType<ResourceManager>();
            RefreshDailyOffering();
        }

        /// <summary>
        /// Refresh the shop offering. Called at the start of each day.
        /// For MVP, just show all available packs.
        /// </summary>
        public void RefreshDailyOffering()
        {
            dailyPackOffering.Clear();
            dailyDirectOffering.Clear();

            if (availablePacks != null)
            {
                foreach (var pack in availablePacks)
                    dailyPackOffering.Add(pack);
            }

            if (directPurchaseBuildings != null)
            {
                foreach (var building in directPurchaseBuildings)
                {
                    if (building.directPurchaseCost > 0)
                        dailyDirectOffering.Add(building);
                }
            }
        }

        /// <summary>
        /// Attempt to buy a pack. Returns the opened buildings, or null if can't afford.
        /// </summary>
        public BuildingData[] BuyPack(PackData pack)
        {
            if (!resourceManager.CanAfford(pack.cost))
                return null;

            resourceManager.SpendMoney(pack.cost);
            GameEvents.FirePackPurchased(pack);

            var buildings = pack.Open();
            GameEvents.FirePackOpened(buildings);
            return buildings;
        }

        /// <summary>
        /// Attempt to directly purchase a single building.
        /// </summary>
        public BuildingData BuyBuilding(BuildingData building)
        {
            if (building.directPurchaseCost <= 0) return null;
            if (!resourceManager.CanAfford(building.directPurchaseCost)) return null;

            resourceManager.SpendMoney(building.directPurchaseCost);
            return building;
        }
    }
}
