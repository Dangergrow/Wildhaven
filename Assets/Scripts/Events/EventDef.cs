/// <summary>
/// Types of random events that can occur in the colony.
/// </summary>
public enum EventType
{
    // Positive
    WandererJoins,
    TradeCaravan,
    CropBoom,
    AncientCache,
    Inspiration,
    Pilgrims,
    MeteoriteLoot,
    BumperHarvest,

    // Negative
    Raid,
    Plague,
    Blight,
    Drought,
    SolarFlare,
    ToxicFallout,
    PsychicWave,
    ManhunterPack,
    Infestation,
    Flashstorm,
    VolcanicWinter,
    Eclipse,
    RaiderSiege,

    // Neutral
    Refugees,
    Thunderstorm,
    HeatWave,
    ColdSnap,
    Aurora,
    AnimalMigration,
}

/// <summary>
/// Defines a game event with its properties.
/// </summary>
[System.Serializable]
public class GameEventDef
{
    public EventType type;
    public string title;
    public string description;
    public float duration; // 0 = instant
    public float minDay; // earliest day it can happen
    public float weight; // relative probability
    public bool repeats = true;
}
