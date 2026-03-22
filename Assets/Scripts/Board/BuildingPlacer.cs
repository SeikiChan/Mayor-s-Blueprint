using UnityEngine;
using MayorsBlueprint.Buildings;
using MayorsBlueprint.Events;

namespace MayorsBlueprint.Board
{
    /// <summary>
    /// Handles player interaction for placing buildings on the board.
    /// Manages preview, rotation, and confirm/cancel flow.
    /// </summary>
    public class BuildingPlacer : MonoBehaviour
    {
        [SerializeField] private GridBoard board;
        [SerializeField] private Material previewValidMaterial;
        [SerializeField] private Material previewInvalidMaterial;

        private BuildingData currentBuilding;
        private int currentRotation;
        private Vector2Int currentAnchor;
        private GameObject previewObject;
        private bool isPlacing;

        public bool IsPlacing => isPlacing;

        // ──────────────────────────────────────────────
        // Public API
        // ──────────────────────────────────────────────

        /// <summary>
        /// Begin placing a building from the reserve.
        /// </summary>
        public void BeginPlacement(BuildingData data)
        {
            if (isPlacing) CancelPlacement();

            currentBuilding = data;
            currentRotation = 0;
            isPlacing = true;

            CreatePreview();
        }

        /// <summary>
        /// Rotate the current preview 90 degrees clockwise.
        /// </summary>
        public void RotatePreview()
        {
            if (!isPlacing) return;
            currentRotation = (currentRotation + 1) % 4;
            UpdatePreview();
        }

        /// <summary>
        /// Try to confirm placement at the current anchor position.
        /// Returns the placed building or null if invalid.
        /// </summary>
        public PlacedBuilding ConfirmPlacement()
        {
            if (!isPlacing) return null;
            if (!board.CanPlace(currentBuilding, currentAnchor, currentRotation))
                return null;

            var placed = board.Place(currentBuilding, currentAnchor, currentRotation);
            CleanupPreview();
            isPlacing = false;
            currentBuilding = null;
            return placed;
        }

        /// <summary>
        /// Cancel the current placement and return to reserve.
        /// </summary>
        public void CancelPlacement()
        {
            CleanupPreview();
            isPlacing = false;
            currentBuilding = null;
        }

        /// <summary>
        /// Update the target anchor position (called from input/mouse system).
        /// </summary>
        public void SetTargetPosition(Vector2Int boardPosition)
        {
            if (!isPlacing) return;
            currentAnchor = boardPosition;
            UpdatePreview();
        }

        // ──────────────────────────────────────────────
        // Preview management
        // ──────────────────────────────────────────────

        private void CreatePreview()
        {
            if (currentBuilding.prefab != null)
            {
                previewObject = Instantiate(currentBuilding.prefab);
                previewObject.name = $"Preview_{currentBuilding.displayName}";
            }
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            if (previewObject == null) return;

            bool valid = board.CanPlace(currentBuilding, currentAnchor, currentRotation);

            // Position the preview at the anchor cell world position
            previewObject.transform.position = BoardToWorldPosition(currentAnchor);
            previewObject.transform.rotation = Quaternion.Euler(0, 90f * currentRotation, 0);

            // Update preview material to show valid/invalid
            var renderers = previewObject.GetComponentsInChildren<Renderer>();
            var mat = valid ? previewValidMaterial : previewInvalidMaterial;
            if (mat != null)
            {
                foreach (var r in renderers)
                    r.material = mat;
            }
        }

        private void CleanupPreview()
        {
            if (previewObject != null)
            {
                Destroy(previewObject);
                previewObject = null;
            }
        }

        /// <summary>
        /// Convert a board grid position to world space.
        /// Override this for isometric or custom layouts.
        /// </summary>
        public virtual Vector3 BoardToWorldPosition(Vector2Int boardPos)
        {
            return new Vector3(boardPos.x, 0, boardPos.y);
        }
    }
}
