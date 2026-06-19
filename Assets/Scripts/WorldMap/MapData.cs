/// <summary>
/// Biome types for hex tiles on the global map.
/// </summary>
public enum MapBiome
{
    Tundra,
    Taiga,
    Forest,
    Plains,
    Swamp,
    Desert,
    Savannah,
    Tropics,
    Jungle,
    IceWastes,
    Volcanic,
    MushroomForest,
    CrystalCaves,
    Deadlands,
    Ocean,
}

/// <summary>
/// Terrain type of a hex.
/// </summary>
public enum HexTerrain
{
    Flat,
    Hills,
    Mountains,
    River,
    Lake,
    Coast,
    Road,
}

/// <summary>
/// Defines a faction in the world.
/// </summary>
[System.Serializable]
public class FactionDef
{
    public string name;
    public int id;
    public FactionType type;
    public float aggression; // 0-1, likelihood of raiding
    public float techLevel;  // 0-5, which era they're in
    public int[] territoryHexIds; // which hexes they own
    public int reputation; // -100 to +100, your standing with them
}

public enum FactionType
{
    Empire,
    Tribe,
    TradeGuild,
    Pirates,
    Cult,
    Nomads,
    Knights,
    Bandits,
    Hermits,
    Mutants,
    Player,
}
