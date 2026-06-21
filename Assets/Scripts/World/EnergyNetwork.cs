using UnityEngine;
using System.Collections.Generic;

/// <summary>Manages electrical networks: generators, wires, consumers. Flood-fill based.</summary>
public class EnergyNetwork : MonoBehaviour
{
    private GridManager _grid;
    public float totalPower { get; private set; }
    public float totalConsumption { get; private set; }
    private float _lastUpdate;

    void Awake() { _grid = GetComponent<GridManager>(); if (_grid == null) _grid = FindObjectOfType<GridManager>(); }

    /// <summary>Recalculate the entire power grid. Called after block changes.</summary>
    public void Recalculate()
    {
        totalPower = 0;
        totalConsumption = 0;
        if (_grid == null) return;

        var visited = new HashSet<Vector3Int>();
        for (int x = 0; x < _grid.Width; x++)
        for (int y = 0; y < _grid.Height; y++)
        for (int z = 0; z < _grid.Depth; z++)
        {
            BlockType b = _grid.GetBlock(x, y, z);
            if (IsGenerator(b) && !visited.Contains(new(x, y, z)))
            {
                var network = FloodFill(x, y, z, visited);
                float genPower = 0, consumption = 0;
                foreach (var p in network)
                {
                    BlockType bt = _grid.GetBlock(p.x, p.y, p.z);
                    if (IsGenerator(bt)) genPower += GetPowerOutput(bt);
                    if (IsConsumer(bt)) consumption += GetPowerConsumption(bt);
                }
                totalPower += genPower;
                totalConsumption += consumption;
            }
        }
        _lastUpdate = Time.time;
    }

    List<Vector3Int> FloodFill(int sx, int sy, int sz, HashSet<Vector3Int> visited)
    {
        var result = new List<Vector3Int>();
        var queue = new Queue<Vector3Int>();
        queue.Enqueue(new(sx, sy, sz));
        visited.Add(new(sx, sy, sz));

        while (queue.Count > 0)
        {
            var p = queue.Dequeue();
            result.Add(p);
            foreach (var d in new Vector3Int[] { new(1,0,0), new(-1,0,0), new(0,1,0), new(0,-1,0), new(0,0,1), new(0,0,-1) })
            {
                Vector3Int n = p + d;
                if (!_grid.InBounds(n.x, n.y, n.z)) continue;
                if (visited.Contains(n)) continue;
                BlockType b = _grid.GetBlock(n.x, n.y, n.z);
                if (IsConductive(b) || IsGenerator(b) || IsConsumer(b))
                {
                    visited.Add(n);
                    queue.Enqueue(n);
                }
            }
        }
        return result;
    }

    bool IsGenerator(BlockType b) => b == BlockType.Coal; // coal = generator for now
    bool IsConsumer(BlockType b) => b == BlockType.Glass; // glass = lamp for now
    bool IsConductive(BlockType b) => b == BlockType.IronOre || b == BlockType.CopperOre || b == BlockType.GoldOre; // metal = wire

    float GetPowerOutput(BlockType b) => 100f;
    float GetPowerConsumption(BlockType b) => 10f;

    /// <summary>Check if position is powered (connected to generator with surplus power).</summary>
    public bool IsPowered(Vector3Int pos)
    {
        if (_grid == null) return false;
        BlockType b = _grid.GetBlock(pos.x, pos.y, pos.z);
        if (!IsConsumer(b) && !IsConductive(b)) return false;
        if (Time.time - _lastUpdate > 2f) Recalculate();
        return totalPower >= totalConsumption;
    }

    /// <summary>Get light radius at position (0-5 blocks).</summary>
    public int GetLightRadius(Vector3Int pos)
    {
        if (!IsPowered(pos)) return 0;
        BlockType b = _grid.GetBlock(pos.x, pos.y, pos.z);
        if (b == BlockType.Glass) return 3; // lamp block
        if (IsConductive(b)) return 1; // wire glow
        return 0;
    }
}
