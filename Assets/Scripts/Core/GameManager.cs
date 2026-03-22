using UnityEngine;
using MayorsBlueprint.Board;
using MayorsBlueprint.Events;
using MayorsBlueprint.Synergy;

namespace MayorsBlueprint.Core
{
    /// <summary>
    /// Top-level orchestrator. Initializes all systems, manages run lifecycle,
    /// and resolves end-of-day settlement and win/lose conditions.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private GameConfig config;

        [Header("System References")]
        [SerializeField] private TurnManager turnManager;
        [SerializeField] private ResourceManager resourceManager;
        [SerializeField] private GridBoard board;
        [SerializeField] private SynergyEvaluator synergyEvaluator;
        [SerializeField] private Pack.ShopManager shopManager;
        [SerializeField] private Reserve.ReserveManager reserveManager;

        private RunState runState = RunState.NotStarted;

        public RunState RunState => runState;
        public GameConfig Config => config;

        // ── Singleton access (optional, for convenience) ──
        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnEnable()
        {
            GameEvents.OnPhaseChanged += HandlePhaseChanged;
        }

        private void OnDisable()
        {
            GameEvents.OnPhaseChanged -= HandlePhaseChanged;
        }

        // ──────────────────────────────────────────────
        // Run lifecycle
        // ──────────────────────────────────────────────

        /// <summary>Start a new run with the current config.</summary>
        public void StartRun()
        {
            runState = RunState.Running;

            // Initialize all systems
            board.Initialize(config.boardWidth, config.boardHeight);
            resourceManager.Initialize(config.startingMoney);
            turnManager.Initialize(config.totalDays);
            reserveManager.Initialize(config.reserveSlots);
            shopManager.Initialize();

            GameEvents.FireRunStarted();
            turnManager.StartNewDay();
        }

        /// <summary>End the current run and evaluate win/lose.</summary>
        public void EndRun()
        {
            bool won = resourceManager.HasReachedTarget(config.targetScore);
            runState = won ? RunState.Won : RunState.Lost;
            GameEvents.FireRunEnded(won);
        }

        /// <summary>Reset everything for a fresh run.</summary>
        public void RestartRun()
        {
            board.Clear();
            runState = RunState.NotStarted;
            StartRun();
        }

        // ──────────────────────────────────────────────
        // Settlement
        // ──────────────────────────────────────────────

        private void HandlePhaseChanged(DayPhase phase)
        {
            if (phase == DayPhase.Settlement)
            {
                ResolveSettlement();
            }
        }

        private void ResolveSettlement()
        {
            int totalScoreGained = 0;
            int totalIncomeGained = config.baseIncomePerDay;

            // 1. Collect base score and income from all placed buildings
            foreach (var building in board.PlacedBuildings)
            {
                totalScoreGained += building.GetEffectiveScore();
                totalIncomeGained += building.GetEffectiveIncome();
            }

            // 2. Evaluate all synergies
            var synergyResults = synergyEvaluator.EvaluateAll(board);
            foreach (var result in synergyResults)
            {
                totalScoreGained += result.scoreModifier;
                totalIncomeGained += result.incomeModifier;
                GameEvents.FireSynergyTriggered(result);
            }

            // 3. Apply resources
            resourceManager.AddScore(totalScoreGained);
            resourceManager.AddMoney(totalIncomeGained);

            GameEvents.FireSettlementResolved(totalScoreGained, totalIncomeGained);

            // 4. Check if run is over
            if (turnManager.IsLastDay)
            {
                EndRun();
            }
            else
            {
                turnManager.FinishSettlement();
            }
        }
    }
}
