using System.Collections.Generic;
using UnityEngine;
using MayorsBlueprint.Buildings;
using MayorsBlueprint.Events;

namespace MayorsBlueprint.Board
{
    /// <summary>
    /// The game board: a fixed-size grid that holds placed buildings.
    /// Handles placement validation and neighbor queries for synergy evaluation.
    /// </summary>
    public class GridBoard : MonoBehaviour
    {
        [SerializeField] private int width = 8;
        [SerializeField] private int height = 8;

        private GridCell[,] cells;
        private readonly List<PlacedBuilding> placedBuildings = new();

        // ── Directions for adjacency checks (4-directional) ──
        private static readonly Vector2Int[] AdjacentOffsets =
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        public int Width => width;
        public int Height => height;
        public IReadOnlyList<PlacedBuilding> PlacedBuildings => placedBuildings;

        // ──────────────────────────────────────────────
        // Initialization
        // ──────────────────────────────────────────────

        public void Initialize(int w, int h)
        {
            width = w;
            height = h;
            cells = new GridCell[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    cells[x, y] = new GridCell(new Vector2Int(x, y));

            placedBuildings.Clear();
        }

        // ──────────────────────────────────────────────
        // Queries
        // ──────────────────────────────────────────────

        public bool IsInBounds(Vector2Int pos)
        {
            return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
        }

        public GridCell GetCell(Vector2Int pos)
        {
            if (!IsInBounds(pos)) return null;
            return cells[pos.x, pos.y];
        }

        /// <summary>
        /// Check whether a building with the given footprint can be placed at anchor with rotation.
        /// </summary>
        public bool CanPlace(BuildingData data, Vector2Int anchor, int rotation)
        {
            var occupiedCells = data.footprint.GetCellsAt(anchor, rotation);
            foreach (var cell in occupiedCells)
            {
                if (!IsInBounds(cell)) return false;
                if (cells[cell.x, cell.y].IsOccupied) return false;
            }
            return true;
        }

        // ──────────────────────────────────────────────
        // Placement
        // ──────────────────────────────────────────────

        /// <summary>
        /// Place a building on the board. Returns null if placement is invalid.
        /// </summary>
        public PlacedBuilding Place(BuildingData data, Vector2Int anchor, int rotation)
        {
            if (!CanPlace(data, anchor, rotation))
                return null;

            var placed = new PlacedBuilding(data, anchor, rotation);
            foreach (var cell in placed.OccupiedCells)
            {
                cells[cell.x, cell.y].SetBuilding(placed);
            }
            placedBuildings.Add(placed);

            GameEvents.FireBuildingPlaced(placed);
            return placed;
        }

        /// <summary>
        /// Remove a building from the board.
        /// </summary>
        public bool Remove(PlacedBuilding building)
        {
            if (!placedBuildings.Contains(building))
                return false;

            foreach (var cell in building.OccupiedCells)
            {
                if (IsInBounds(cell))
                    cells[cell.x, cell.y].ClearBuilding();
            }
            placedBuildings.Remove(building);

            if (building.Visual != null)
                Destroy(building.Visual);

            GameEvents.FireBuildingRemoved(building);
            return true;
        }

        // ──────────────────────────────────────────────
        // Neighbor queries (for synergy)
        // ──────────────────────────────────────────────

        /// <summary>
        /// Returns all distinct buildings adjacent to the given building.
        /// </summary>
        public List<PlacedBuilding> GetAdjacentBuildings(PlacedBuilding building)
        {
            var neighbors = new HashSet<PlacedBuilding>();
            foreach (var cell in building.OccupiedCells)
            {
                foreach (var offset in AdjacentOffsets)
                {
                    var neighborPos = cell + offset;
                    var neighborCell = GetCell(neighborPos);
                    if (neighborCell != null && neighborCell.IsOccupied
                        && neighborCell.OccupyingBuilding != building)
                    {
                        neighbors.Add(neighborCell.OccupyingBuilding);
                    }
                }
            }
            return new List<PlacedBuilding>(neighbors);
        }

        /// <summary>
        /// Returns all buildings of a given category currently on the board.
        /// </summary>
        public List<PlacedBuilding> GetBuildingsByCategory(Core.BuildingCategory category)
        {
            var result = new List<PlacedBuilding>();
            foreach (var b in placedBuildings)
            {
                if (b.Data.category == category)
                    result.Add(b);
            }
            return result;
        }

        /// <summary>
        /// Resets the entire board (for new run).
        /// </summary>
        public void Clear()
        {
            foreach (var b in placedBuildings)
            {
                if (b.Visual != null) Destroy(b.Visual);
            }
            placedBuildings.Clear();

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    cells[x, y].ClearBuilding();
        }
    }
}
