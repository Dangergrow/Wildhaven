using UnityEngine;
using System.Collections.Generic;

/// <summary>Voxel world with chunked mesh building for fast block updates.</summary>
public class GridManager : MonoBehaviour
{
    /// <summary>Fired when a block is destroyed. Args: x, y, z, oldBlockType.</summary>
    public System.Action<int, int, int, BlockType> OnBlockRemoved;
    #region Public

    [Header("World")] public int worldWidth = 100, worldHeight = 32, worldDepth = 100;
    [Header("Block")] public float blockSize = 1f; public Material blockMaterial;
    [Tooltip("Optional: assign BlockDatabase asset for per-block config")]
    public BlockDatabase blockDatabase;
    [Header("Gen")] public int seed;

    #endregion

    #region Private

    const int CHUNK = 16;

    GridCell[,,] _grid;
    int _cx, _cy, _cz;

    class Chunk
    {
        public GameObject go;
        public Mesh mesh;
        public MeshFilter filter;
        public MeshRenderer renderer;
        public List<Material> materials;
        public bool dirty = true;
    }

    Chunk[,,] _chunks;
    Texture2D _atlas;

    #endregion

    #region Accessors

    public int Width => worldWidth; public int Height => worldHeight; public int Depth => worldDepth;
    public float BlockSize => blockSize;
    public bool InBounds(int x, int y, int z) => (uint)x < worldWidth && (uint)y < worldHeight && (uint)z < worldDepth;
    public BlockType GetBlock(int x, int y, int z) => InBounds(x, y, z) ? _grid[x, y, z].blockType : BlockType.Air;
    public Vector3Int WorldToGrid(Vector3 p) => new((int)(p.x / blockSize), (int)(p.y / blockSize), (int)(p.z / blockSize));
    public Vector3 GridToWorld(int x, int y, int z) { float h = blockSize * .5f; return new(x * blockSize + h, y * blockSize + h, z * blockSize + h); }

    #endregion

    #region Unity

    void Awake()
    {
        _cx = Mathf.CeilToInt((float)worldWidth / CHUNK);
        _cy = Mathf.CeilToInt((float)worldHeight / CHUNK);
        _cz = Mathf.CeilToInt((float)worldDepth / CHUNK);
        _grid = new GridCell[worldWidth, worldHeight, worldDepth];
        _chunks = new Chunk[_cx, _cy, _cz];

        // Create chunk GameObjects
        for (int cx = 0; cx < _cx; cx++)
        for (int cy = 0; cy < _cy; cy++)
        for (int cz = 0; cz < _cz; cz++)
        {
            var c = new Chunk();
            c.go = new GameObject($"Chunk_{cx}_{cy}_{cz}");
            c.go.transform.SetParent(transform);
            c.go.transform.localPosition = Vector3.zero;
            c.filter = c.go.AddComponent<MeshFilter>();
            c.renderer = c.go.AddComponent<MeshRenderer>();
            c.mesh = new Mesh { name = $"ChunkMesh_{cx}_{cy}_{cz}", indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 };
            _chunks[cx, cy, cz] = c;
        }

        GenerateTerrain();
        if (HasSave) LoadWorld(false);
        BuildAllChunks();
    }

    #endregion

    #region Public API

    public void SetBlock(int x, int y, int z, BlockType t)
    {
        if (!InBounds(x, y, z)) return;
        _grid[x, y, z] = new GridCell(t);
        DirtyChunkAt(x, y, z);
        // Also dirty neighbors if at chunk boundary
        if (x % CHUNK == 0 && x > 0) DirtyChunkAt(x - 1, y, z);
        if (x % CHUNK == CHUNK - 1 && x < worldWidth - 1) DirtyChunkAt(x + 1, y, z);
        if (y % CHUNK == 0 && y > 0) DirtyChunkAt(x, y - 1, z);
        if (y % CHUNK == CHUNK - 1 && y < worldHeight - 1) DirtyChunkAt(x, y + 1, z);
        if (z % CHUNK == 0 && z > 0) DirtyChunkAt(x, y, z - 1);
        if (z % CHUNK == CHUNK - 1 && z < worldDepth - 1) DirtyChunkAt(x, y, z + 1);
        RebuildDirtyChunks();
    }

    public void RemoveBlock(int x, int y, int z)
    {
        BlockType old = GetBlock(x, y, z);
        SetBlock(x, y, z, BlockType.Air);
        if (old != BlockType.Air)
            OnBlockRemoved?.Invoke(x, y, z, old);
    }

    /// <summary>Grid-based raycast.</summary>
    public Vector3Int? RaycastGrid(Ray ray, float maxDist = 200f)
    {
        Vector3 ro = ray.origin, rd = ray.direction.normalized;
        float step = blockSize * .25f;
        for (float t = 0f; t < maxDist; t += step)
        {
            Vector3 p = ro + rd * t;
            int x = Mathf.FloorToInt(p.x / blockSize);
            int y = Mathf.FloorToInt(p.y / blockSize);
            int z = Mathf.FloorToInt(p.z / blockSize);
            if (InBounds(x, y, z) && _grid[x, y, z].blockType != BlockType.Air)
                return new Vector3Int(x, y, z);
        }
        return null;
    }

    #endregion

    #region Chunk Operations

    void DirtyChunkAt(int x, int y, int z)
    {
        int cx = x / CHUNK, cy = y / CHUNK, cz = z / CHUNK;
        if (cx < _cx && cy < _cy && cz < _cz) _chunks[cx, cy, cz].dirty = true;
    }

    void RebuildDirtyChunks()
    {
        for (int cx = 0; cx < _cx; cx++)
        for (int cy = 0; cy < _cy; cy++)
        for (int cz = 0; cz < _cz; cz++)
            if (_chunks[cx, cy, cz].dirty)
                BuildChunkMesh(cx, cy, cz);
    }

    void BuildAllChunks()
    {
        for (int cx = 0; cx < _cx; cx++)
        for (int cy = 0; cy < _cy; cy++)
        for (int cz = 0; cz < _cz; cz++)
            BuildChunkMesh(cx, cy, cz);
    }

    void BuildChunkMesh(int cx, int cy, int cz)
    {
        var chunk = _chunks[cx, cy, cz];
        chunk.mesh.Clear();
        chunk.dirty = false;

        int x0 = cx * CHUNK, y0 = cy * CHUNK, z0 = cz * CHUNK;
        int x1 = Mathf.Min(x0 + CHUNK, worldWidth);
        int y1 = Mathf.Min(y0 + CHUNK, worldHeight);
        int z1 = Mathf.Min(z0 + CHUNK, worldDepth);

        var V = new Dictionary<BlockType, List<Vector3>>();
        var T = new Dictionary<BlockType, List<int>>();
        var N = new Dictionary<BlockType, List<Vector3>>();
        var UV = new Dictionary<BlockType, List<Vector2>>();

        // Greedy meshing per face direction
        int[] ax = { 1, 1, 0, 0, 2, 2 };
        int[] sn = { 1, -1, 1, -1, 1, -1 };
        int[] ua = { 2, 0, 1, 2, 0, 1 };
        int[] va = { 0, 2, 2, 1, 1, 0 };
        Vector3[] nrms = { Vector3.up, Vector3.down, Vector3.right, Vector3.left, Vector3.forward, Vector3.back };

        int[] dims = { worldWidth, worldHeight, worldDepth };
        int[] mins = { x0, y0, z0 };
        int[] maxs = { x1, y1, z1 };

        for (int d = 0; d < 6; d++)
        {
            int axis = ax[d], sign = sn[d], uAx = ua[d], vAx = va[d];
            Vector3 normal = nrms[d];
            int layerMin = mins[axis], layerMax = maxs[axis];
            int uSize = maxs[uAx] - mins[uAx], vSize = maxs[vAx] - mins[vAx];

            for (int layer = layerMin; layer < layerMax; layer++)
            {
                BlockType?[,] mask = new BlockType?[uSize, vSize];
                for (int u = 0; u < uSize; u++)
                for (int v = 0; v < vSize; v++)
                {
                    int[] bc = new int[3]; bc[axis] = layer; bc[uAx] = mins[uAx] + u; bc[vAx] = mins[vAx] + v;
                    int bx = bc[0], by = bc[1], bz = bc[2];
                    BlockType t = _grid[bx, by, bz].blockType;
                    if (t == BlockType.Air) continue;
                    int nx = bx + (axis == 0 ? sign : 0);
                    int ny = by + (axis == 1 ? sign : 0);
                    int nz = bz + (axis == 2 ? sign : 0);
                    if (!InBounds(nx, ny, nz) || _grid[nx, ny, nz].IsEmpty)
                        mask[u, v] = t;
                }

                bool[,] visited = new bool[uSize, vSize];
                for (int u = 0; u < uSize; u++)
                for (int v = 0; v < vSize; v++)
                {
                    if (!mask[u, v].HasValue || visited[u, v]) continue;
                    BlockType t = mask[u, v].Value;
                    int w = 1;
                    while (u + w < uSize && mask[u + w, v].HasValue && mask[u + w, v].Value == t && !visited[u + w, v]) w++;
                    int h = 1; bool ok = true;
                    while (v + h < vSize && ok)
                    {
                        for (int k = 0; k < w; k++)
                            if (!mask[u + k, v + h].HasValue || mask[u + k, v + h].Value != t || visited[u + k, v + h])
                            { ok = false; break; }
                        if (ok) h++;
                    }
                    for (int i = 0; i < w; i++)
                    for (int j = 0; j < h; j++)
                        visited[u + i, v + j] = true;

                    if (!V.ContainsKey(t)) { V[t] = new(); T[t] = new(); N[t] = new(); UV[t] = new(); }
                    var vv = V[t]; var tt = T[t]; var nn = N[t]; var uv = UV[t]; int s = vv.Count;

                    int fc = sign > 0 ? layer + 1 : layer;
                    int[] c0 = new int[3], c1 = new int[3], c2 = new int[3], c3 = new int[3];
                    c0[axis] = fc; c0[uAx] = mins[uAx] + u;     c0[vAx] = mins[vAx] + v;
                    c1[axis] = fc; c1[uAx] = mins[uAx] + u + w; c1[vAx] = mins[vAx] + v;
                    c2[axis] = fc; c2[uAx] = mins[uAx] + u + w; c2[vAx] = mins[vAx] + v + h;
                    c3[axis] = fc; c3[uAx] = mins[uAx] + u;     c3[vAx] = mins[vAx] + v + h;

                    vv.Add(new Vector3(c0[0], c0[1], c0[2]) * blockSize);
                    vv.Add(new Vector3(c1[0], c1[1], c1[2]) * blockSize);
                    vv.Add(new Vector3(c2[0], c2[1], c2[2]) * blockSize);
                    vv.Add(new Vector3(c3[0], c3[1], c3[2]) * blockSize);
                    for (int i = 0; i < 4; i++) nn.Add(normal);

                    int idx = (int)t;
                    float uu0 = (idx % 8) / 8f, vv0 = (idx / 8) / 4f;
                    uv.Add(new Vector2(uu0, vv0)); uv.Add(new Vector2(uu0 + .125f, vv0));
                    uv.Add(new Vector2(uu0 + .125f, vv0 + .25f)); uv.Add(new Vector2(uu0, vv0 + .25f));

                    tt.Add(s); tt.Add(s + 1); tt.Add(s + 2);
                    tt.Add(s); tt.Add(s + 2); tt.Add(s + 3);
                }
            }
        }

        if (V.Count == 0) { chunk.filter.mesh = chunk.mesh; return; }

        var allV = new List<Vector3>(); var allN = new List<Vector3>(); var allUV = new List<Vector2>();
        var types = new List<BlockType>(); var subT = new List<int[]>();
        foreach (var kv in V)
        {
            int off = allV.Count;
            var tl = T[kv.Key];
            for (int i = 0; i < tl.Count; i++) tl[i] += off;
            allV.AddRange(kv.Value); allN.AddRange(N[kv.Key]); allUV.AddRange(UV[kv.Key]);
            types.Add(kv.Key); subT.Add(tl.ToArray());
        }

        chunk.mesh.subMeshCount = subT.Count;
        chunk.mesh.SetVertices(allV); chunk.mesh.SetNormals(allN); chunk.mesh.SetUVs(0, allUV);
        for (int i = 0; i < subT.Count; i++) chunk.mesh.SetTriangles(subT[i], i);
        chunk.mesh.RecalculateBounds();
        chunk.filter.mesh = chunk.mesh;

        if (blockMaterial != null)
        {
            var mats = new List<Material>();
            for (int i = 0; i < types.Count; i++)
            {
                var m = new Material(blockMaterial);
                m.SetColor("_BaseColor", BlockColor(types[i]));
                if (_atlas == null) _atlas = CreateAtlas();
                m.SetTexture("_BaseMap", _atlas);
                mats.Add(m);
            }
            chunk.renderer.materials = mats.ToArray();
        }
    }

    #endregion

    #region Terrain

    void GenerateTerrain()
    {
        if (seed == 0) seed = Random.Range(1, 1000000);
        var r = new System.Random(seed);
        for (int x = 0; x < worldWidth; x++)
        for (int z = 0; z < worldDepth; z++)
        {
            float nx = (float)x / worldWidth, nz = (float)z / worldDepth;
            float h = Mathf.PerlinNoise(nx * 2.2f + seed * .001f, nz * 2.2f + seed * .001f)
                    + Mathf.PerlinNoise(nx * 5f + seed * .002f, nz * 5f + seed * .002f) * .3f
                    + Mathf.PerlinNoise(nx * 11f + seed * .003f, nz * 11f + seed * .003f) * .15f;
            int th = Mathf.Clamp(Mathf.FloorToInt(h * worldHeight * .55f), 2, worldHeight - 1);
            for (int y = 0; y < worldHeight; y++)
            {
                if (y == 0) _grid[x, y, z] = new(BlockType.Bedrock);
                else if (y < th - 5) _grid[x, y, z] = new(BlockType.Stone);
                else if (y < th - 2)
                { int rn = r.Next(100); _grid[x, y, z] = new(rn < 2 ? BlockType.Coal : rn < 1 ? BlockType.IronOre : BlockType.Stone); }
                else if (y < th - 1) _grid[x, y, z] = new(BlockType.Dirt);
                else if (y == th && th >= worldHeight - 5) _grid[x, y, z] = new(BlockType.Snow);
                else if (y == th) _grid[x, y, z] = new(BlockType.Grass);
                else _grid[x, y, z] = GridCell.Empty;
            }
            // Water fills depressions: if terrain below sea level, fill above terrain with water
            int seaLevel = 10;
            if (th < seaLevel)
                for (int y = th + 1; y <= seaLevel; y++)
                    _grid[x, y, z] = new(BlockType.Water);
        }
    }

    #endregion

    #region Colors

    Color BlockColor(BlockType t)
    {
        if (blockDatabase != null) return blockDatabase.GetColor(t);
        return t switch
    {
        BlockType.Grass => new(.25f, .55f, .15f), BlockType.Dirt => new(.45f, .32f, .18f),
        BlockType.Stone => new(.50f, .50f, .55f), BlockType.Bedrock => new(.15f, .14f, .16f),
        BlockType.IronOre => new(.65f, .50f, .45f), BlockType.CopperOre => new(.80f, .45f, .25f),
        BlockType.Coal => new(.12f, .12f, .12f), BlockType.Sand => new(.85f, .80f, .55f),
        BlockType.Wood => new(.55f, .40f, .20f), BlockType.WoodPlanks => new(.65f, .50f, .30f),
        BlockType.StoneBrick => new(.55f, .55f, .58f), BlockType.Glass => new(.75f, .88f, .95f),
        BlockType.Water => new(.20f, .40f, .75f), BlockType.Snow => new(.95f, .95f, .97f),
        _ => Color.gray,
    };
    }

    Texture2D CreateAtlas()
    {
        int cols = 8, rows = 4, cell = 16;
        var tex = new Texture2D(cols * cell, rows * cell, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };
        var colors = new Color[cols * cell * rows * cell];
        for (int bt = 0; bt < 24; bt++)
        {
            Color bc = BlockColor((BlockType)bt);
            var rng = new System.Random(bt + 42);
            for (int dy = 0; dy < cell; dy++)
            for (int dx = 0; dx < cell; dx++)
            {
                float n = (float)rng.NextDouble() * .12f;
                float edge = (dx == 0 || dy == 0 || dx == cell - 1 || dy == cell - 1) ? .75f : 1f;
                int px = (bt % cols) * cell + dx, py = (bt / cols) * cell + dy;
                colors[py * cols * cell + px] = new Color(
                    Mathf.Clamp01(bc.r + n - .06f) * edge,
                    Mathf.Clamp01(bc.g + n - .06f) * edge,
                    Mathf.Clamp01(bc.b + n - .06f) * edge, 1);
            }
        }
        tex.SetPixels(colors); tex.Apply();
        return tex;
    }

    #endregion

    #region Save/Load

    string SavePath => System.IO.Path.Combine(Application.persistentDataPath, "world.sav");
    public bool HasSave => System.IO.File.Exists(SavePath);

    public void SaveWorld()
    {
        using (var w = new System.IO.BinaryWriter(System.IO.File.Open(SavePath, System.IO.FileMode.Create)))
        {
            w.Write((byte)'W'); w.Write((byte)'H'); w.Write((byte)'V'); w.Write((byte)'N');
            w.Write(worldWidth); w.Write(worldHeight); w.Write(worldDepth); w.Write(seed);
            for (int x = 0; x < worldWidth; x++)
            for (int y = 0; y < worldHeight; y++)
            for (int z = 0; z < worldDepth; z++)
                w.Write((byte)_grid[x, y, z].blockType);
        }
        Debug.Log("World saved");
    }

    public void LoadWorld(bool rebuild = true)
    {
        if (!HasSave) return;
        using (var r = new System.IO.BinaryReader(System.IO.File.OpenRead(SavePath)))
        {
            if (r.ReadByte() != 'W' || r.ReadByte() != 'H' || r.ReadByte() != 'V' || r.ReadByte() != 'N') return;
            int w = r.ReadInt32(), h = r.ReadInt32(), d = r.ReadInt32();
            if (w != worldWidth || h != worldHeight || d != worldDepth) return;
            seed = r.ReadInt32();
            for (int x = 0; x < worldWidth; x++)
            for (int y = 0; y < worldHeight; y++)
            for (int z = 0; z < worldDepth; z++)
                _grid[x, y, z] = new GridCell((BlockType)r.ReadByte());
        }
        if (rebuild) BuildAllChunks();
    }

    #endregion
}
