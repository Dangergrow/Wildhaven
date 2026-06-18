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

    static readonly Vector3[,] FV = {
        { new(0,1,0), new(1,1,0), new(1,1,1), new(0,1,1) }, // UP
        { new(0,0,1), new(1,0,1), new(1,0,0), new(0,0,0) }, // DOWN
        { new(1,0,1), new(1,1,1), new(1,1,0), new(1,0,0) }, // RIGHT
        { new(0,0,0), new(0,1,0), new(0,1,1), new(0,0,1) }, // LEFT
        { new(0,0,1), new(1,0,1), new(1,1,1), new(0,1,1) }, // FWD
        { new(1,0,0), new(0,0,0), new(0,1,0), new(1,1,0) }, // BACK
    };
    static readonly Vector3Int[] FO = {
        new(0,1,0), new(0,-1,0), new(1,0,0), new(-1,0,0), new(0,0,1), new(0,0,-1)
    };

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
            float h = Mathf.PerlinNoise(nx * 4 + seed * .001f, nz * 4 + seed * .001f)
                    + Mathf.PerlinNoise(nx * 8 + seed * .002f, nz * 8 + seed * .002f) * .5f
                    + Mathf.PerlinNoise(nx * 16 + seed * .003f, nz * 16 + seed * .003f) * .3f;
            int th = Mathf.Clamp((int)(h * worldHeight * .6f), 2, worldHeight - 1);
            for (int y = 0; y < worldHeight; y++)
            {
                if (y == 0) _grid[x, y, z] = new(BlockType.Bedrock);
                else if (y < th - 3) _grid[x, y, z] = new(BlockType.Stone);
                else if (y < th - 1)
                { int rn = r.Next(100); _grid[x, y, z] = new(rn < 3 ? BlockType.Coal : rn < 4 ? BlockType.IronOre : BlockType.Stone); }
                else if (y == th - 1) _grid[x, y, z] = new(BlockType.Dirt);
                else if (y == th) _grid[x, y, z] = new(BlockType.Grass);
                else _grid[x, y, z] = GridCell.Empty;
            }
        }
    }

    #endregion

    #region Mesh

    void BuildMesh()
    {
        _mesh.Clear();
        var V = new Dictionary<BlockType, List<Vector3>>();
        var T = new Dictionary<BlockType, List<int>>();
        var N = new Dictionary<BlockType, List<Vector3>>();

        for (int x = 0; x < worldWidth; x++)
        for (int y = 0; y < worldHeight; y++)
        for (int z = 0; z < worldDepth; z++)
        {
            BlockType t = _grid[x, y, z].blockType;
            if (t == BlockType.Air) continue;
            Vector3 bp = new Vector3(x, y, z) * blockSize;

            for (int d = 0; d < 6; d++)
            {
                int nx = x + FO[d].x, ny = y + FO[d].y, nz = z + FO[d].z;
                if (InBounds(nx, ny, nz) && !_grid[nx, ny, nz].IsEmpty) continue;

                if (!V.ContainsKey(t)) { V[t] = new(); T[t] = new(); N[t] = new(); }
                var vv = V[t]; var tt = T[t]; var nn = N[t]; int s = vv.Count;
                Vector3 nrm = d switch { 0=>Vector3.up, 1=>Vector3.down, 2=>Vector3.right, 3=>Vector3.left, 4=>Vector3.forward, _=>Vector3.back };
                for (int i = 0; i < 4; i++) { vv.Add(bp + FV[d, i] * blockSize); nn.Add(nrm); }
                tt.Add(s); tt.Add(s + 1); tt.Add(s + 2);
                tt.Add(s); tt.Add(s + 2); tt.Add(s + 3);
            }
        }

        if (V.Count == 0) return;

        var allV = new List<Vector3>();
        var allN = new List<Vector3>();
        var types = new List<BlockType>();
        var subT = new List<int[]>();
        foreach (var kv in V)
        {
            int off = allV.Count;
            var tList = T[kv.Key];
            for (int i = 0; i < tList.Count; i++) tList[i] += off;
            allV.AddRange(kv.Value);
            allN.AddRange(N[kv.Key]);
            types.Add(kv.Key);
            subT.Add(tList.ToArray());
        }

        _mesh.subMeshCount = subT.Count;
        _mesh.SetVertices(allV);
        _mesh.SetNormals(allN);
        for (int i = 0; i < subT.Count; i++) _mesh.SetTriangles(subT[i], i);
        _mesh.RecalculateBounds();
        _mf.sharedMesh = _mesh;

        // Update collider for raycasts
        var mc = GetComponent<MeshCollider>();
        if (mc) mc.sharedMesh = _mesh;

        // Per-type colored materials using URP/Unlit
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
        }

        Debug.Log($"[Grid] {allV.Count} verts, {types.Count} types");
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
