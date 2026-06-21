using UnityEngine;
using System.Collections.Generic;

/// <summary>Blueprint system — designate blocks for construction. Colonists build them.</summary>
public class BlueprintManager : MonoBehaviour
{
    private GridManager _grid;

    public struct Blueprint
    {
        public Vector3Int pos;
        public BlockType targetType;
        public bool assigned; // colonist working on it
    }

    private List<Blueprint> _blueprints = new();

    void Awake() { _grid = GetComponent<GridManager>(); if (_grid == null) _grid = FindObjectOfType<GridManager>(); }

    /// <summary>Add a blueprint at grid position. Colonists will build it.</summary>
    public void AddBlueprint(Vector3Int pos, BlockType type)
    {
        if (_grid == null || !_grid.InBounds(pos.x, pos.y, pos.z)) return;
        if (_grid.GetBlock(pos.x, pos.y, pos.z) != BlockType.Air) return;
        _blueprints.Add(new Blueprint { pos = pos, targetType = type });
    }

    /// <summary>Remove blueprint at position.</summary>
    public void RemoveBlueprint(Vector3Int pos)
    {
        _blueprints.RemoveAll(b => b.pos == pos);
    }

    /// <summary>Try to build all pending blueprints. Called by colonist AI.</summary>
    public bool TryBuildNext(Vector3 colonistPos, out Vector3Int buildPos)
    {
        buildPos = default;
        for (int i = _blueprints.Count - 1; i >= 0; i--)
        {
            var bp = _blueprints[i];
            if (bp.assigned) continue;
            float dist = Vector3.Distance(_grid.GridToWorld(bp.pos.x, bp.pos.y, bp.pos.z), colonistPos);
            if (dist < 3f) // within build range
            {
                _grid.SetBlock(bp.pos.x, bp.pos.y, bp.pos.z, bp.targetType);
                buildPos = bp.pos;
                _blueprints.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    /// <summary>Get blueprint count for UI.</summary>
    public int Count => _blueprints.Count;

    /// <summary>Draw blueprint markers in editor.</summary>
    void OnDrawGizmos()
    {
        if (_grid == null) return;
        Gizmos.color = new Color(0, 0.5f, 1, 0.3f);
        foreach (var bp in _blueprints)
        {
            Vector3 p = _grid.GridToWorld(bp.pos.x, bp.pos.y, bp.pos.z);
            Gizmos.DrawWireCube(p, Vector3.one * 0.9f);
        }
    }
}
