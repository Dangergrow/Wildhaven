using UnityEngine;
using System.Collections.Generic;

/// <summary>Voxel world: terrain + mesh with per-block-type submeshes and URP/Unlit materials.</summary>
public class GridManager : MonoBehaviour
{
    #region Public

    [Header("World")]
    public int worldWidth = 100, worldHeight = 32, worldDepth = 100;
    [Header("Block")]
    public float blockSize = 1f;
    public Material blockMaterial;
    [Header("Gen")]
    public int seed;

    #endregion

    #region Private

    GridCell[,,] _grid;
    Mesh _mesh;
    MeshFilter _mf;
    MeshRenderer _mr;

    #endregion

    public int Width => worldWidth;
    public int Height => worldHeight;
    public int Depth => worldDepth;
    public float BlockSize => blockSize;

    void Awake()
    {
        _grid = new GridCell[worldWidth, worldHeight, worldDepth];
        _mesh = new Mesh { name = "WorldMesh", indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 };
        _mf = GetComponent<MeshFilter>(); if (!_mf) _mf = gameObject.AddComponent<MeshFilter>();
        _mr = GetComponent<MeshRenderer>(); if (!_mr) _mr = gameObject.AddComponent<MeshRenderer>();

        // Ensure collider exists before BuildMesh
        if (!GetComponent<MeshCollider>()) gameObject.AddComponent<MeshCollider>();

        GenerateTerrain();
        BuildMesh();
    }

    #region API

    public void SetBlock(int x, int y, int z, BlockType t)
        { if (InBounds(x, y, z)) { _grid[x, y, z] = new GridCell(t); BuildMesh(); } }
    public void RemoveBlock(int x, int y, int z) => SetBlock(x, y, z, BlockType.Air);
    public BlockType GetBlock(int x, int y, int z)
        => InBounds(x, y, z) ? _grid[x, y, z].blockType : BlockType.Air;
    public bool InBounds(int x, int y, int z)
        => (uint)x < worldWidth && (uint)y < worldHeight && (uint)z < worldDepth;
    public Vector3Int WorldToGrid(Vector3 p)
        => new((int)(p.x / blockSize), (int)(p.y / blockSize), (int)(p.z / blockSize));
    public Vector3 GridToWorld(int x, int y, int z)
        { float h = blockSize * .5f; return new(x * blockSize + h, y * blockSize + h, z * blockSize + h); }

    /// <summary>Voxel traversal raycast. Returns grid pos of first solid block, or null.</summary>
    public Vector3Int? RaycastGrid(Ray ray, float maxDist = 200f)
    {
        Vector3 ro = ray.origin;
        Vector3 rd = ray.direction.normalized;
        float step = blockSize * 0.25f;
        for (float t = 0f; t < maxDist; t += step)
        {
            Vector3 p = ro + rd * t;
            int x = Mathf.FloorToInt(p.x / blockSize);
            int y = Mathf.FloorToInt(p.y / blockSize);
            int z = Mathf.FloorToInt(p.z / blockSize);
            if (x >= 0 && x < worldWidth && y >= 0 && y < worldHeight && z >= 0 && z < worldDepth)
                if (_grid[x, y, z].blockType != BlockType.Air)
                    return new Vector3Int(x, y, z);
        }
        return null;
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
            // Continental + mountain noise
            float continent = Mathf.PerlinNoise(nx * 2.5f + seed * .001f, nz * 2.5f + seed * .001f);
            float hills = Mathf.PerlinNoise(nx * 6f + seed * .002f, nz * 6f + seed * .002f) * .4f;
            float detail = Mathf.PerlinNoise(nx * 12f + seed * .003f, nz * 12f + seed * .003f) * .15f;
            float h = continent + hills + detail;
            // Map to 4..worldHeight-1 range for more dramatic terrain
            int th = Mathf.Clamp(Mathf.FloorToInt(h * worldHeight * .85f), 4, worldHeight - 1);

            for (int y = 0; y < worldHeight; y++)
            {
                if (y == 0) _grid[x, y, z] = new(BlockType.Bedrock);
                else if (y <= 2 && y < th) _grid[x, y, z] = new(BlockType.Water); // water level
                else if (y < th - 4) _grid[x, y, z] = new(BlockType.Stone);
                else if (y < th - 1)
                { int rn = r.Next(100); _grid[x, y, z] = new(rn < 3 ? BlockType.Coal : rn < 4 ? BlockType.IronOre : BlockType.Stone); }
                else if (y == th - 1) _grid[x, y, z] = new(BlockType.Dirt);
                else if (y == th) _grid[x, y, z] = new(BlockType.Grass);
                else _grid[x, y, z] = GridCell.Empty;
            }
        }
    }

    #endregion

    #region Mesh (Greedy)

    void BuildMesh()
    {
        _mesh.Clear();
        var V = new Dictionary<BlockType, List<Vector3>>();
        var T = new Dictionary<BlockType, List<int>>();
        var N = new Dictionary<BlockType, List<Vector3>>();
        var UV = new Dictionary<BlockType, List<Vector2>>();

        // Direction metadata: axis, sign, uAxis, vAxis, normal
        int[] ax = { 1, 1, 0, 0, 2, 2 };   // normal axis
        int[] sn = { 1, -1, 1, -1, 1, -1 }; // sign
        int[] ua = { 2, 0, 1, 2, 0, 1 };   // u axis
        int[] va = { 0, 2, 2, 1, 1, 0 };   // v axis
        Vector3[] nrms = { Vector3.up, Vector3.down, Vector3.right, Vector3.left, Vector3.forward, Vector3.back };

        for (int d = 0; d < 6; d++)
        {
            int axis = ax[d], sign = sn[d], uAx = ua[d], vAx = va[d];
            Vector3 normal = nrms[d];
            int layers = Dim(axis), uSize = Dim(uAx), vSize = Dim(vAx);

            for (int layer = 0; layer < layers; layer++)
            {
                // Build 2D mask of visible faces on this layer
                BlockType?[,] mask = new BlockType?[uSize, vSize];
                for (int u = 0; u < uSize; u++)
                for (int v = 0; v < vSize; v++)
                {
                    int cx = Coord(0, layer, u, v, axis, uAx, vAx);
                    int cy = Coord(1, layer, u, v, axis, uAx, vAx);
                    int cz = Coord(2, layer, u, v, axis, uAx, vAx);
                    if (!InBounds(cx, cy, cz)) continue;
                    BlockType t = _grid[cx, cy, cz].blockType;
                    if (t == BlockType.Air) continue;
                    int nx = cx + (axis == 0 ? sign : 0);
                    int ny = cy + (axis == 1 ? sign : 0);
                    int nz = cz + (axis == 2 ? sign : 0);
                    if (!InBounds(nx, ny, nz) || _grid[nx, ny, nz].IsEmpty)
                        mask[u, v] = t;
                }

                // Greedy merge
                bool[,] visited = new bool[uSize, vSize];
                for (int u = 0; u < uSize; u++)
                for (int v = 0; v < vSize; v++)
                {
                    if (!mask[u, v].HasValue || visited[u, v]) continue;
                    BlockType t = mask[u, v].Value;
                    int w = 1;
                    while (u + w < uSize && mask[u + w, v].HasValue && mask[u + w, v].Value == t && !visited[u + w, v]) w++;
                    int h = 1;
                    bool ok = true;
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

                    // Generate quad
                    AddQuad(t, d, layer, u, v, w, h, axis, uAx, vAx, sign, normal, V, T, N, UV);
                }
            }
        }

        if (V.Count == 0) return;

        // Combine submeshes (same as before)
        var allV = new List<Vector3>();
        var allN = new List<Vector3>();
        var allUV = new List<Vector2>();
        var types = new List<BlockType>();
        var subT = new List<int[]>();
        foreach (var kv in V)
        {
            int off = allV.Count;
            var tList = T[kv.Key];
            for (int i = 0; i < tList.Count; i++) tList[i] += off;
            allV.AddRange(kv.Value);
            allN.AddRange(N[kv.Key]);
            allUV.AddRange(UV[kv.Key]);
            types.Add(kv.Key);
            subT.Add(tList.ToArray());
        }

        _mesh.subMeshCount = subT.Count;
        _mesh.SetVertices(allV);
        _mesh.SetNormals(allN);
        _mesh.SetUVs(0, allUV);
        for (int i = 0; i < subT.Count; i++) _mesh.SetTriangles(subT[i], i);
        _mesh.RecalculateBounds();
        _mf.sharedMesh = _mesh;
        var mc = GetComponent<MeshCollider>();
        if (mc) mc.sharedMesh = _mesh;

        if (blockMaterial != null)
        {
            var mats = new List<Material>();
            for (int i = 0; i < types.Count; i++)
            {
                var m = new Material(blockMaterial);
                m.SetColor("_BaseColor", BlockColor(types[i]));
                mats.Add(m);
            }
            _mr.materials = mats.ToArray();

            // Assign shared texture atlas to all materials
            if (_atlas == null) _atlas = CreateAtlas();
            foreach (var m in mats) m.SetTexture("_BaseMap", _atlas);
        }
    }

    Texture2D _atlas;

    Texture2D CreateAtlas()
    {
        int cols = 8, rows = 4, cell = 32;
        var tex = new Texture2D(cols * cell, rows * cell, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        var colors = new Color[cols * cell * rows * cell];

        for (int bt = 0; bt < 24; bt++)
        {
            int cx = bt % cols, cy = bt / cols;
            Color bc = BlockColor((BlockType)bt);
            var rng = new System.Random(bt + 42);
            for (int dy = 0; dy < cell; dy++)
            for (int dx = 0; dx < cell; dx++)
            {
                float n = (float)rng.NextDouble() * 0.15f;
                // Darken edges slightly
                float edge = 1f;
                if (dx == 0 || dy == 0 || dx == cell - 1 || dy == cell - 1) edge = 0.7f;
                int px = cx * cell + dx, py = cy * cell + dy;
                colors[py * cols * cell + px] = new Color(
                    Mathf.Clamp01(bc.r + n - 0.075f) * edge,
                    Mathf.Clamp01(bc.g + n - 0.075f) * edge,
                    Mathf.Clamp01(bc.b + n - 0.075f) * edge, 1);
            }
        }
        tex.SetPixels(colors);
        tex.Apply();
        return tex;
    }

    int Dim(int a) => a == 0 ? worldWidth : a == 1 ? worldHeight : worldDepth;

    int Coord(int outAxis, int layer, int u, int v, int axis, int uAx, int vAx)
    {
        int[] c = new int[3];
        c[axis] = layer; c[uAx] = u; c[vAx] = v;
        return c[outAxis];
    }

    void AddQuad(BlockType t, int d, int layer, int u, int v, int w, int h,
        int axis, int uAx, int vAx, int sign, Vector3 normal,
        Dictionary<BlockType, List<Vector3>> V, Dictionary<BlockType, List<int>> T,
        Dictionary<BlockType, List<Vector3>> N, Dictionary<BlockType, List<Vector2>> UV)
    {
        if (!V.ContainsKey(t)) { V[t] = new(); T[t] = new(); N[t] = new(); UV[t] = new(); }
        var vv = V[t]; var tt = T[t]; var nn = N[t]; var uv = UV[t]; int s = vv.Count;

        int fc = sign > 0 ? layer + 1 : layer;
        int[] c0 = new int[3], c1 = new int[3], c2 = new int[3], c3 = new int[3];
        c0[axis] = fc; c0[uAx] = u;     c0[vAx] = v;
        c1[axis] = fc; c1[uAx] = u + w; c1[vAx] = v;
        c2[axis] = fc; c2[uAx] = u + w; c2[vAx] = v + h;
        c3[axis] = fc; c3[uAx] = u;     c3[vAx] = v + h;

        vv.Add(new Vector3(c0[0], c0[1], c0[2]) * blockSize);
        vv.Add(new Vector3(c1[0], c1[1], c1[2]) * blockSize);
        vv.Add(new Vector3(c2[0], c2[1], c2[2]) * blockSize);
        vv.Add(new Vector3(c3[0], c3[1], c3[2]) * blockSize);
        for (int i = 0; i < 4; i++) nn.Add(normal);

        // UV atlas: 8 cols × 4 rows
        int idx = (int)t;
        float u0 = (idx % 8) / 8f, v0 = (idx / 8) / 4f;
        float u1 = u0 + 1f / 8f, v1 = v0 + 1f / 4f;
        uv.Add(new Vector2(u0, v0));
        uv.Add(new Vector2(u1, v0));
        uv.Add(new Vector2(u1, v1));
        uv.Add(new Vector2(u0, v1));

        tt.Add(s); tt.Add(s + 1); tt.Add(s + 2);
        tt.Add(s); tt.Add(s + 2); tt.Add(s + 3);
    }

    #endregion

    #region Colors

    Color BlockColor(BlockType t) => t switch
    {
        BlockType.Grass => new(.25f, .55f, .15f),
        BlockType.Dirt => new(.45f, .32f, .18f),
        BlockType.Stone => new(.50f, .50f, .55f),
        BlockType.Bedrock => new(.15f, .14f, .16f),
        BlockType.IronOre => new(.65f, .50f, .45f),
        BlockType.CopperOre => new(.80f, .45f, .25f),
        BlockType.Coal => new(.12f, .12f, .12f),
        BlockType.Sand => new(.85f, .80f, .55f),
        BlockType.Wood => new(.55f, .40f, .20f),
        BlockType.WoodPlanks => new(.65f, .50f, .30f),
        BlockType.StoneBrick => new(.55f, .55f, .58f),
        BlockType.Glass => new(.75f, .88f, .95f),
        BlockType.Water => new(.20f, .40f, .75f),
        BlockType.Snow => new(.95f, .95f, .97f),
        _ => Color.gray,
    };

    #endregion
}
