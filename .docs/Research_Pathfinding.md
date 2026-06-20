# Research: A* Pathfinding for Grid-Based Worlds

**Source:** Red Blob Games — "Implementation of A*" (redblobgames.com)
**Relevance:** Wildhaven colonist AI needs grid-based pathfinding on the voxel terrain.

---

## 1. A* Algorithm Summary

A* = Dijkstra's Algorithm + a heuristic. It guarantees the shortest path if the heuristic is **admissible** (never overestimates).

```
f(n) = g(n) + h(n)
  g(n) = actual cost from start to node n
  h(n) = heuristic estimate from node n to goal
```

For grid worlds, use **Manhattan distance** as the heuristic:
```
h(a, b) = |a.x - b.x| + |a.y - b.y|   // 4-direction movement
h(a, b) = max(|dx|, |dy|)            // 8-direction movement (diagonal)
```

---

## 2. C# A* Implementation (Adapted for Wildhaven)

```csharp
public struct GridNode
{
    public int x, y, z; // z = height layer on voxel terrain
}

public class PathNode
{
    public GridNode position;
    public float gCost;  // cost from start
    public float hCost;  // heuristic to goal
    public float fCost => gCost + hCost;
    public GridNode parent;
    public bool isWalkable;
}

public class Pathfinder
{
    private int maxX, maxZ;
    private Dictionary<(int, int), float> heightMap; // terrain heights

    // Manhattan distance heuristic (3D or 2D projected)
    private static float Heuristic(GridNode a, GridNode b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.z - b.z);
    }

    public List<GridNode> FindPath(GridNode start, GridNode goal)
    {
        var openSet = new PriorityQueue<GridNode, float>();
        var cameFrom = new Dictionary<GridNode, GridNode>();
        var gScore = new Dictionary<GridNode, float>();

        openSet.Enqueue(start, 0);
        gScore[start] = 0;

        while (openSet.Count > 0)
        {
            var current = openSet.Dequeue();

            if (current.Equals(goal))
                return ReconstructPath(cameFrom, current);

            foreach (var neighbor in GetNeighbors(current))
            {
                float tentativeG = gScore[current] + MoveCost(current, neighbor);

                if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
                {
                    gScore[neighbor] = tentativeG;
                    float f = tentativeG + Heuristic(neighbor, goal);
                    openSet.Enqueue(neighbor, f);
                    cameFrom[neighbor] = current;
                }
            }
        }
        return new List<GridNode>(); // no path
    }

    private List<GridNode> GetNeighbors(GridNode node)
    {
        var neighbors = new List<GridNode>();
        // 4 cardinal directions on XZ plane
        int[] dx = { 1, -1, 0, 0 };
        int[] dz = { 0, 0, 1, -1 };

        for (int i = 0; i < 4; i++)
        {
            int nx = node.x + dx[i];
            int nz = node.z + dz[i];
            if (!IsWalkable(nx, nz)) continue;

            // Get height from terrain — clamp step height
            int ny = GetTerrainHeight(nx, nz);
            if (Mathf.Abs(ny - node.y) > 1) continue; // max step height = 1

            neighbors.Add(new GridNode { x = nx, y = ny, z = nz });
        }
        return neighbors;
    }

    private float MoveCost(GridNode a, GridNode b)
    {
        // Base cost 1.0 + extra for climbing
        return 1.0f + Mathf.Abs(b.y - a.y) * 0.5f;
    }

    private List<GridNode> ReconstructPath(Dictionary<GridNode, GridNode> cameFrom, GridNode current)
    {
        var path = new List<GridNode> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Add(current);
        }
        path.Reverse();
        return path;
    }
}
```

---

## 3. Priority Queue Note

Unity doesn't ship with `PriorityQueue<TElement, TPriority>`. Options:
- **.NET 6+**: `System.Collections.Generic.PriorityQueue` is built-in.
- **Older Unity**: Use [BlueRaja's High-Speed-Priority-Queue-for-C-Sharp](https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp) (NuGet / copy source).
- **Quick placeholder**: `SortedDictionary<float, List<GridNode>>` works but is slower.

---

## 4. Optimizations (from Red Blob Games)

| Technique | Description |
|---|---|
| **Early exit** | Break as soon as `current == goal`. A* already does this by design. |
| **No decrease-key** | Don't bother updating priority of duplicates in open set. Just allow duplicates and skip already-visited nodes when they pop. The algorithm still works and is often faster in practice. |
| **External storage** | Use Dictionaries for `cameFrom` and `gScore` instead of embedding them in node objects. Avoids expensive initialization of large arrays. |
| **Skip closed set** | Merge `open` and `closed` into a single `reached` collection. Check `gScore.ContainsKey(neighbor)` to know if a node was already visited. |
| **Neighbor order** | Alternating the search direction `if ((x+y) % 2 == 0) reverse(neighbors)` produces straighter-looking paths for BFS/Dijkstra (ties broken more evenly). |
| **Reuse arrays** | Allocate neighbor list once and pass it in — avoids GC allocations in Unity C#. |

---

## 5. Ugly Paths & Solutions

Grid-based A* often produces "staircase" or "wobbly" paths because many paths have equal cost.

**Fixes:**
1. **Tie-breaking in heuristic** — add a tiny cross-product term: `h += cross * 0.001`
2. **Line-of-sight post-processing** — after finding the path, remove intermediate waypoints that have a clear LOS to each other (string pulling).
3. **The "ugly paths" neighbor trick** — reverse neighbor order on alternating grid cells (see section above). Only helps BFS/Dijkstra, not A* with a good heuristic.

---

## 6. Wildhaven-Specific Notes

- **Grid is XZ-plane** with Y as height (voxel terrain).
- **Max step height = 1 block** — colonists can climb 1-block ledges.
- **Walkability** depends on the top block of each column being solid ground (not air, not water).
- **Path caching**: For repeated queries (e.g., many colonists to same goal), cache the `cameFrom` map as a flow field.
- **3D A\***: If colonists need ladders or can fall, extend to full 3D neighbors (26-direction).
