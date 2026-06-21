using UnityEngine;

/// <summary>Applies research unlocks to game systems — gates building, crafting, etc.</summary>
public class ResearchEffects : MonoBehaviour
{
    private ResearchManager _research;

    void Awake() { _research = FindFirstObjectByType<ResearchManager>(); }

    /// <summary>Check if a block type is unlocked by research.</summary>
    public bool IsBlockUnlocked(BlockType type)
    {
        if (_research == null) return true; // no research = everything unlocked
        return type switch
        {
            BlockType.StoneBrick => _research.IsResearched("stonecutting"),
            BlockType.Glass => _research.IsResearched("glassmaking"),
            BlockType.IronOre => _research.IsResearched("metalworking"),
            BlockType.GoldOre => _research.IsResearched("metalworking"),
            BlockType.Obsidian => _research.IsResearched("advanced_stone"),
            BlockType.Marble => _research.IsResearched("stonecutting"),
            BlockType.CopperOre => _research.IsResearched("metalworking"),
            _ => true, // basic blocks always unlocked
        };
    }

    /// <summary>Check if a crafting station is unlocked.</summary>
    public bool IsStationUnlocked(string stationName) => _research == null || stationName switch
    {
        "Stove" => _research.IsResearched("cooking_2"),
        "Oven" => _research.IsResearched("cooking_2"),
        "Brewery" => _research.IsResearched("brewing"),
        "Smithy" => _research.IsResearched("metalworking"),
        "Laboratory" => _research.IsResearched("medicine_2"),
        _ => true,
    };
}
