# Research: Voxel Engine Data Structures & Performance

**Source:** "An Analysis of Minecraft-like Engines" by mikolalysenko (0fps.net, 2012)
**Relevance:** Wildhaven uses a voxel world — chunking, meshing, and iteration performance are critical.

---

## 1. Core Data Structures (Ranked by Performance)

| Data Structure | Random Access | Iteration | Memory | Verdict |
|---|---|---|---|---|
| **Flat array** | O(1) — 0.224 μs | O(n) — 0.178 μs | O(n) | Best single-access. Fixed size only. |
| **Virtualized array (chunks + hashmap)** | O(1) — 0.278 μs | O(n) — 0.210 μs | O(v) | **Industry standard.** Chunks paged by hash. |
| **Octree** | O(h) — 2.05 μs | O(n·h) — 0.981 μs | O(v·h) | **Avoid.** ~10x slower. Tree traversal kills perf. |
| **Interval tree + hash table** | O(log r_C) — 0.571 μs | **O(r)** — **0.003 μs** | O(v·r_C) | Fastest iteration. Worth considering for physics-heavy worlds. |

**Key numbers from the benchmark (256^3 volume):**
- Random read: flat array wins (0.224 μs). Interval tree is ~2.5x slower (0.571 μs).
- Sequential iteration (Moore radius=0): interval tree is **~60x faster** (0.003 vs 0.178 μs).
- Sequential iteration (Moore radius=1): interval tree is **~18x faster** (0.006 vs 0.107 μs).

---

## 2. Chunk-Based Virtualization (Recommended for Wildhaven)

This is what Minecraft itself converges on:

```
World = Dictionary<ChunkCoord, Chunk>
Chunk = Flat 3D array of blocks (e.g., 16x16x256 or 16x16x16)

Chunk lookup uses upper K bits of world coordinate as chunk ID
Chunks are lazily initialized / procedurally generated
Chunks unloaded when no players nearby
```

**C# sketch for Wildhaven:**

```csharp
public struct ChunkCoord
{
    public int x, y, z;
    // Use bit-interleave (Z-order curve) for GetHashCode
    // to keep nearby chunks near each other in the dictionary
}

public class Chunk
{
    public const int SIZE = 16;
    public BlockType[,,] blocks = new BlockType[SIZE, SIZE, SIZE];
    public bool dirty; // triggers mesh rebuild
}

public class VoxelWorld
{
    private Dictionary<ChunkCoord, Chunk> chunks = new();

    public BlockType GetBlock(int wx, int wy, int wz)
    {
        var coord = new ChunkCoord {
            x = wx >> 4, y = wy >> 4, z = wz >> 4  // >> 4 = /16
        };
        if (chunks.TryGetValue(coord, out var chunk))
            return chunk.blocks[wx & 15, wy & 15, wz & 15];  // & 15 = %16
        return BlockType.Air;
    }
}
```

---

## 3. The Iteration Bottleneck

**Key claim:** Iteration dominates random access by a factor of **(chunk size) × (visible radius)^2**.

Tasks that require iteration:
- **Mesh generation** — the #1 unavoidable iteration cost
- Lighting updates (flood-fill, day/night)
- Physics updates (sand, water, fire spread)
- Disk/network serialization

**Optimization approach used by Minecraft itself:**
- Physics does NOT iterate all blocks. It uses an "active blocks" list. Only ~20 random blocks per chunk per tick are updated.
- Lighting is computed once on chunk load, then only updated on block change.
- Mesh generation runs on dirty chunks only.

---

## 4. Run-Length Encoding & Greedy Meshing Insight

If two adjacent voxels have the **same 3×3×3 Moore neighborhood**, they can share mesh faces — this is the mathematical basis for **greedy meshing**.

```
Instead of: 6 faces per cube × N cubes = huge vertex count
Greedy mesh: merge adjacent same-type quads into larger quads
Result: far fewer vertices, faster rendering
```

The same neighborhood-grouping principle can speed up physics iteration — apply an update to an entire "run" of identical neighborhoods at once.

---

## 5. Recommendations for Wildhaven

1. **Use virtualized chunks** (16×16×16) with a Dictionary — proven, simple, O(1) access.
2. **Mesh greedily** — reduce vertex count by merging adjacent block faces of the same type.
3. **Mark chunks dirty** on block change; only rebuild meshes for dirty chunks.
4. **Don't iterate all blocks for physics** — maintain an `activeBlocks` list.
5. **Skip octrees entirely** — they're 10x slower in practice.
6. **Consider Z-order curve hashing** for chunk dictionary keys — better cache locality.
