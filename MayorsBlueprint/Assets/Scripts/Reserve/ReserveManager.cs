using System;
using UnityEngine;
using MayorsBlueprint.Buildings;
using MayorsBlueprint.Core;
using MayorsBlueprint.Events;

namespace MayorsBlueprint.Reserve
{
    /// <summary>
    /// Limited storage bar where opened buildings wait before being placed.
    /// Creates pressure: limited slots force the player to make placement decisions.
    /// </summary>
    public class ReserveManager : MonoBehaviour
    {
        private BuildingData[] slots;
        private int maxSlots;

        public int MaxSlots => maxSlots;
        public int OccupiedSlots { get; private set; }
        public bool IsFull => OccupiedSlots >= maxSlots;

        public void Initialize(int slotCount)
        {
            maxSlots = slotCount;
            slots = new BuildingData[maxSlots];
            OccupiedSlots = 0;
        }

        /// <summary>Get the building in a specific slot (null if empty).</summary>
        public BuildingData GetSlot(int index)
        {
            if (index < 0 || index >= maxSlots) return null;
            return slots[index];
        }

        /// <summary>
        /// Add a building to the first available slot.
        /// Returns the slot index, or -1 if the reserve is full.
        /// </summary>
        public int AddBuilding(BuildingData building)
        {
            if (IsFull)
            {
                GameEvents.FireReserveFull();
                return -1;
            }

            for (int i = 0; i < maxSlots; i++)
            {
                if (slots[i] == null)
                {
                    slots[i] = building;
                    OccupiedSlots++;
                    GameEvents.FireBuildingAddedToReserve(building, i);
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Remove a building from a specific slot and return it.
        /// </summary>
        public BuildingData TakeBuilding(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= maxSlots) return null;
            var building = slots[slotIndex];
            if (building == null) return null;

            slots[slotIndex] = null;
            OccupiedSlots--;
            GameEvents.FireBuildingRemovedFromReserve(slotIndex);
            return building;
        }

        /// <summary>
        /// Sell a building from reserve for money.
        /// </summary>
        public bool SellBuilding(int slotIndex, ResourceManager resources, int sellValue)
        {
            var building = TakeBuilding(slotIndex);
            if (building == null) return false;

            resources.AddMoney(sellValue);
            return true;
        }

        /// <summary>
        /// Check if any two slots contain the same building type for upgrading.
        /// Returns the indices of the first matching pair, or (-1, -1) if none found.
        /// </summary>
        public (int, int) FindUpgradePair()
        {
            for (int i = 0; i < maxSlots; i++)
            {
                if (slots[i] == null || slots[i].upgradedVersion == null) continue;

                int count = 1;
                int secondIndex = -1;
                for (int j = i + 1; j < maxSlots; j++)
                {
                    if (slots[j] != null && slots[j].buildingId == slots[i].buildingId)
                    {
                        count++;
                        secondIndex = j;
                        if (count >= slots[i].duplicatesRequiredForUpgrade)
                            return (i, secondIndex);
                    }
                }
            }
            return (-1, -1);
        }

        /// <summary>
        /// Merge duplicates at the given indices into an upgraded building.
        /// </summary>
        public BuildingData UpgradeBuilding(int slotA, int slotB)
        {
            var buildingA = GetSlot(slotA);
            if (buildingA == null || buildingA.upgradedVersion == null) return null;

            var buildingB = GetSlot(slotB);
            if (buildingB == null || buildingA.buildingId != buildingB.buildingId) return null;

            // Remove both
            TakeBuilding(slotA);
            TakeBuilding(slotB);

            // Add upgraded version
            var upgraded = buildingA.upgradedVersion;
            AddBuilding(upgraded);
            return upgraded;
        }

        /// <summary>Clear all slots (for new run).</summary>
        public void Clear()
        {
            for (int i = 0; i < maxSlots; i++)
                slots[i] = null;
            OccupiedSlots = 0;
        }
    }
}
