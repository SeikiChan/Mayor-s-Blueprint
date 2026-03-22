using System.Collections.Generic;
using UnityEngine;
using MayorsBlueprint.Board;
using MayorsBlueprint.Core;

namespace MayorsBlueprint.Synergy
{
    /// <summary>
    /// Evaluates all synergy rules against the current board state.
    /// Called during end-of-day settlement and for immediate placement feedback.
    /// </summary>
    public class SynergyEvaluator : MonoBehaviour
    {
        [SerializeField] private SynergyRule[] rules;

        /// <summary>
        /// Evaluate all synergies for the entire board. Used during settlement.
        /// </summary>
        public List<SynergyResult> EvaluateAll(GridBoard board)
        {
            var results = new List<SynergyResult>();

            foreach (var building in board.PlacedBuildings)
            {
                var buildingResults = EvaluateForBuilding(building, board);
                results.AddRange(buildingResults);
            }

            return results;
        }

        /// <summary>
        /// Evaluate synergies for a single building. Used for immediate placement feedback.
        /// </summary>
        public List<SynergyResult> EvaluateForBuilding(PlacedBuilding building, GridBoard board)
        {
            var results = new List<SynergyResult>();

            foreach (var rule in rules)
            {
                if (building.Data.category != rule.sourceCategory)
                    continue;

                switch (rule.checkType)
                {
                    case SynergyCheckType.Adjacent:
                        EvaluateAdjacent(building, rule, board, results);
                        break;
                    case SynergyCheckType.District:
                        EvaluateDistrict(building, rule, board, results);
                        break;
                    case SynergyCheckType.Global:
                        EvaluateGlobal(building, rule, board, results);
                        break;
                }
            }

            return results;
        }

        /// <summary>
        /// Quick preview of score/income delta for placing a building at a position.
        /// Used to show the player what they'd gain before confirming.
        /// </summary>
        public (int scoreDelta, int incomeDelta) PreviewPlacement(
            Buildings.BuildingData data, Vector2Int anchor, int rotation, GridBoard board)
        {
            // Temporarily place
            var placed = new PlacedBuilding(data, anchor, rotation);
            var results = EvaluateForBuilding(placed, board);

            int score = data.baseScore;
            int income = data.baseIncome;
            foreach (var r in results)
            {
                score += r.scoreModifier;
                income += r.incomeModifier;
            }
            return (score, income);
        }

        // ──────────────────────────────────────────────
        // Check type implementations
        // ──────────────────────────────────────────────

        private void EvaluateAdjacent(PlacedBuilding source, SynergyRule rule,
            GridBoard board, List<SynergyResult> results)
        {
            var neighbors = board.GetAdjacentBuildings(source);
            foreach (var neighbor in neighbors)
            {
                if (neighbor.Data.category != rule.targetCategory) continue;

                int scoreMod = rule.isNegative ? -rule.scoreBonus : rule.scoreBonus;
                int incomeMod = rule.isNegative ? -rule.incomeBonus : rule.incomeBonus;

                results.Add(new SynergyResult
                {
                    rule = rule,
                    sourceCell = source.Anchor,
                    targetCell = neighbor.Anchor,
                    scoreModifier = scoreMod,
                    incomeModifier = incomeMod
                });
            }
        }

        private void EvaluateDistrict(PlacedBuilding source, SynergyRule rule,
            GridBoard board, List<SynergyResult> results)
        {
            // District synergy: bonus if 3+ buildings of the target category exist on board
            var categoryBuildings = board.GetBuildingsByCategory(rule.targetCategory);
            if (categoryBuildings.Count >= 3)
            {
                int scoreMod = rule.isNegative ? -rule.scoreBonus : rule.scoreBonus;
                int incomeMod = rule.isNegative ? -rule.incomeBonus : rule.incomeBonus;

                results.Add(new SynergyResult
                {
                    rule = rule,
                    sourceCell = source.Anchor,
                    targetCell = Vector2Int.zero,
                    scoreModifier = scoreMod,
                    incomeModifier = incomeMod
                });
            }
        }

        private void EvaluateGlobal(PlacedBuilding source, SynergyRule rule,
            GridBoard board, List<SynergyResult> results)
        {
            // Global synergy: bonus for each building of the target category anywhere on board
            var categoryBuildings = board.GetBuildingsByCategory(rule.targetCategory);
            foreach (var target in categoryBuildings)
            {
                if (target == source) continue;

                int scoreMod = rule.isNegative ? -rule.scoreBonus : rule.scoreBonus;
                int incomeMod = rule.isNegative ? -rule.incomeBonus : rule.incomeBonus;

                results.Add(new SynergyResult
                {
                    rule = rule,
                    sourceCell = source.Anchor,
                    targetCell = target.Anchor,
                    scoreModifier = scoreMod,
                    incomeModifier = incomeMod
                });
            }
        }
    }
}
