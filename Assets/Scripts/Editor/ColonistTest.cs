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

        // Create colonist manually (prefab GUIDs may not match across PCs)
        var colGo = new GameObject("TestColonist");
        colGo.transform.position = spawnPos + Vector3.up * 0.5f;
        var col = colGo.AddComponent<Colonist>();
        colGo.AddComponent<ColonistAI>();
        colGo.AddComponent<NeedsSystem>();
        colGo.AddComponent<ColonistGravity>();
        colGo.AddComponent<WaterInteraction>();
        colGo.AddComponent<BuildBlocker>();
        colGo.AddComponent<MentalState>();
        colGo.AddComponent<Inventory>();
        // Skip ALL other components — test minimal movement first
        colGo.AddComponent<CapsuleCollider>();
        col.colonistName = "Tester";
        col.health = col.maxHealth = 100;
        col.hunger = 50; col.fatigue = 0; col.mood = 50;
        var ai = colGo.GetComponent<ColonistAI>();
        // Awake doesn't run in Editor mode — call manually
        CallMethod(ai, "Awake");
        CallMethod(colGo.GetComponent<NeedsSystem>(), "Awake");
        CallMethod(colGo.GetComponent<ColonistGravity>(), "Awake");
        CallMethod(colGo.GetComponent<WaterInteraction>(), "Awake");
        CallMethod(colGo.GetComponent<BuildBlocker>(), "Start");
        CallMethod(colGo.GetComponent<MentalState>(), "Awake");
        CallMethod(colGo.GetComponent<Inventory>(), "Awake");

        // Cap collider for selection
        var cap = colGo.GetComponent<CapsuleCollider>();
        if (cap == null) cap = colGo.AddComponent<CapsuleCollider>();

        // Setup DayCycle (needed by AI)
        var dayGo = new GameObject("DayCycle");
        var day = dayGo.AddComponent<DayCycle>();
        day.gameSpeed = 0f; // SIMULATE PAUSE — does colonist still move?

        // Prefab components already initialized via Instantiate
        col.currentState = ColonistState.Idle;
        ai = colGo.GetComponent<ColonistAI>();

        Vector3 startPos = colGo.transform.position;

        // Force order by directly setting fields (bypass state checks)
        Vector3 target = spawnPos + new Vector3(3, 0, 2);
        ai.currentOrder = ColonistAI.OrderType.Move;
        ai.orderTarget = target;
        Debug.Log($"[CTEST] Order forced: Move to {target}");

        // Simulate frames — call Update on key components
        for (int f = 0; f < 120; f++)
        {
            CallMethod(ai, "Update");
            CallMethod(colGo.GetComponent<ColonistGravity>(), "Update");
            CallMethod(colGo.GetComponent<WaterInteraction>(), "Update");
            CallMethod(colGo.GetComponent<BuildBlocker>(), "Update");
            CallMethod(colGo.GetComponent<MentalState>(), "Update");
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
