using UnityEngine;
using System.Collections.Generic;

/// <summary>Structural integrity: unsupported blocks collapse. Simulates GM stability.</summary>
public class StabilitySystem : MonoBehaviour
{
    private GridManager _grid;
    private float _checkTimer;
    private HashSet<Vector3Int> _supported = new();

    void Awake() { _grid = GetComponent<GridManager>(); if (_grid == null) _grid = FindFirstObjectByType<GridManager>(); }

    void Update()
    {
        _checkTimer += Time.unscaledDeltaTime;
        if (_checkTimer < 5f) return;
        _checkTimer = 0f;
        CheckStability();
    }

    /// <summary>Check all blocks — unsupported ones collapse. Multi-pass for correct chains.</summary>
    void CheckStability()
    {
        if (_grid == null) return;
        _supported.Clear();

        // Mark bedrock as always supported
        for (int x = 0; x < _grid.Width; x++)
        for (int z = 0; z < _grid.Depth; z++)
            _supported.Add(new(x, 0, z));

        // Multi-pass BFS: keep expanding support until no new blocks are added
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
                if (_grid.GetBlock(x, y, z) == BlockType.Air) continue;
                if (HasSupport(x, y, z)) { _supported.Add(p); changed = true; }
            }
        }

        // Collapse unsupported blocks
        int collapsed = 0;
        for (int x = 0; x < _grid.Width; x++)
        for (int y = 0; y < _grid.Height; y++)
        for (int z = 0; z < _grid.Depth; z++)
        {
            Vector3Int p = new(x, y, z);
            if (_grid.GetBlock(x, y, z) == BlockType.Air) continue;
            if (y == 0) continue; // bedrock stays
            if (!_supported.Contains(p))
            {
                _grid.RemoveBlock(x, y, z);
                collapsed++;
            }
        }
        if (collapsed > 0) Debug.Log($"[Stability] {collapsed} blocks collapsed!");
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
