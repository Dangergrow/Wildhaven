/// <summary>
/// Single hex tile on the global map.
/// </summary>
[System.Serializable]
public class HexTile
{
    public int id;
    public int q, r; // axial coordinates
    public MapBiome biome;
    public HexTerrain terrain;
    public float elevation;
    public float temperature;
    public float rainfall;
    public int factionId = -1; // -1 = unclaimed
    public bool hasSettlement;
    public bool hasRoad;
    public bool hasRiver;

    /// <summary>Is this tile passable by caravans?</summary>
    public bool IsPassable => biome != MapBiome.Ocean && terrain != HexTerrain.Mountains;

    /// <summary>Movement cost multiplier for caravans (1 = normal).</summary>
    public float MovementCost
    {
        get
        {
            float cost = 1f;
            if (terrain == HexTerrain.Hills) cost *= 1.5f;
            if (terrain == HexTerrain.Mountains) cost *= 3f;
            if (biome == MapBiome.Swamp) cost *= 2f;
            if (biome == MapBiome.Desert) cost *= 1.3f;
            if (biome == MapBiome.IceWastes) cost *= 2f;
            if (hasRoad) cost *= 0.5f;
            if (hasRiver) cost *= 0.8f;
            return cost;
        }
    }
}
