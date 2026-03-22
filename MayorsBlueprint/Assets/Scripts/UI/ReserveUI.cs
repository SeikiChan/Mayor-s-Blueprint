using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MayorsBlueprint.Buildings;
using MayorsBlueprint.Board;
using MayorsBlueprint.Core;
using MayorsBlueprint.Events;
using MayorsBlueprint.Reserve;

namespace MayorsBlueprint.UI
{
    /// <summary>
    /// UI bar showing buildings currently held in reserve.
    /// Clicking a slot initiates placement on the board.
    /// </summary>
    public class ReserveUI : MonoBehaviour
    {
        [SerializeField] private Transform slotContainer;
        [SerializeField] private GameObject slotPrefab;

        private ReserveManager reserveManager;
        private BuildingPlacer buildingPlacer;
        private GameObject[] slotObjects;

        private void Start()
        {
            reserveManager = FindAnyObjectByType<ReserveManager>();
            buildingPlacer = FindAnyObjectByType<BuildingPlacer>();
        }

        private void OnEnable()
        {
            GameEvents.OnBuildingAddedToReserve += HandleBuildingAdded;
            GameEvents.OnBuildingRemovedFromReserve += HandleBuildingRemoved;
        }

        private void OnDisable()
        {
            GameEvents.OnBuildingAddedToReserve -= HandleBuildingAdded;
            GameEvents.OnBuildingRemovedFromReserve -= HandleBuildingRemoved;
        }

        /// <summary>
        /// Create slot UI elements matching the reserve capacity.
        /// Call after ReserveManager is initialized.
        /// </summary>
        public void SetupSlots(int count)
        {
            if (slotContainer == null || slotPrefab == null) return;

            slotObjects = new GameObject[count];
            for (int i = 0; i < count; i++)
            {
                var slot = Instantiate(slotPrefab, slotContainer);
                slot.name = $"ReserveSlot_{i}";
                slotObjects[i] = slot;

                int capturedIndex = i;
                var button = slot.GetComponent<Button>();
                if (button != null)
                    button.onClick.AddListener(() => OnSlotClicked(capturedIndex));

                UpdateSlotVisual(i, null);
            }
        }

        private void HandleBuildingAdded(BuildingData data, int slotIndex)
        {
            UpdateSlotVisual(slotIndex, data);
        }

        private void HandleBuildingRemoved(int slotIndex)
        {
            UpdateSlotVisual(slotIndex, null);
        }

        private void UpdateSlotVisual(int index, BuildingData data)
        {
            if (slotObjects == null || index >= slotObjects.Length) return;

            var slot = slotObjects[index];
            var icon = slot.GetComponentInChildren<Image>();
            var label = slot.GetComponentInChildren<TextMeshProUGUI>();

            if (data != null)
            {
                if (icon != null && data.icon != null)
                {
                    icon.sprite = data.icon;
                    icon.enabled = true;
                }
                if (label != null)
                    label.text = data.displayName;
            }
            else
            {
                if (icon != null) icon.enabled = false;
                if (label != null) label.text = "Empty";
            }
        }

        private void OnSlotClicked(int slotIndex)
        {
            if (reserveManager == null || buildingPlacer == null) return;

            var building = reserveManager.GetSlot(slotIndex);
            if (building == null) return;

            // Take from reserve and begin placement
            var taken = reserveManager.TakeBuilding(slotIndex);
            if (taken != null)
            {
                buildingPlacer.BeginPlacement(taken);
            }
        }
    }
}
