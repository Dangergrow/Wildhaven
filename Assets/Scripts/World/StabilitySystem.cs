using UnityEngine;
using System.Collections.Generic;

/// <summary>Structural integrity: BFS flood-fill from bedrock. Unsupported structural blocks collapse.</summary>
public class StabilitySystem : MonoBehaviour
{
    private GridManager _grid;
    private float _checkTimer;

    private static readonly HashSet<BlockType> StructuralBlocks = new()
    {
        BlockType.Wood, BlockType.WoodPlanks, BlockType.StoneBrick, BlockType.Glass,
        BlockType.Marble, BlockType.Obsidian, BlockType.Clay, BlockType.Ice,
        BlockType.IronOre, BlockType.CopperOre, BlockType.GoldOre, BlockType.Coal,
    };

    bool IsStructural(BlockType t) => StructuralBlocks.Contains(t);

    // 6-directional neighbors
    static readonly Vector3Int[] Dirs = {
        new(0,1,0), new(0,-1,0), new(1,0,0), new(-1,0,0), new(0,0,1), new(0,0,-1)
    };

    void Awake()
    {
        _grid = GetComponent<GridManager>();
        if (_grid == null) _grid = FindFirstObjectByType<GridManager>();
    }

    void Update()
    {
        if (_grid == null) return;
        _checkTimer += Time.unscaledDeltaTime;
        if (_checkTimer < 5f) return;
        _checkTimer = 0f;
        CheckStability();
    }

    void CheckStability()
    {
        int w = _grid.Width, h = _grid.Height, d = _grid.Depth;

        // HashSet for O(1) visited check
        var visited = new HashSet<Vector3Int>();
        var queue = new Queue<Vector3Int>();

        // Seed: all bedrock blocks (y=0) that are solid
        for (int x = 0; x < w; x++)
        for (int z = 0; z < d; z++)
        {
            if (_grid.GetBlock(x, 0, z) != BlockType.Air)
            {
                var p = new Vector3Int(x, 0, z);
                visited.Add(p);
                queue.Enqueue(p);
            }
        }

        // BFS flood-fill: explore all connected solid blocks
        while (queue.Count > 0)
        {
            Vector3Int cur = queue.Dequeue();
            foreach (Vector3Int dOff in Dirs)
            {
                int nx = cur.x + dOff.x, ny = cur.y + dOff.y, nz = cur.z + dOff.z;
                if ((uint)nx >= w || (uint)ny >= h || (uint)nz >= d) continue;
                if (_grid.GetBlock(nx, ny, nz) == BlockType.Air) continue;
                var np = new Vector3Int(nx, ny, nz);
                if (visited.Add(np))
                    queue.Enqueue(np);
            }
        }

        // Collapse: structural blocks NOT reached by BFS are unsupported
        int collapsed = 0;
        for (int x = 0; x < w; x++)
        for (int y = 0; y < h; y++)
        for (int z = 0; z < d; z++)
        {
            BlockType b = _grid.GetBlock(x, y, z);
            if (b == BlockType.Air || !IsStructural(b)) continue;
            if (!visited.Contains(new Vector3Int(x, y, z)))
            {
                _grid.RemoveBlock(x, y, z);
                collapsed++;
            }
        }

        if (collapsed > 0)
            Debug.Log($"[Stability] {collapsed} unsupported blocks collapsed");
    }
}
