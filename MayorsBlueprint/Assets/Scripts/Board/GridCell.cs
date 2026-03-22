using UnityEngine;

namespace MayorsBlueprint.Board
{
    /// <summary>
    /// Represents a single cell on the game board.
    /// </summary>
    public class GridCell
    {
        public Vector2Int Position { get; }
        public bool IsOccupied => OccupyingBuilding != null;
        public PlacedBuilding OccupyingBuilding { get; private set; }

        public GridCell(Vector2Int position)
        {
            Position = position;
        }

        public void SetBuilding(PlacedBuilding building)
        {
            OccupyingBuilding = building;
        }

        public void ClearBuilding()
        {
            OccupyingBuilding = null;
        }
    }
}
