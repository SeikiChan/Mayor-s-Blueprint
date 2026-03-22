using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MayorsBlueprint.Core;
using MayorsBlueprint.Events;

namespace MayorsBlueprint.UI
{
    /// <summary>
    /// Main HUD showing day counter, money, score, current phase, and target score.
    /// </summary>
    public class HUDManager : MonoBehaviour
    {
        [Header("Text Elements")]
        [SerializeField] private TextMeshProUGUI dayText;
        [SerializeField] private TextMeshProUGUI moneyText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI phaseText;
        [SerializeField] private TextMeshProUGUI targetScoreText;

        [Header("Buttons")]
        [SerializeField] private Button endDayButton;
        [SerializeField] private Button finishShoppingButton;
        [SerializeField] private Button finishPlacementButton;

        private TurnManager turnManager;

        private void Start()
        {
            turnManager = FindAnyObjectByType<TurnManager>();

            if (GameManager.Instance != null && targetScoreText != null)
                targetScoreText.text = $"Target: {GameManager.Instance.Config.targetScore}";

            // Wire buttons
            if (endDayButton != null)
                endDayButton.onClick.AddListener(OnEndDayClicked);
            if (finishShoppingButton != null)
                finishShoppingButton.onClick.AddListener(OnFinishShoppingClicked);
            if (finishPlacementButton != null)
                finishPlacementButton.onClick.AddListener(OnFinishPlacementClicked);
        }

        private void OnEnable()
        {
            GameEvents.OnDayStarted += UpdateDay;
            GameEvents.OnMoneyChanged += UpdateMoney;
            GameEvents.OnScoreChanged += UpdateScore;
            GameEvents.OnPhaseChanged += UpdatePhase;
        }

        private void OnDisable()
        {
            GameEvents.OnDayStarted -= UpdateDay;
            GameEvents.OnMoneyChanged -= UpdateMoney;
            GameEvents.OnScoreChanged -= UpdateScore;
            GameEvents.OnPhaseChanged -= UpdatePhase;
        }

        private void UpdateDay(int day)
        {
            if (dayText != null)
                dayText.text = $"Day {day} / {(turnManager != null ? turnManager.ToString() : "?")}";
        }

        private void UpdateMoney(int amount)
        {
            if (moneyText != null)
                moneyText.text = $"${amount}";
        }

        private void UpdateScore(int score)
        {
            if (scoreText != null)
                scoreText.text = $"Score: {score}";
        }

        private void UpdatePhase(DayPhase phase)
        {
            if (phaseText != null)
                phaseText.text = phase.ToString();

            // Toggle button visibility based on phase
            if (finishShoppingButton != null)
                finishShoppingButton.gameObject.SetActive(phase == DayPhase.Shopping);
            if (finishPlacementButton != null)
                finishPlacementButton.gameObject.SetActive(phase == DayPhase.Placement);
            if (endDayButton != null)
                endDayButton.gameObject.SetActive(phase == DayPhase.Management);
        }

        private void OnEndDayClicked()
        {
            turnManager?.FinishManagement();
        }

        private void OnFinishShoppingClicked()
        {
            turnManager?.FinishShopping();
        }

        private void OnFinishPlacementClicked()
        {
            turnManager?.FinishPlacement();
        }
    }
}
