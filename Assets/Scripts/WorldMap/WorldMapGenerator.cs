using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Generates the global hex map with biomes, terrain, factions, and settlements.
/// </summary>
public class WorldMapGenerator : MonoBehaviour
{
    [Header("Map Size")]
    public int mapRadius = 30; // hexes from center to edge
    public int seed;

    [Header("Factions")]
    public FactionDef[] factions;
    public int factionCount = 8;

    [Header("Ocean")]
    [Range(0f, 1f)]
    public float oceanCoverage = 0.4f;

    // All hex tiles
    public Dictionary<(int q, int r), HexTile> Tiles { get; private set; }
        = new Dictionary<(int, int), HexTile>();

    private System.Random _rng;

    void Awake()
    {
        if (seed == 0) seed = Random.Range(1, 1000000);
        _rng = new System.Random(seed);
        GenerateMap();
        Debug.Log($"[WorldMap] Generated {Tiles.Count} hexes with seed {seed}");
    }

    void GenerateMap()
    {
        Tiles.Clear();
        int id = 0;

        // Generate hex grid
        for (int q = -mapRadius; q <= mapRadius; q++)
        {
            int r1 = Mathf.Max(-mapRadius, -q - mapRadius);
            int r2 = Mathf.Min(mapRadius, -q + mapRadius);
            for (int r = r1; r <= r2; r++)
            {
                HexTile tile = new HexTile
                {
                    id = id++,
                    q = q,
                    r = r,
                };

                // Noise-based elevation
                float nx = (q + mapRadius) / (float)(2 * mapRadius);
                float ny = (r + mapRadius) / (float)(2 * mapRadius);
                float noise = Mathf.PerlinNoise(nx * 3f + seed * 0.01f, ny * 3f + seed * 0.01f);
                noise += Mathf.PerlinNoise(nx * 6f + seed * 0.02f, ny * 6f + seed * 0.02f) * 0.5f;
                noise /= 1.5f;

                // Distance from center (for island shape)
                float dist = Mathf.Sqrt(q * q + r * r + q * r) / mapRadius;

                tile.elevation = noise * (1f - dist * 0.5f);
                tile.temperature = Mathf.PerlinNoise(nx * 4f + 999f, ny * 4f + 999f);
                tile.rainfall = Mathf.PerlinNoise(nx * 5f + 555f, ny * 5f + 555f);

                // Determine biome
                if (tile.elevation < oceanCoverage * 0.5f || dist > 1.1f)
                {
                    tile.biome = MapBiome.Ocean;
                }
                else
                {
                    tile.biome = DetermineBiome(tile.temperature, tile.rainfall, tile.elevation);
                }

                // Terrain
                tile.terrain = (tile.elevation > 0.85f) ? HexTerrain.Mountains :
                               (tile.elevation > 0.7f) ? HexTerrain.Hills : HexTerrain.Flat;

                Tiles[(q, r)] = tile;
            }
        }

        // Place roads randomly between settlements
        PlaceRoads();
        // Assign faction territories
        AssignFactions();
    }

    MapBiome DetermineBiome(float temp, float rain, float elevation)
    {
        if (elevation > 0.85f) return MapBiome.IceWastes; // mountain peaks
        if (temp < 0.2f) return MapBiome.Tundra;
        if (temp < 0.35f) return MapBiome.Taiga;
        if (rain < 0.3f) return MapBiome.Desert;
        if (rain > 0.75f && temp > 0.6f) return MapBiome.Jungle;
        if (rain > 0.6f && temp > 0.4f) return MapBiome.Swamp;
        if (temp > 0.7f && rain < 0.5f) return MapBiome.Savannah;
        if (temp > 0.65f) return MapBiome.Tropics;
        if (rain > 0.5f) return MapBiome.Forest;
        return MapBiome.Plains;
    }

    void PlaceRoads()
    {
        // Simple: connect random land hexes with roads
        for (int i = 0; i < 50; i++)
        {
            int q = _rng.Next(-mapRadius, mapRadius + 1);
            int r = _rng.Next(-mapRadius, mapRadius + 1);
            if (Tiles.TryGetValue((q, r), out HexTile tile) && tile.biome != MapBiome.Ocean)
            {
                tile.hasRoad = true;
            }
        }
    }

    void AssignFactions()
    {
        if (factions == null || factions.Length == 0)
            factions = new FactionDef[factionCount];

        // Find land hexes
        List<HexTile> land = new List<HexTile>();
        foreach (HexTile tile in Tiles.Values)
            if (tile.biome != MapBiome.Ocean) land.Add(tile);

        // Split land among factions
        for (int i = 0; i < factionCount; i++)
        {
            if (factions[i] == null) factions[i] = new FactionDef();

            factions[i].id = i;
            if (string.IsNullOrEmpty(factions[i].name))
                factions[i].name = ((FactionType)i).ToString();

            // Pick random territory center
            int centerIdx = _rng.Next(land.Count);
            HexTile center = land[centerIdx];

            // Assign nearby hexes to this faction
            List<int> territory = new List<int>();
            foreach (HexTile tile in land)
            {
                float dist = Mathf.Sqrt((tile.q - center.q) * (tile.q - center.q) +
                                        (tile.r - center.r) * (tile.r - center.r) +
                                        (tile.q - center.q) * (tile.r - center.r));
                if (dist < mapRadius * 0.3f)
                {
                    tile.factionId = i;
                    territory.Add(tile.id);
                }
            }
            factions[i].territoryHexIds = territory.ToArray();
        }

        // Assign starting player hex (center of map)
        foreach (HexTile tile in Tiles.Values)
        {
            if (tile.q == 0 && tile.r == 0 && tile.biome != MapBiome.Ocean)
            {
                tile.factionId = -999; // player
                tile.hasSettlement = true;
                break;
            }
        }
    }
}
