using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MayorsBlueprint.Buildings;
using MayorsBlueprint.Core;
using MayorsBlueprint.Events;
using MayorsBlueprint.Pack;
using MayorsBlueprint.Reserve;

namespace MayorsBlueprint.UI
{
    /// <summary>
    /// UI panel for the pack shop. Shows available packs and direct-purchase buildings.
    /// Active only during the Shopping phase.
    /// </summary>
    public class ShopUI : MonoBehaviour
    {
        [SerializeField] private GameObject shopPanel;
        [SerializeField] private Transform packContainer;
        [SerializeField] private GameObject packButtonPrefab;

        private ShopManager shopManager;
        private ReserveManager reserveManager;

        private void Start()
        {
            shopManager = FindAnyObjectByType<ShopManager>();
            reserveManager = FindAnyObjectByType<ReserveManager>();
        }

        private void OnEnable()
        {
            GameEvents.OnPhaseChanged += HandlePhaseChanged;
            GameEvents.OnDayStarted += HandleDayStarted;
        }

        private void OnDisable()
        {
            GameEvents.OnPhaseChanged -= HandlePhaseChanged;
            GameEvents.OnDayStarted -= HandleDayStarted;
        }

        private void HandlePhaseChanged(DayPhase phase)
        {
            if (shopPanel != null)
                shopPanel.SetActive(phase == DayPhase.Shopping);
        }

        private void HandleDayStarted(int day)
        {
            shopManager?.RefreshDailyOffering();
            RefreshPackDisplay();
        }

        private void RefreshPackDisplay()
        {
            if (packContainer == null || packButtonPrefab == null || shopManager == null)
                return;

            // Clear existing buttons
            foreach (Transform child in packContainer)
                Destroy(child.gameObject);

            // Create buttons for each available pack
            foreach (var pack in shopManager.DailyPacks)
            {
                var buttonObj = Instantiate(packButtonPrefab, packContainer);

                var label = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                if (label != null)
                    label.text = $"{pack.displayName}\n${pack.cost}";

                var button = buttonObj.GetComponent<Button>();
                if (button != null)
                {
                    var capturedPack = pack;
                    button.onClick.AddListener(() => OnPackClicked(capturedPack));
                }
            }
        }

        private void OnPackClicked(PackData pack)
        {
            var buildings = shopManager.BuyPack(pack);
            if (buildings == null) return; // Can't afford

            // Add opened buildings to reserve
            foreach (var building in buildings)
            {
                reserveManager.AddBuilding(building);
            }
        }
    }
}
