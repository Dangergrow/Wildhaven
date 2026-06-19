using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Generates optimized mesh from voxel grid data.
/// Uses greedy meshing — combines adjacent identical blocks into single quads.
/// Only renders visible faces (facing air/transparent blocks).
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VoxelMeshBuilder : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Size of one block in world units")]
    public float blockSize = 1f;

    [Tooltip("Tile size in texture atlas (e.g., 0.25 = 4x4 atlas)")]
    public float atlasTileSize = 0.25f;

    [Header("References")]
    [Tooltip("Reference to GridManager for grid data")]
    public GridManager gridManager;

    [Tooltip("Material with block atlas texture")]
    public Material blockMaterial;

    // Face directions
    private static readonly Vector3Int[] FaceDirections = new Vector3Int[]
    {
        Vector3Int.up,
        Vector3Int.down,
        Vector3Int.forward,
        Vector3Int.back,
        Vector3Int.right,
        Vector3Int.left,
    };

    // Face data: normal, right axis, up axis
    private struct FaceData
    {
        public Vector3 normal;
        public Vector3 right;
        public Vector3 up;
        public int axis; // 0=X, 1=Y, 2=Z
        public bool positive;
    }

    private static readonly FaceData[] FaceDataInfo = new FaceData[]
    {
        // Y+
        new FaceData { normal = Vector3.up, right = Vector3.right, up = Vector3.forward, axis = 1, positive = true },
        // Y-
        new FaceData { normal = Vector3.down, right = Vector3.right, up = Vector3.forward, axis = 1, positive = false },
        // Z+
        new FaceData { normal = Vector3.forward, right = Vector3.right, up = Vector3.up, axis = 2, positive = true },
        // Z-
        new FaceData { normal = Vector3.back, right = Vector3.right, up = Vector3.up, axis = 2, positive = false },
        // X+
        new FaceData { normal = Vector3.right, right = Vector3.forward, up = Vector3.up, axis = 0, positive = true },
        // X-
        new FaceData { normal = Vector3.left, right = Vector3.forward, up = Vector3.up, axis = 0, positive = false },
    };

    private Mesh _mesh;
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;

    // Mesh data lists (reused to avoid allocations)
    private List<Vector3> _vertices;
    private List<int> _triangles;
    private List<Vector2> _uvs;
    private List<Color> _colors;

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();

        _mesh = new Mesh();
        _mesh.name = "VoxelWorld";
        _meshFilter.mesh = _mesh;

        if (blockMaterial != null) _meshRenderer.material = blockMaterial;

        _vertices = new List<Vector3>(65536);
        _triangles = new List<int>(65536);
        _uvs = new List<Vector2>(65536);
        _colors = new List<Color>(65536);

        if (gridManager == null) gridManager = FindObjectOfType<GridManager>();

        // Ensure MeshCollider exists for physics/raycasts
        if (GetComponent<MeshCollider>() == null)
            gameObject.AddComponent<MeshCollider>();
    }

    private void Start()
    {
        RebuildAll();
    }

    /// <summary>
    /// Rebuilds the entire mesh from grid data.
    /// </summary>
    public void RebuildAll()
    {
        if (gridManager == null)
        {
            Debug.LogError("[VoxelMeshBuilder] GridManager reference is null.");
            return;
        }

        _vertices.Clear();
        _triangles.Clear();
        _uvs.Clear();
        _colors.Clear();

        _mesh.Clear();

        int width = gridManager.Width;
        int height = gridManager.Height;
        int depth = gridManager.Depth;

        // Process each face direction
        for (int face = 0; face < 6; face++)
        {
            BuildFaceDirection(face, width, height, depth);
        }

        _mesh.SetVertices(_vertices);
        _mesh.SetTriangles(_triangles, 0);
        _mesh.SetUVs(0, _uvs);
        _mesh.SetColors(_colors);
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();

        // Refresh MeshCollider if present
        MeshCollider collider = GetComponent<MeshCollider>();
        if (collider != null) collider.sharedMesh = _mesh;

        // Bake NavMesh at runtime for AI pathfinding
        Unity.AI.Navigation.NavMeshSurface surface = GetComponent<Unity.AI.Navigation.NavMeshSurface>();
        if (surface != null)
        {
            surface.BuildNavMesh();
        }
    }

    /// <summary>
    /// Greedy meshing for a single face direction.
    /// Combines adjacent identical blocks into larger quads.
    /// </summary>
    private void BuildFaceDirection(int faceIndex, int width, int height, int depth)
    {
        FaceData data = FaceDataInfo[faceIndex];
        Vector3Int normal = FaceDirections[faceIndex];

        int axisA, axisB, axisC;
        int sizeA, sizeB, sizeC;

        if (data.axis == 0) // X faces
        {
            axisA = 1; axisB = 2; axisC = 0;
            sizeA = height; sizeB = depth; sizeC = width;
        }
        else if (data.axis == 1) // Y faces
        {
            axisA = 0; axisB = 2; axisC = 1;
            sizeA = width; sizeB = depth; sizeC = height;
        }
        else // Z faces
        {
            axisA = 0; axisB = 1; axisC = 2;
            sizeA = width; sizeB = height; sizeC = depth;
        }

        bool[,] processed = new bool[sizeA, sizeB];

        for (int c = 0; c < sizeC; c++)
        {
            // Clear processed flags for this slice
            for (int a = 0; a < sizeA; a++)
                for (int b = 0; b < sizeB; b++)
                    processed[a, b] = false;

            for (int a = 0; a < sizeA; a++)
            {
                for (int b = 0; b < sizeB; b++)
                {
                    if (processed[a, b]) continue;

                    Vector3Int pos = IndexToWorld(a, b, c, axisA, axisB, axisC);

                    if (!ShouldRenderFace(pos, normal)) continue;

                    BlockType blockType = gridManager.GetBlock(pos.x, pos.y, pos.z);

                    // Try to expand the quad along axisA (width)
                    int quadWidth = 1;
                    for (int aw = a + 1; aw < sizeA; aw++)
                    {
                        Vector3Int testPos = IndexToWorld(aw, b, c, axisA, axisB, axisC);
                        if (processed[aw, b] || !ShouldRenderFace(testPos, normal) ||
                            gridManager.GetBlock(testPos.x, testPos.y, testPos.z) != blockType)
                            break;
                        quadWidth++;
                    }

                    // Try to expand along axisB (height of quad)
                    int quadHeight = 1;
                    bool canExpand = true;
                    for (int bh = b + 1; bh < sizeB && canExpand; bh++)
                    {
                        for (int aw = a; aw < a + quadWidth; aw++)
                        {
                            Vector3Int testPos = IndexToWorld(aw, bh, c, axisA, axisB, axisC);
                            if (processed[aw, bh] || !ShouldRenderFace(testPos, normal) ||
                                gridManager.GetBlock(testPos.x, testPos.y, testPos.z) != blockType)
                            {
                                canExpand = false;
                                break;
                            }
                        }
                        if (canExpand) quadHeight++;
                    }

                    // Mark as processed
                    for (int aw = a; aw < a + quadWidth; aw++)
                        for (int bh = b; bh < b + quadHeight; bh++)
                            processed[aw, bh] = true;

                    // Build the quad
                    AddQuad(data, pos, quadWidth, quadHeight, blockType);
                }
            }
        }
    }

    /// <summary>
    /// Determines if a face at the given position and normal should be rendered.
    /// Renders if this block is solid and the adjacent block in normal direction is empty or transparent.
    /// </summary>
    private bool ShouldRenderFace(Vector3Int pos, Vector3Int normal)
    {
        BlockType blockType = gridManager.GetBlock(pos.x, pos.y, pos.z);
        if (blockType == BlockType.Air) return false;

        Vector3Int neighborPos = pos + normal;
        BlockType neighborType = gridManager.GetBlock(neighborPos.x, neighborPos.y, neighborPos.z);

        // Render face if neighbor is air or transparent block
        return neighborType == BlockType.Air;
    }

    /// <summary>
    /// Adds a quad (two triangles) to the mesh.
    /// </summary>
    private void AddQuad(FaceData data, Vector3Int gridPos, int width, int height, BlockType blockType)
    {
        int vertexStart = _vertices.Count;
        float s = blockSize;

        // Calculate world position of the face origin
        Vector3 origin = new Vector3(gridPos.x * s, gridPos.y * s, gridPos.z * s);

        // For positive-facing normals, offset by blockSize in that direction
        if (data.positive)
        {
            origin += data.normal * s;
        }

        // Four corners of the quad
        Vector3 p0 = origin;
        Vector3 p1 = origin + data.right * width * s;
        Vector3 p2 = origin + data.right * width * s + data.up * height * s;
        Vector3 p3 = origin + data.up * height * s;

        _vertices.Add(p0);
        _vertices.Add(p1);
        _vertices.Add(p2);
        _vertices.Add(p3);

        // Two triangles
        _triangles.Add(vertexStart);
        _triangles.Add(vertexStart + 1);
        _triangles.Add(vertexStart + 2);
        _triangles.Add(vertexStart);
        _triangles.Add(vertexStart + 2);
        _triangles.Add(vertexStart + 3);

        // UV mapping
        int tileIndex = (int)blockType;
        float tileU = (tileIndex % 4) * atlasTileSize;
        float tileV = 1f - (tileIndex / 4 + 1) * atlasTileSize;

        _uvs.Add(new Vector2(tileU, tileV + atlasTileSize));
        _uvs.Add(new Vector2(tileU + atlasTileSize * width, tileV + atlasTileSize));
        _uvs.Add(new Vector2(tileU + atlasTileSize * width, tileV));
        _uvs.Add(new Vector2(tileU, tileV));

        // Vertex colors — slight tint based on face direction for depth
        float brightness = 0.8f + 0.2f * Vector3.Dot(data.normal, Vector3.up);
        Color faceColor = new Color(brightness, brightness, brightness, 1f);
        _colors.Add(faceColor);
        _colors.Add(faceColor);
        _colors.Add(faceColor);
        _colors.Add(faceColor);
    }

    /// <summary>
    /// Converts 2D coordinates plus layer index to 3D world position.
    /// </summary>
    private Vector3Int IndexToWorld(int a, int b, int c, int axisA, int axisB, int axisC)
    {
        Vector3Int result = Vector3Int.zero;
        result[axisA] = a;
        result[axisB] = b;
        result[axisC] = c;
        return result;
    }
}
