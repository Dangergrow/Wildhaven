using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>CLI test runner for Unity batchmode. Execute via:
/// Unity.exe -batchmode -quit -projectPath . -executeMethod TestRunner.RunAllTests -logFile test.log</summary>
public static class TestRunner
{
    static void Log(string msg) { Debug.Log($"[TEST] {msg}"); }

    /// <summary>Run all automated tests and output results to log.</summary>
    [MenuItem("Wildhaven/Run All Tests")]
    public static void RunAllTests()
    {
        Log("=== Wildhaven Test Suite ===");
        bool ok = TestWorldGeneration();
        Log($"World generation: {(ok ? "PASS" : "FAIL")}");
        Log($"=== Done ===");
    }

    /// <summary>Generate world, verify mesh, test save/load.</summary>
    static bool TestWorldGeneration()
    {
        // Find or create GridManager
        var gm = Object.FindObjectOfType<GridManager>();
        if (gm == null)
        {
            var world = new GameObject("TestWorld");
            gm = world.AddComponent<GridManager>();
        }
        gm.worldWidth = 50; gm.worldHeight = 32; gm.worldDepth = 50;
        gm.seed = 42;

        // Initialize grid (Awake not called in Editor mode)
        gm.InitGrid();
        gm.GenerateTerrain();
        gm.BuildAllChunks();

        // Verify mesh exists (check chunks, not main GridManager)
        int totalVerts = 0;
        int totalChunks = 0;
        foreach (Transform child in gm.transform)
        {
            var mf2 = child.GetComponent<MeshFilter>();
            if (mf2 != null && mf2.sharedMesh != null)
            {
                totalVerts += mf2.sharedMesh.vertexCount;
                totalChunks++;
            }
        }
        if (totalVerts == 0) { Log("ERROR: No mesh generated!"); return false; }
        Log($"Mesh: {totalVerts} total verts across {totalChunks} chunks");

        // Test raycast
        var ray = new Ray(new Vector3(25, 50, 25), Vector3.down);
        var hit = gm.RaycastGrid(ray);
        if (hit == null) { Log("ERROR: Raycast returned null!"); return false; }
        Log($"Raycast hit: {hit.Value}");

        // Test save/load
        gm.SaveWorld();
        if (!gm.HasSave) { Log("ERROR: Save failed!"); return false; }
        gm.LoadWorld();
        Log("Save/Load: OK");

        // Test block placement
        gm.SetBlock(25, 15, 25, BlockType.WoodPlanks);
        if (gm.GetBlock(25, 15, 25) != BlockType.WoodPlanks) { Log("ERROR: SetBlock failed!"); return false; }
        gm.RemoveBlock(25, 15, 25);
        if (gm.GetBlock(25, 15, 25) != BlockType.Air) { Log("ERROR: RemoveBlock failed!"); return false; }
        Log("Block placement: OK");

        // Cleanup test world save
        var savePath = Path.Combine(Application.persistentDataPath, "world.sav");
        if (File.Exists(savePath)) File.Delete(savePath);

        return true;
    }
}
