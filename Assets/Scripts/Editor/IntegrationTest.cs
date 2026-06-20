using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

/// <summary>Full integration test — simulates Play mode and tests colonist movement.</summary>
public static class IntegrationTest
{
    [MenuItem("Wildhaven/Run Integration Test")]
    public static void Run()
    {
        Debug.Log("[INTEGRATION] Starting...");
        EditorApplication.isPlaying = true;
        EditorApplication.update += OnUpdate;
    }

    static int _frame;
    static GameObject _world, _gameMgr, _cameraObj;
    static Colonist _testColonist;

    static void OnUpdate()
    {
        _frame++;
        if (_frame == 1)
        {
            SetupScene();
            return;
        }
        if (_frame == 10)
        {
            // Spawn colonist and issue order
            SpawnAndOrder();
        }
        if (_frame > 10 && _testColonist != null)
        {
            var ai = _testColonist.GetComponent<ColonistAI>();
            var startPos = _testColonist.transform.position;
            // Run 30 more frames
            if (_frame > 40)
            {
                var endPos = _testColonist.transform.position;
                float moved = Vector3.Distance(startPos, endPos);
                Debug.Log($"[INTEGRATION] Colonist moved {moved:F2} units in 30 frames");
                EditorApplication.isPlaying = false;
                EditorApplication.update -= OnUpdate;
            }
        }
    }

    static void SetupScene()
    {
        // World
        _world = new GameObject("World");
        var gm = _world.AddComponent<GridManager>();
        gm.worldWidth = 30; gm.worldHeight = 32; gm.worldDepth = 30;
        gm.seed = 42;
        gm.InitGrid();
        gm.GenerateTerrain();
        gm.BuildAllChunks();

        // Camera
        _cameraObj = new GameObject("Camera");
        var cam = _cameraObj.AddComponent<Camera>();
        cam.tag = "MainCamera";
        _cameraObj.transform.position = new Vector3(15, 30, 15);
        _cameraObj.transform.rotation = Quaternion.Euler(70, 45, 0);
        _cameraObj.AddComponent<CameraController>();

        // GameManager for DayCycle etc.
        _gameMgr = new GameObject("GameManager");
        _gameMgr.AddComponent<DayCycle>();
        var spawner = _gameMgr.AddComponent<ColonistSpawner>();
        var build = _gameMgr.AddComponent<BuildManager>();
        build.enabled = false;
        var select = _gameMgr.AddComponent<SelectionManager>();
    }

    static void SpawnAndOrder()
    {
        var spawner = Object.FindFirstObjectByType<ColonistSpawner>();
        var gm = Object.FindFirstObjectByType<GridManager>();
        if (spawner == null || gm == null) { Debug.LogError("[INTEGRATION] Spawner or GM missing!"); return; }

        // Find a good spawn surface
        Vector3 spawnPos = Vector3.zero;
        for (int x = 10; x < 20; x++)
        for (int z = 10; z < 20; z++)
        {
            for (int y = gm.Height - 1; y > 0; y--)
            {
                if (gm.GetBlock(x, y, z) == BlockType.Air && gm.GetBlock(x, y - 1, z) == BlockType.Grass)
                {
                    spawnPos = gm.GridToWorld(x, y, z);
                    goto found;
                }
            }
        }
        found:
        if (spawnPos == Vector3.zero) { Debug.LogError("[INTEGRATION] No spawn surface!"); return; }

        // Create colonist directly (bypass prefab)
        var colGo = new GameObject("TestColonist");
        colGo.transform.position = spawnPos + Vector3.up * 0.5f;
        var col = colGo.AddComponent<Colonist>();
        col.colonistName = "Tester";
        col.health = col.maxHealth = 100;
        var ai = colGo.AddComponent<ColonistAI>();
        var needs = colGo.AddComponent<NeedsSystem>();
        colGo.AddComponent<CapsuleCollider>();

        _testColonist = col;

        // Give Move order to a nearby position
        Vector3 target = spawnPos + new Vector3(3, 0, 2);
        Debug.Log($"[INTEGRATION] Ordering colonist from {spawnPos} to {target}");
        ai.GiveOrder(ColonistAI.OrderType.Move, target);
    }
}
