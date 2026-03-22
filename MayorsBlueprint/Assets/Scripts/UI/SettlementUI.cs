using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MayorsBlueprint.Events;
using MayorsBlueprint.Synergy;

namespace MayorsBlueprint.UI
{
    /// <summary>
    /// End-of-day settlement summary showing score and income gained.
    /// </summary>
    public class SettlementUI : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI scoreGainedText;
        [SerializeField] private TextMeshProUGUI incomeGainedText;
        [SerializeField] private Transform synergyListContainer;
        [SerializeField] private GameObject synergyEntryPrefab;
        [SerializeField] private Button continueButton;

        private void Start()
        {
            if (panel != null) panel.SetActive(false);

            if (continueButton != null)
                continueButton.onClick.AddListener(OnContinueClicked);
        }

        private void OnEnable()
        {
            GameEvents.OnSettlementResolved += ShowSettlement;
        }

        private void OnDisable()
        {
            GameEvents.OnSettlementResolved -= ShowSettlement;
        }

        private void ShowSettlement(int scoreGained, int incomeGained)
        {
            if (panel != null) panel.SetActive(true);

            if (scoreGainedText != null)
                scoreGainedText.text = $"+{scoreGained} Score";

            if (incomeGainedText != null)
                incomeGainedText.text = $"+${incomeGained} Income";
        }

        private void OnContinueClicked()
        {
            if (panel != null) panel.SetActive(false);

            var turnManager = FindAnyObjectByType<Core.TurnManager>();
            turnManager?.FinishSettlement();
        }
    }
}
