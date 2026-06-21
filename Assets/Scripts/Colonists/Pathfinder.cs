using UnityEngine;
using System.Collections.Generic;

/// <summary>A* pathfinding on the voxel grid. Used by colonists for player orders.</summary>
public static class Pathfinder
{
    struct Node
    {
        public Vector3Int pos;
        public int g, h, f;
        public Vector3Int parent;
        public bool closed;
    }

    static readonly Vector3Int[] Dirs = {
        new(1,0,0), new(-1,0,0), new(0,0,1), new(0,0,-1)
    };

    /// <summary>Find path from start to end on the terrain surface. Returns list of grid positions or null.</summary>
    public static List<Vector3Int> FindPath(GridManager grid, Vector3Int start, Vector3Int end, int maxSteps = 500)
    {
        // Try to find walkable positions near start/end
        start = FindWalkableNear(grid, start);
        end = FindWalkableNear(grid, end);
        if (!IsWalkable(grid, start) || !IsWalkable(grid, end)) return null;
        if (start == end) return new List<Vector3Int> { start };

        var nodes = new Dictionary<Vector3Int, Node>();
        var open = new List<Vector3Int>();

        nodes[start] = new Node { pos = start, h = Heuristic(start, end) };
        open.Add(start);

        while (open.Count > 0 && open.Count < maxSteps)
        {
            // Find lowest F
            int best = 0;
            for (int i = 1; i < open.Count; i++)
                if (nodes[open[i]].f < nodes[open[best]].f) best = i;

            Vector3Int cur = open[best];
            open.RemoveAt(best);

            if (cur == end)
            {
                // Reconstruct path
                var path = new List<Vector3Int>();
                while (cur != start)
                {
                    path.Add(cur);
                    cur = nodes[cur].parent;
                }
                path.Add(start);
                path.Reverse();
                return path;
            }

            var cn = nodes[cur];
            cn.closed = true;
            nodes[cur] = cn;

            foreach (var d in Dirs)
            {
                Vector3Int next = cur + d;
                // Step handling: if next is solid, try stepping up
                if (!IsWalkable(grid, next))
                {
                    Vector3Int stepUp = next + Vector3Int.up;
                    if (IsWalkable(grid, stepUp)) next = stepUp;
                    else continue;
                }
                // Also handle stepping down
                if (IsWalkable(grid, next + Vector3Int.down) && grid.GetBlock(next.x, next.y - 1, next.z) != BlockType.Air)
                    next = next + Vector3Int.down; // prefer staying on surface

                if (!nodes.ContainsKey(next))
                    nodes[next] = new Node { pos = next, g = int.MaxValue };

                var nn = nodes[next];
                if (nn.closed) continue;

                int stepCost = next.y != cur.y ? 3 : 1; // stepping up costs more
                int newG = cn.g + stepCost;
                if (newG < nn.g || nn.g == 0)
                {
                    nn.g = newG;
                    nn.h = Heuristic(next, end);
                    nn.f = nn.g + nn.h;
                    nn.parent = cur;
                    nodes[next] = nn;
                    if (!open.Contains(next)) open.Add(next);
                }
            }
        }
        return null; // no path found
    }

    static bool IsWalkable(GridManager grid, Vector3Int p)
    {
        if (!grid.InBounds(p.x, p.y, p.z)) return false;
        BlockType here = grid.GetBlock(p.x, p.y, p.z);
        if (here != BlockType.Air) return false;
        BlockType below = grid.GetBlock(p.x, p.y - 1, p.z);
        // Walkable: solid ground below OR stairs adjacent
        if (below != BlockType.Air && below != BlockType.Water) return true;
        // Allow stepping onto stairs blocks
        if (here == BlockType.WoodPlanks) return true; // stairs
        return false;
    }

    static int Heuristic(Vector3Int a, Vector3Int b)
        => Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);

    /// <summary>Find nearest walkable cell within 3 block radius.</summary>
    static Vector3Int FindWalkableNear(GridManager grid, Vector3Int p)
    {
        if (IsWalkable(grid, p)) return p;
        for (int r = 1; r <= 3; r++)
            for (int dx = -r; dx <= r; dx++)
            for (int dz = -r; dz <= r; dz++)
            for (int dy = -r; dy <= r; dy++)
            {
                Vector3Int n = new(p.x + dx, p.y + dy, p.z + dz);
                if (IsWalkable(grid, n)) return n;
            }
        return p; // give up, will fail walkability check
    }
}
