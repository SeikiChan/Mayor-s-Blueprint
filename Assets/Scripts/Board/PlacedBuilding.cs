using UnityEngine;
using MayorsBlueprint.Buildings;

namespace MayorsBlueprint.Board
{
    /// <summary>
    /// Runtime representation of a building placed on the board.
    /// </summary>
    public class PlacedBuilding
    {
        public BuildingData Data { get; }

        /// <summary>The anchor cell where the building was placed.</summary>
        public Vector2Int Anchor { get; }

        /// <summary>Rotation in quarter turns (0-3).</summary>
        public int Rotation { get; }

        /// <summary>All board cells this building occupies.</summary>
        public Vector2Int[] OccupiedCells { get; }

        /// <summary>Current upgrade level (0 = base).</summary>
        public int UpgradeLevel { get; set; }

        /// <summary>The scene GameObject representing this building.</summary>
        public GameObject Visual { get; set; }

        public PlacedBuilding(BuildingData data, Vector2Int anchor, int rotation)
        {
            Data = data;
            Anchor = anchor;
            Rotation = rotation;
            OccupiedCells = data.footprint.GetCellsAt(anchor, rotation);
        }

        public int GetEffectiveScore()
        {
            return Data.baseScore + (UpgradeLevel * Mathf.CeilToInt(Data.baseScore * 0.5f));
        }

        public int GetEffectiveIncome()
        {
            return Data.baseIncome + (UpgradeLevel * Mathf.CeilToInt(Data.baseIncome * 0.5f));
        }
    }
}
