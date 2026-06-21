using UnityEngine;
using System.Collections.Generic;

/// <summary>Handles copy/paste for structures. Ctrl+C copy, Ctrl+V paste.</summary>
public class StructureClipboard : MonoBehaviour
{
    private GridManager _grid;
    private Dictionary<Vector3Int, BlockType> _copied = new();
    private Vector3Int _copyOrigin;

    void Awake() { _grid = GetComponent<GridManager>(); if (_grid == null) _grid = FindFirstObjectByType<GridManager>(); }

    /// <summary>Copy a 5x5x5 area around the position.</summary>
    public void Copy(Vector3Int center, int radius = 2)
    {
        _copied.Clear();
        _copyOrigin = center;
        for (int dx = -radius; dx <= radius; dx++)
        for (int dy = -radius; dy <= radius; dy++)
        for (int dz = -radius; dz <= radius; dz++)
        {
            Vector3Int p = new(center.x + dx, center.y + dy, center.z + dz);
            if (_grid.InBounds(p.x, p.y, p.z))
            {
                BlockType t = _grid.GetBlock(p.x, p.y, p.z);
                if (t != BlockType.Air) _copied[p - center] = t;
            }
        }
        Debug.Log($"[Clipboard] Copied {_copied.Count} blocks");
    }

    /// <summary>Paste copied blocks at new position.</summary>
    public void Paste(Vector3Int target)
    {
        foreach (var kv in _copied)
        {
            Vector3Int p = target + kv.Key;
            if (_grid.InBounds(p.x, p.y, p.z) && _grid.GetBlock(p.x, p.y, p.z) == BlockType.Air)
                _grid.SetBlock(p.x, p.y, p.z, kv.Value);
        }
        Debug.Log($"[Clipboard] Pasted {_copied.Count} blocks");
    }

    public bool HasCopy => _copied.Count > 0;
}
