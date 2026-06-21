using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages plant growth on the map. Plants grow, can be harvested, and yield items.
/// Uses grid coordinates for placement.
/// </summary>
public class PlantGrowth : MonoBehaviour
{
    [Header("Settings")]
    public float growthCheckInterval = 5f; // seconds at 1x
    public float growthMultiplier = 1f;

    [Header("Crop Definitions")]
    public CropDef[] cropDefs;

    // Active plants: grid position → plant data
    private Dictionary<Vector3Int, PlantData> _plants = new Dictionary<Vector3Int, PlantData>();
    private GridManager _grid;
    private DayCycle _day;
    private float _timer;
    private System.Random _rng = new System.Random();

    void Awake()
    {
        _grid = FindObjectOfType<GridManager>();
        _day = FindObjectOfType<DayCycle>();

        if (cropDefs == null || cropDefs.Length == 0)
        {
            cropDefs = new CropDef[]
            {
                new CropDef { cropType = CropType.Wheat, growDays = 3f, yieldAmount = 5, yieldItem = ItemType.Wheat, requiresLight = false },
                new CropDef { cropType = CropType.Potato, growDays = 4f, yieldAmount = 4, yieldItem = ItemType.Potato, requiresLight = false },
                new CropDef { cropType = CropType.Cannabis, growDays = 6f, yieldAmount = 3, yieldItem = ItemType.HempFlower, requiresLight = true, bonusItem = ItemType.SeedsCannabis, bonusChance = 0.5f },
                new CropDef { cropType = CropType.MedicalHerb, growDays = 5f, yieldAmount = 3, yieldItem = ItemType.MedicalHerb, requiresLight = false },
                new CropDef { cropType = CropType.Berry, growDays = 4f, yieldAmount = 6, yieldItem = ItemType.Berries, requiresLight = false },
                new CropDef { cropType = CropType.Cotton, growDays = 5f, yieldAmount = 4, yieldItem = ItemType.Cotton, requiresLight = false },
            };
        }
    }

    void Update()
    {
        if (_day != null && _day.IsPaused) return;

        _timer += Time.deltaTime * (_day != null ? _day.gameSpeed : 1f) * growthMultiplier;
        if (_timer >= growthCheckInterval)
        {
            _timer = 0f;
            GrowAll();
        }
    }

    /// <summary>
    /// Plants a crop at the given grid position.
    /// </summary>
    public bool Plant(int gx, int gy, int gz, CropType cropType, Colonist planter)
    {
        Vector3Int pos = new Vector3Int(gx, gy, gz);
        if (_plants.ContainsKey(pos)) return false;

        // Check soil below
        BlockType below = _grid.GetBlock(gx, gy - 1, gz);
        if (below != BlockType.Dirt && below != BlockType.Grass)
            return false;

        CropDef def = GetCropDef(cropType);
        if (def == null) return false;

        _plants[pos] = new PlantData
        {
            cropType = cropType,
            growth = 0f,
            planterId = planter != null ? planter.colonistName : "Unknown",
        };

        Debug.Log($"[Plants] {cropType} planted at ({gx},{gy},{gz})");
        return true;
    }

    /// <summary>
    /// Harvests a plant at the given position. Returns items dropped.
    /// </summary>
    public int Harvest(int gx, int gy, int gz, out ItemType item, out int amount)
    {
        item = ItemType.Wheat;
        amount = 0;
        Vector3Int pos = new Vector3Int(gx, gy, gz);

        if (!_plants.TryGetValue(pos, out PlantData plant)) return 0;
        if (plant.growth < 1f) return 0; // not ready

        CropDef def = GetCropDef(plant.cropType);
        if (def == null) return 0;

        item = def.yieldItem;
        amount = def.yieldAmount + _rng.Next(0, 3);

        // Bonus seed drop
        if (def.bonusItem != ItemType.Coal && Random.value < def.bonusChance)
        {
            // TODO: spawn bonus item as well
        }

        _plants.Remove(pos);
        Debug.Log($"[Plants] Harvested {plant.cropType} at ({gx},{gy},{gz}) → {amount}x {item}");
        return amount;
    }

    /// <summary>
    /// Advances growth for all plants.
    /// </summary>
    void GrowAll()
    {
        List<Vector3Int> toRemove = new List<Vector3Int>();
        foreach (var kvp in _plants)
        {
            CropDef def = GetCropDef(kvp.Value.cropType);
            if (def == null) { toRemove.Add(kvp.Key); continue; }

            // Light check
            bool hasLight = true;
            if (def.requiresLight)
            {
                if (_day != null && _day.IsNight)
                    hasLight = false;
            }

            float growthRate = hasLight ? 1f : 0.2f;
            float dayFraction = growthCheckInterval / (86400f / _day?.secondsPerMinute ?? 60f); // fraction of a game day

            PlantData updated = kvp.Value;
            updated.growth += dayFraction / def.growDays * growthRate;
            updated.growth = Mathf.Min(updated.growth, 1f);
            _plants[kvp.Key] = updated;
        }

        foreach (Vector3Int key in toRemove)
            _plants.Remove(key);
    }

    CropDef GetCropDef(CropType type)
    {
        foreach (CropDef def in cropDefs)
            if (def.cropType == type) return def;
        return null;
    }

    /// <summary>
    /// Checks if a plant is ready to harvest.
    /// </summary>
    public bool IsReady(int gx, int gy, int gz)
    {
        Vector3Int pos = new Vector3Int(gx, gy, gz);
        return _plants.TryGetValue(pos, out PlantData plant) && plant.growth >= 1f;
    }

    /// <summary>
    /// Returns the number of planted crops.
    /// </summary>
    public int PlantCount => _plants.Count;

    /// <summary>Try to harvest mature plant at grid position. Returns true if harvested.</summary>
    public bool TryHarvestAt(Vector3Int pos, Inventory inv)
    {
        if (!_plants.ContainsKey(pos)) return false;
        var plant = _plants[pos];
        if (plant.growth < 1f) return false; // not mature
        // Find crop def
        foreach (var def in cropDefs)
        {
            if (def.cropType == plant.cropType)
            {
                if (inv != null) inv.AddItem(def.yieldItem, def.yieldAmount);
                _plants.Remove(pos);
                return true;
            }
        }
        return false;
    }
}

[System.Serializable]
public struct PlantData
{
    public CropType cropType;
    public float growth; // 0-1
    public string planterId;
}

[System.Serializable]
public class CropDef
{
    public CropType cropType;
    public float growDays; // game days to mature
    public int yieldAmount;
    public ItemType yieldItem;
    public bool requiresLight;
    public ItemType bonusItem;
    public float bonusChance;
}

public enum CropType
{
    Wheat,
    Potato,
    Cannabis,
    MedicalHerb,
    Berry,
    Cotton,
    Tobacco,
    Hop,
    Rice,
}
