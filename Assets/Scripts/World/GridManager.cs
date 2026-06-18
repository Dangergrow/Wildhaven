using UnityEngine;

/// <summary>
/// Manages the entire voxel grid for the world.
/// Handles block placement, removal, and querying.
/// </summary>
public class GridManager : MonoBehaviour
{
    [Header("World Size")]
    [Tooltip("Width of the world in blocks (X axis)")]
    public int worldWidth = 100;

    [Tooltip("Height of the world in blocks (Y axis)")]
    public int worldHeight = 32;

    [Tooltip("Depth of the world in blocks (Z axis)")]
    public int worldDepth = 100;

    [Header("Block Settings")]
    [Tooltip("Size of each block in world units")]
    public float blockSize = 1f;

    [Tooltip("Default block material")]
    public Material blockMaterial;

    [Header("Generation")]
    [Tooltip("Seed for terrain generation. 0 = random")]
    public int seed;

    // Main 3D grid [x, y, z]
    private GridCell[,,] _grid;

    // Mesh data per chunk
    private Mesh _chunkMesh;

    // Public accessors
    public int Width => worldWidth;
    public int Height => worldHeight;
    public int Depth => worldDepth;
    public float BlockSize => blockSize;

    private void Awake()
    {
        _grid = new GridCell[worldWidth, worldHeight, worldDepth];
        _chunkMesh = new Mesh();

        GenerateTerrain();
        BuildMesh();
    }

    /// <summary>
    /// Sets a block at the given world position.
    /// </summary>
    public void SetBlock(int x, int y, int z, BlockType type)
    {
        if (IsInBounds(x, y, z))
        {
            _grid[x, y, z] = new GridCell(type);
            BuildMesh();
        }
    }

    /// <summary>
    /// Removes a block (sets to Air).
    /// </summary>
    public void RemoveBlock(int x, int y, int z)
    {
        SetBlock(x, y, z, BlockType.Air);
    }

    /// <summary>
    /// Gets the block type at the given position.
    /// </summary>
    public BlockType GetBlock(int x, int y, int z)
    {
        if (!IsInBounds(x, y, z)) return BlockType.Air;
        return _grid[x, y, z].blockType;
    }

    /// <summary>
    /// Checks if coordinates are within world bounds.
    /// </summary>
    public bool IsInBounds(int x, int y, int z)
    {
        return x >= 0 && x < worldWidth &&
               y >= 0 && y < worldHeight &&
               z >= 0 && z < worldDepth;
    }

    /// <summary>
    /// Converts world position to grid coordinates.
    /// </summary>
    public Vector3Int WorldToGrid(Vector3 worldPos)
    {
        return new Vector3Int(
            Mathf.FloorToInt(worldPos.x / blockSize),
            Mathf.FloorToInt(worldPos.y / blockSize),
            Mathf.FloorToInt(worldPos.z / blockSize)
        );
    }

    /// <summary>
    /// Converts grid coordinates to world position (center of block).
    /// </summary>
    public Vector3 GridToWorld(int x, int y, int z)
    {
        return new Vector3(
            x * blockSize + blockSize * 0.5f,
            y * blockSize + blockSize * 0.5f,
            z * blockSize + blockSize * 0.5f
        );
    }

    /// <summary>
    /// Generates procedural terrain using Perlin noise.
    /// </summary>
    private void GenerateTerrain()
    {
        if (seed == 0) seed = Random.Range(1, 1000000);
        var random = new System.Random(seed);

        for (int x = 0; x < worldWidth; x++)
        {
            for (int z = 0; z < worldDepth; z++)
            {
                float nx = (float)x / worldWidth;
                float nz = (float)z / worldDepth;

                // Base terrain height using Perlin noise
                float baseHeight = Mathf.PerlinNoise(nx * 4f + seed * 0.001f, nz * 4f + seed * 0.001f);
                float hillHeight = Mathf.PerlinNoise(nx * 8f + seed * 0.002f, nz * 8f + seed * 0.002f) * 0.5f;
                float mountainHeight = Mathf.PerlinNoise(nx * 16f + seed * 0.003f, nz * 16f + seed * 0.003f) * 0.3f;

                int terrainHeight = Mathf.FloorToInt((baseHeight + hillHeight + mountainHeight) * (worldHeight * 0.6f));
                terrainHeight = Mathf.Clamp(terrainHeight, 2, worldHeight - 1);

                for (int y = 0; y < worldHeight; y++)
                {
                    if (y == 0)
                    {
                        // Bedrock at bottom
                        _grid[x, y, z] = new GridCell(BlockType.Bedrock);
                    }
                    else if (y < terrainHeight - 3)
                    {
                        // Deep stone
                        _grid[x, y, z] = new GridCell(BlockType.Stone);
                    }
                    else if (y < terrainHeight - 1)
                    {
                        // Stone with possible ores
                        BlockType type = BlockType.Stone;
                        if (random.Next(100) < 3) type = BlockType.Coal;
                        else if (random.Next(100) < 1) type = BlockType.IronOre;
                        _grid[x, y, z] = new GridCell(type);
                    }
                    else if (y == terrainHeight - 1)
                    {
                        // Dirt layer
                        _grid[x, y, z] = new GridCell(BlockType.Dirt);
                    }
                    else if (y == terrainHeight)
                    {
                        // Grass on top
                        _grid[x, y, z] = new GridCell(BlockType.Grass);
                    }
                    else
                    {
                        // Air above ground
                        _grid[x, y, z] = GridCell.Empty;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Builds the combined mesh for the entire grid.
    /// </summary>
    private void BuildMesh()
    {
        // Placeholder for mesh generation — will be expanded
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null) meshFilter = gameObject.AddComponent<MeshFilter>();

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null) meshRenderer = gameObject.AddComponent<MeshRenderer>();

        if (blockMaterial != null) meshRenderer.material = blockMaterial;

        // For now: create a simple flat ground plane as placeholder
        // Full greedy mesh with face culling will be implemented in Step 2
        _chunkMesh.Clear();
        _chunkMesh.name = "WorldMesh";
        meshFilter.mesh = _chunkMesh;
    }
}
