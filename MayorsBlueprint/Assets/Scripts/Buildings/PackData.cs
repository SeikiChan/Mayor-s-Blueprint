using System;
using UnityEngine;

namespace MayorsBlueprint.Buildings
{
    /// <summary>
    /// Defines a building pack available in the shop.
    /// </summary>
    [CreateAssetMenu(fileName = "NewPack", menuName = "Mayor's Blueprint/Pack Data")]
    public class PackData : ScriptableObject
    {
        public string packId;
        public string displayName;
        [TextArea(2, 3)]
        public string description;
        public Sprite icon;

        [Header("Cost")]
        public int cost;

        [Header("Contents")]
        [Tooltip("How many buildings the player draws when opening this pack.")]
        public int buildingsPerPack = 3;

        [Tooltip("Weighted pool of possible buildings in this pack.")]
        public PackEntry[] possibleBuildings;

        /// <summary>
        /// A weighted entry in the pack pool.
        /// </summary>
        [Serializable]
        public struct PackEntry
        {
            public BuildingData building;
            [Range(1, 100)]
            public int weight;
        }

        /// <summary>
        /// Roll buildings from this pack using weighted random selection.
        /// </summary>
        public BuildingData[] Open()
        {
            if (possibleBuildings == null || possibleBuildings.Length == 0)
                return Array.Empty<BuildingData>();

            int totalWeight = 0;
            foreach (var entry in possibleBuildings)
                totalWeight += entry.weight;

            var results = new BuildingData[buildingsPerPack];
            for (int i = 0; i < buildingsPerPack; i++)
            {
                int roll = UnityEngine.Random.Range(0, totalWeight);
                int cumulative = 0;
                foreach (var entry in possibleBuildings)
                {
                    cumulative += entry.weight;
                    if (roll < cumulative)
                    {
                        results[i] = entry.building;
                        break;
                    }
                }
            }
            return results;
        }
    }
}
