namespace MayorsBlueprint.Core
{
    /// <summary>
    /// Phases within a single day.
    /// </summary>
    public enum DayPhase
    {
        DayStart,
        Shopping,
        PackOpening,
        Placement,
        Management,
        DayEnd,
        Settlement
    }

    /// <summary>
    /// Overall run state.
    /// </summary>
    public enum RunState
    {
        NotStarted,
        Running,
        Won,
        Lost
    }

    /// <summary>
    /// Building categories used for synergy rules.
    /// </summary>
    public enum BuildingCategory
    {
        Residential,
        Commercial,
        Industrial,
        Park,
        Public,
        Special
    }

    /// <summary>
    /// Building rarity tiers.
    /// </summary>
    public enum BuildingRarity
    {
        Common,
        Uncommon,
        Rare
    }

    /// <summary>
    /// How a synergy checks relationships between buildings.
    /// </summary>
    public enum SynergyCheckType
    {
        Adjacent,
        District,
        Global
    }

    /// <summary>
    /// What resource a synergy modifies.
    /// </summary>
    public enum SynergyBonusType
    {
        Score,
        Income,
        Both
    }
}
