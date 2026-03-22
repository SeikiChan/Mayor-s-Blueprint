using UnityEngine;
using MayorsBlueprint.Events;

namespace MayorsBlueprint.Core
{
    /// <summary>
    /// Manages the day/phase structure of a run.
    /// Drives the core loop: DayStart → Shopping → PackOpening → Placement → Management → DayEnd → Settlement.
    /// </summary>
    public class TurnManager : MonoBehaviour
    {
        private int currentDay;
        private DayPhase currentPhase;
        private int totalDays;

        public int CurrentDay => currentDay;
        public DayPhase CurrentPhase => currentPhase;
        public bool IsLastDay => currentDay >= totalDays;

        public void Initialize(int days)
        {
            totalDays = days;
            currentDay = 0;
        }

        // ──────────────────────────────────────────────
        // Phase progression
        // ──────────────────────────────────────────────

        /// <summary>Start a new day. Called at the beginning of the run and after each settlement.</summary>
        public void StartNewDay()
        {
            currentDay++;
            SetPhase(DayPhase.DayStart);
            GameEvents.FireDayStarted(currentDay);

            // Automatically advance to shopping phase
            SetPhase(DayPhase.Shopping);
        }

        /// <summary>Player finished buying packs, move to opening.</summary>
        public void FinishShopping()
        {
            if (currentPhase != DayPhase.Shopping) return;
            SetPhase(DayPhase.PackOpening);
        }

        /// <summary>All packs opened, move to placement.</summary>
        public void FinishPackOpening()
        {
            if (currentPhase != DayPhase.PackOpening) return;
            SetPhase(DayPhase.Placement);
        }

        /// <summary>Player finished placing buildings, move to management.</summary>
        public void FinishPlacement()
        {
            if (currentPhase != DayPhase.Placement) return;
            SetPhase(DayPhase.Management);
        }

        /// <summary>Player finished optional management actions, end the day.</summary>
        public void FinishManagement()
        {
            if (currentPhase != DayPhase.Management) return;
            SetPhase(DayPhase.DayEnd);
            GameEvents.FireDayEnded(currentDay);

            // Move to settlement
            SetPhase(DayPhase.Settlement);
        }

        /// <summary>Settlement is resolved, ready for next day or run end.</summary>
        public void FinishSettlement()
        {
            if (currentPhase != DayPhase.Settlement) return;

            if (IsLastDay)
            {
                // Run is over — GameManager will check win/lose
                return;
            }

            StartNewDay();
        }

        /// <summary>Allow skipping directly from placement to day end (skip management).</summary>
        public void SkipToEndDay()
        {
            if (currentPhase == DayPhase.Placement || currentPhase == DayPhase.Management)
            {
                SetPhase(DayPhase.DayEnd);
                GameEvents.FireDayEnded(currentDay);
                SetPhase(DayPhase.Settlement);
            }
        }

        private void SetPhase(DayPhase phase)
        {
            currentPhase = phase;
            GameEvents.FirePhaseChanged(phase);
        }
    }
}
