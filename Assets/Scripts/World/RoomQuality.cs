using UnityEngine;

/// <summary>Detects rooms and calculates quality based on size, walls, and contents.</summary>
public class RoomQuality : MonoBehaviour
{
    private GridManager _grid;

    void Awake() { _grid = GetComponent<GridManager>(); if (_grid == null) _grid = FindObjectOfType<GridManager>(); }

    /// <summary>Check if a position is inside an enclosed room and return quality 0-100.</summary>
    public float GetRoomQuality(Vector3 worldPos)
    {
        if (_grid == null) return 50f;
        Vector3Int p = _grid.WorldToGrid(worldPos);
        // Simple check: are all 4 horizontal neighbors solid?
        int solidSides = 0;
        int wallQuality = 0;
        Vector3Int[] dirs = { new(1,0,0), new(-1,0,0), new(0,0,1), new(0,0,-1) };
        foreach (var d in dirs)
        {
            Vector3Int n = p + d;
            BlockType b = _grid.GetBlock(n.x, n.y, n.z);
            if (b != BlockType.Air && b != BlockType.Water) { solidSides++; wallQuality += GetWallScore(b); }
        }

        if (solidSides < 3) return 20f; // not enclosed

        // Check above for ceiling
        bool hasCeiling = _grid.GetBlock(p.x, p.y + 1, p.z) != BlockType.Air;

        // Score: walls (0-40) + ceiling (20) + size bonus (0-40)
        float score = Mathf.Min(wallQuality * 10, 40);
        if (hasCeiling) score += 20;
        score += Mathf.Min(solidSides * 10, 40);
        return Mathf.Clamp(score, 10, 100);
    }

    int GetWallScore(BlockType t) => t switch
    {
        BlockType.StoneBrick => 4,
        BlockType.Marble => 5,
        BlockType.WoodPlanks => 3,
        BlockType.Wood => 2,
        BlockType.Stone => 2,
        BlockType.Dirt => 1,
        BlockType.Sand => 1,
        BlockType.Glass => 3,
        BlockType.Obsidian => 4,
        _ => 1,
    };

    /// <summary>Get quality label for display.</summary>
    public static string GetLabel(float q) => q switch
    {
        >= 90 => "Luxurious",
        >= 70 => "Impressive",
        >= 50 => "Decent",
        >= 30 => "Mediocre",
        >= 15 => "Poor",
        _ => "Awful",
    };
}
