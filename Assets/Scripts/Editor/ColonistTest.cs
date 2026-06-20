using UnityEngine;
using UnityEditor;
using System.Reflection;

/// <summary>Manual frame simulation test — no Play mode needed.</summary>
public static class ColonistTest
{
    [MenuItem("Wildhaven/Test Colonist Move")]
    public static void Run()
    {
        Debug.Log("[CTEST] === Colonist Movement Test ===");

        // Setup world
        var world = new GameObject("TestWorld");
        var gm = world.AddComponent<GridManager>();
        gm.worldWidth = 20; gm.worldHeight = 20; gm.worldDepth = 20;
        gm.seed = 99;
        gm.InitGrid();
        gm.GenerateTerrain();
        gm.BuildAllChunks();

        // Find walkable surface within bounds
        Vector3 spawnPos = Vector3.zero;
        for (int x = 5; x < 15; x++)
        for (int z = 5; z < 15; z++)
        for (int y = gm.Height - 2; y > 2; y--)
        {
            if (gm.GetBlock(x, y, z) == BlockType.Air && gm.GetBlock(x, y - 1, z) == BlockType.Grass)
            {
                spawnPos = gm.GridToWorld(x, y, z);
                Debug.Log($"[CTEST] Spawn surface at grid({x},{y},{z}) world({spawnPos})");
                goto spawnFound;
            }
        }
        spawnFound:

        if (spawnPos == Vector3.zero)
        {
            Debug.Log("[CTEST] FAIL: No spawn surface found!");
            return;
        }

        // Create colonist
        var colGo = new GameObject("TestColonist");
        colGo.transform.position = spawnPos + Vector3.up * 0.5f;

        // Add components manually
        var col = colGo.AddComponent<Colonist>();
        col.colonistName = "Tester";
        col.health = col.maxHealth = 100;
        col.hunger = 50;
        col.fatigue = 0;
        col.mood = 50;

        var ai = colGo.AddComponent<ColonistAI>();
        var needs = colGo.AddComponent<NeedsSystem>();

        // Add collider for physics raycasts
        var cap = colGo.AddComponent<CapsuleCollider>();
        cap.radius = 0.3f;
        cap.height = 1.5f;

        // Setup DayCycle (needed by AI)
        var dayGo = new GameObject("DayCycle");
        var day = dayGo.AddComponent<DayCycle>();
        day.gameSpeed = 1f;

        // Manually call Awake-like init
        CallMethod(ai, "Awake");
        CallMethod(needs, "Awake");
        col.currentState = ColonistState.Idle;

        Vector3 startPos = colGo.transform.position;

        // Issue Move order
        Vector3 target = spawnPos + new Vector3(3, 0, 2);
        bool ordered = ai.GiveOrder(ColonistAI.OrderType.Move, target);
        Debug.Log($"[CTEST] Order issued: {ordered}  target={target}");

        if (!ordered) { Debug.Log("[CTEST] FAIL: Order rejected!"); return; }

        // Simulate 120 frames (2 seconds)
        for (int f = 0; f < 120; f++)
        {
            CallMethod(ai, "Update");
        }

        Vector3 endPos = colGo.transform.position;
        float moved = Vector3.Distance(startPos, endPos);
        Debug.Log($"[CTEST] Start: {startPos}  End: {endPos}  Moved: {moved:F2}");

        if (moved > 0.5f)
            Debug.Log("[CTEST] PASS: Colonist moved!");
        else
            Debug.Log("[CTEST] FAIL: Colonist didn't move!");

        // Cleanup
        Object.DestroyImmediate(world);
        Object.DestroyImmediate(colGo);
        Object.DestroyImmediate(dayGo);
    }

    static void CallMethod(object obj, string method)
    {
        var m = obj.GetType().GetMethod(method, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        if (m != null) m.Invoke(obj, null);
    }
}
