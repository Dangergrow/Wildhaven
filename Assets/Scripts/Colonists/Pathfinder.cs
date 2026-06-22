using UnityEngine;
using System.Collections.Generic;

/// <summary>A* pathfinding on the voxel grid. Handles slopes, steps, and terrain.</summary>
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
    public static List<Vector3Int> FindPath(GridManager grid, Vector3Int start, Vector3Int end, int maxSteps = 1000)
    {
        start = FindWalkableNear(grid, start);
        end = FindWalkableNear(grid, end);
        if (!IsWalkable(grid, start) || !IsWalkable(grid, end)) return null;
        if (start == end) return new List<Vector3Int> { start };

        var nodes = new Dictionary<Vector3Int, Node>();
        var open = new List<Vector3Int>();

        nodes[start] = new Node { pos = start, h = Heuristic(start, end) };
        open.Add(start);

        int iterations = 0;
        while (open.Count > 0 && iterations < maxSteps * 3)
        {
            iterations++;
            int best = 0;
            for (int i = 1; i < open.Count; i++)
                if (nodes[open[i]].f < nodes[open[best]].f) best = i;

            Vector3Int cur = open[best];
            open.RemoveAt(best);

            if (cur == end)
            {
                var path = new List<Vector3Int>();
                while (cur != start)
                {
                    path.Add(cur);
                    cur = nodes[cur].parent;
                }
                path.Reverse();
                return path;
            }

            var cn = nodes[cur];
            cn.closed = true;
            nodes[cur] = cn;

            foreach (var d in Dirs)
            {
                ProcessNeighbor(grid, cur, cur + d, end, nodes, open, cn);
                // Also try step up (climb 1 block)
                ProcessNeighbor(grid, cur, cur + d + Vector3Int.up, end, nodes, open, cn);
                // Also try step down (descend 1 block)
                ProcessNeighbor(grid, cur, cur + d + Vector3Int.down, end, nodes, open, cn);
            }
        }
        return null;
    }

    static void ProcessNeighbor(GridManager grid, Vector3Int cur, Vector3Int next,
        Vector3Int end, Dictionary<Vector3Int, Node> nodes, List<Vector3Int> open, Node cn)
    {
        if (!IsWalkable(grid, next)) return;
        if (!nodes.ContainsKey(next))
            nodes[next] = new Node { pos = next, g = int.MaxValue };

        var nn = nodes[next];
        if (nn.closed) return;

        int stepCost = next.y != cur.y ? 5 : 1;
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

    public static bool IsWalkable(GridManager grid, Vector3Int p)
    {
        if (!grid.InBounds(p.x, p.y, p.z)) return false;
        BlockType here = grid.GetBlock(p.x, p.y, p.z);
        if (here == BlockType.Air)
        {
            BlockType below = grid.GetBlock(p.x, p.y - 1, p.z);
            return below != BlockType.Air && below != BlockType.Water;
        }
        return false;
    }

    static int Heuristic(Vector3Int a, Vector3Int b)
        => Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) * 2 + Mathf.Abs(a.z - b.z);

    static Vector3Int FindWalkableNear(GridManager grid, Vector3Int p)
    {
        if (IsWalkable(grid, p)) return p;
        for (int r = 1; r <= 5; r++)
            for (int dx = -r; dx <= r; dx++)
            for (int dz = -r; dz <= r; dz++)
            for (int dy = -r; dy <= r; dy++)
            {
                Vector3Int n = new(p.x + dx, p.y + dy, p.z + dz);
                if (IsWalkable(grid, n)) return n;
            }
        return p;
    }
}
