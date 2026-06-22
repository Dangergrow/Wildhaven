using UnityEngine;
using System.Collections.Generic;

/// <summary>Structural integrity: unsupported player-placed blocks collapse.</summary>
public class StabilitySystem : MonoBehaviour
{
    private GridManager _grid;
    private float _checkTimer;
    private HashSet<Vector3Int> _supported = new();

    /// <summary>Block types that need structural support (player-placed). Natural terrain is exempt.</summary>
    private static readonly HashSet<BlockType> StructuralBlocks = new()
    {
        BlockType.Wood, BlockType.WoodPlanks, BlockType.StoneBrick, BlockType.Glass,
        BlockType.Marble, BlockType.Obsidian, BlockType.Clay, BlockType.Ice,
        BlockType.IronOre, BlockType.CopperOre, BlockType.GoldOre, BlockType.Coal,
    };

    bool IsStructural(BlockType t) => StructuralBlocks.Contains(t);
    bool IsNatural(BlockType t) => t == BlockType.Grass || t == BlockType.Stone ||
        t == BlockType.Sand || t == BlockType.Gravel || t == BlockType.Water || t == BlockType.Snow ||
        t == BlockType.Bedrock;

    void Awake() { _grid = GetComponent<GridManager>(); if (_grid == null) _grid = FindFirstObjectByType<GridManager>(); }

    void Update()
    {
        _checkTimer += Time.unscaledDeltaTime;
        if (_checkTimer < 5f) return;
        _checkTimer = 0f;
        CheckStability();
    }

    void CheckStability()
    {
        if (_grid == null) return;
        _supported.Clear();

        // Mark bedrock + natural terrain as always supported
        for (int x = 0; x < _grid.Width; x++)
        for (int z = 0; z < _grid.Depth; z++)
        {
            for (int y = 0; y < _grid.Height; y++)
            {
                BlockType b = _grid.GetBlock(x, y, z);
                if (b == BlockType.Air) continue;
                if (y == 0 || IsNatural(b))
                    _supported.Add(new(x, y, z));
                else if (IsStructural(b))
                    break; // stop at first structural block in column
                else break; // unknown type — treat as natural barrier
            }
        }

        // BFS: expand support upward through structural blocks
        bool changed = true;
        while (changed)
        {
            changed = false;
            for (int x = 0; x < _grid.Width; x++)
            for (int y = 1; y < _grid.Height; y++)
            for (int z = 0; z < _grid.Depth; z++)
            {
                Vector3Int p = new(x, y, z);
                if (_supported.Contains(p)) continue;
                BlockType b = _grid.GetBlock(x, y, z);
                if (b == BlockType.Air) continue;
                if (!IsStructural(b)) { _supported.Add(p); continue; } // natural blocks are always supported
                if (HasSupport(x, y, z)) { _supported.Add(p); changed = true; }
            }
        }

        // Collapse unsupported structural blocks
        int collapsed = 0;
        for (int x = 0; x < _grid.Width; x++)
        for (int y = 0; y < _grid.Height; y++)
        for (int z = 0; z < _grid.Depth; z++)
        {
            if (_grid.GetBlock(x, y, z) == BlockType.Air) continue;
            if (!IsStructural(_grid.GetBlock(x, y, z))) continue;
            if (!_supported.Contains(new(x, y, z)))
            {
                _grid.RemoveBlock(x, y, z);
                collapsed++;
            }
        }
        if (collapsed > 0) Debug.Log($"[Stability] {collapsed} player-placed blocks collapsed!");
    }

    bool HasSupport(int x, int y, int z)
    {
        // Check 4 horizontal neighbors + below
        Vector3Int[] check = { new(x-1,y,z), new(x+1,y,z), new(x,y-1,z), new(x,y,z-1), new(x,y,z+1) };
        foreach (var c in check)
            if (_supported.Contains(c)) return true;
        return false;
    }
}
