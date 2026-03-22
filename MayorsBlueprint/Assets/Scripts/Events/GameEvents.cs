using System;

namespace MayorsBlueprint.Events
{
    /// <summary>
    /// Central static event bus for decoupled communication between systems.
    /// </summary>
    public static class GameEvents
    {
        // ── Run lifecycle ──
        public static event Action OnRunStarted;
        public static event Action<bool> OnRunEnded; // true = win

        // ── Day lifecycle ──
        public static event Action<int> OnDayStarted; // day number
        public static event Action<int> OnDayEnded;

        // ── Phase transitions ──
        public static event Action<Core.DayPhase> OnPhaseChanged;

        // ── Resources ──
        public static event Action<int> OnMoneyChanged;
        public static event Action<int> OnScoreChanged;

        // ── Shop / Packs ──
        public static event Action<Buildings.PackData> OnPackPurchased;
        public static event Action<Buildings.BuildingData[]> OnPackOpened;

        // ── Reserve ──
        public static event Action<Buildings.BuildingData, int> OnBuildingAddedToReserve; // data, slotIndex
        public static event Action<int> OnBuildingRemovedFromReserve; // slotIndex
        public static event Action OnReserveFull;

        // ── Board / Placement ──
        public static event Action<Board.PlacedBuilding> OnBuildingPlaced;
        public static event Action<Board.PlacedBuilding> OnBuildingRemoved;

        // ── Synergy ──
        public static event Action<Synergy.SynergyResult> OnSynergyTriggered;

        // ── Settlement ──
        public static event Action<int, int> OnSettlementResolved; // scoreGained, incomeGained

        // ── Fire helpers ──
        public static void FireRunStarted() => OnRunStarted?.Invoke();
        public static void FireRunEnded(bool won) => OnRunEnded?.Invoke(won);
        public static void FireDayStarted(int day) => OnDayStarted?.Invoke(day);
        public static void FireDayEnded(int day) => OnDayEnded?.Invoke(day);
        public static void FirePhaseChanged(Core.DayPhase phase) => OnPhaseChanged?.Invoke(phase);
        public static void FireMoneyChanged(int amount) => OnMoneyChanged?.Invoke(amount);
        public static void FireScoreChanged(int score) => OnScoreChanged?.Invoke(score);
        public static void FirePackPurchased(Buildings.PackData pack) => OnPackPurchased?.Invoke(pack);
        public static void FirePackOpened(Buildings.BuildingData[] buildings) => OnPackOpened?.Invoke(buildings);
        public static void FireBuildingAddedToReserve(Buildings.BuildingData data, int slot) => OnBuildingAddedToReserve?.Invoke(data, slot);
        public static void FireBuildingRemovedFromReserve(int slot) => OnBuildingRemovedFromReserve?.Invoke(slot);
        public static void FireReserveFull() => OnReserveFull?.Invoke();
        public static void FireBuildingPlaced(Board.PlacedBuilding placed) => OnBuildingPlaced?.Invoke(placed);
        public static void FireBuildingRemoved(Board.PlacedBuilding placed) => OnBuildingRemoved?.Invoke(placed);
        public static void FireSynergyTriggered(Synergy.SynergyResult result) => OnSynergyTriggered?.Invoke(result);
        public static void FireSettlementResolved(int score, int income) => OnSettlementResolved?.Invoke(score, income);
    }
}
