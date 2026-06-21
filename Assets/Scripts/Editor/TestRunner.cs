using UnityEngine;
using UnityEditor;
using System.IO;

public static class TestRunner
{
    static void Log(string msg) { Debug.Log($"[TEST] {msg}"); }

    [MenuItem("Wildhaven/Run All Tests")]
    public static void RunAllTests()
    {
        Log("=== Wildhaven ===");
        Log(TestWorldGen() ? "World: PASS" : "World: FAIL");
        Log(TestSave() ? "Save: PASS" : "Save: FAIL");
        Log(TestBlocks() ? "Blocks: PASS" : "Blocks: FAIL");
        Log(TestEconomy() ? "Econ: PASS" : "Econ: FAIL");
        Log(TestColonist() ? "Colonist: PASS" : "Colonist: FAIL");
        Log("=== Done ===");
    }

    static GridManager MakeWorld()
    {
        var gm = Object.FindAnyObjectByType<GridManager>();
        if (gm == null) { var w = new GameObject("TW"); gm = w.AddComponent<GridManager>(); }
        gm.worldWidth = 50; gm.worldHeight = 32; gm.worldDepth = 50; gm.seed = 42;
        gm.InitGrid(); gm.GenerateTerrain(); gm.BuildAllChunks();
        return gm;
    }

    static bool TestWorldGen()
    {
        var gm = MakeWorld();
        int v = 0;
        foreach (Transform c in gm.transform) { var mf = c.GetComponent<MeshFilter>(); if (mf && mf.sharedMesh) v += mf.sharedMesh.vertexCount; }
        return v > 0;
    }

    static bool TestSave()
    {
        var gm = MakeWorld();
        gm.SaveWorld();
        bool ok = gm.HasSave;
        if (ok) gm.LoadWorld();
        return ok;
    }

    static bool TestBlocks()
    {
        var gm = MakeWorld();
        gm.SetBlock(25, 15, 25, BlockType.GoldOre);
        if (gm.GetBlock(25, 15, 25) != BlockType.GoldOre) return false;
        gm.RemoveBlock(25, 15, 25);
        return gm.GetBlock(25, 15, 25) == BlockType.Air;
    }

    static bool TestEconomy()
    {
        var go = new GameObject("TE");
        var e = go.AddComponent<EconomyManager>();
        e.ModifyMoney(500);
        bool ok = e.TotalCopper >= 500 && e.GetBuyPrice(ItemType.Bread, 0) > 0;
        Object.DestroyImmediate(go);
        return ok;
    }

    static bool TestColonist()
    {
        var gm = MakeWorld();
        var colGo = new GameObject("TC");
        var col = colGo.AddComponent<Colonist>();
        col.colonistName = "Tester"; col.health = 100; col.maxHealth = 100; col.currentState = ColonistState.Idle;
        var ai = colGo.AddComponent<ColonistAI>();
        var d = new GameObject("TD").AddComponent<DayCycle>(); d.gameSpeed = 1f;
        colGo.transform.position = gm.GridToWorld(15, 12, 15);

        CallAwake(ai);
        ai.currentOrder = ColonistAI.OrderType.Move;
        ai.orderTarget = gm.GridToWorld(18, 12, 15);

        var start = colGo.transform.position;
        for (int f = 0; f < 60; f++) CallUpdate(ai);
        float moved = Vector3.Distance(start, colGo.transform.position);

        Object.DestroyImmediate(colGo); Object.DestroyImmediate(d.gameObject);
        return moved > 0.5f;
    }

    static void CallAwake(object o) { o.GetType().GetMethod("Awake", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(o, null); }
    static void CallUpdate(object o) { o.GetType().GetMethod("Update", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(o, null); }
}
