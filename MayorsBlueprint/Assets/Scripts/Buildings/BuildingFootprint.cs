using System;
using UnityEngine;

namespace MayorsBlueprint.Buildings
{
    /// <summary>
    /// Defines a polyomino footprint as a set of cell offsets from the anchor (0,0).
    /// Serializable so it can be embedded in ScriptableObjects.
    /// </summary>
    [Serializable]
    public class BuildingFootprint
    {
        [Tooltip("Cell offsets relative to the anchor cell (0,0). Include (0,0) itself.")]
        public Vector2Int[] cells = { Vector2Int.zero };

        /// <summary>Number of cells this footprint occupies.</summary>
        public int CellCount => cells.Length;

        /// <summary>
        /// Returns the footprint rotated 90 degrees clockwise the given number of times.
        /// </summary>
        public Vector2Int[] GetRotated(int quarterTurns)
        {
            quarterTurns = ((quarterTurns % 4) + 4) % 4;
            var rotated = new Vector2Int[cells.Length];
            for (int i = 0; i < cells.Length; i++)
            {
                var c = cells[i];
                for (int r = 0; r < quarterTurns; r++)
                {
                    c = new Vector2Int(c.y, -c.x);
                }
                rotated[i] = c;
            }
            return rotated;
        }

        /// <summary>
        /// Returns the world-space cell positions when placed at the given board anchor
        /// with the given rotation.
        /// </summary>
        public Vector2Int[] GetCellsAt(Vector2Int anchor, int rotation)
        {
            var rotated = GetRotated(rotation);
            var result = new Vector2Int[rotated.Length];
            for (int i = 0; i < rotated.Length; i++)
            {
                result[i] = anchor + rotated[i];
            }
            return result;
        }

        // ── Preset factory methods ──

        public static BuildingFootprint Single()
        {
            return new BuildingFootprint { cells = new[] { Vector2Int.zero } };
        }

        public static BuildingFootprint Line2()
        {
            return new BuildingFootprint
            {
                cells = new[] { Vector2Int.zero, new Vector2Int(1, 0) }
            };
        }

        public static BuildingFootprint LShape()
        {
            return new BuildingFootprint
            {
                cells = new[]
                {
                    Vector2Int.zero,
                    new Vector2Int(1, 0),
                    new Vector2Int(0, 1)
                }
            };
        }

        public static BuildingFootprint Block2x2()
        {
            return new BuildingFootprint
            {
                cells = new[]
                {
                    Vector2Int.zero,
                    new Vector2Int(1, 0),
                    new Vector2Int(0, 1),
                    new Vector2Int(1, 1)
                }
            };
        }

        public static BuildingFootprint TShape()
        {
            return new BuildingFootprint
            {
                cells = new[]
                {
                    Vector2Int.zero,
                    new Vector2Int(-1, 0),
                    new Vector2Int(1, 0),
                    new Vector2Int(0, 1)
                }
            };
        }
    }
}
