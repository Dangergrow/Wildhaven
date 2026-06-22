using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Spawns colonists at guaranteed valid positions above terrain.
/// No physics, no gravity — pure grid-based placement.
/// </summary>
public class ColonistSpawner : MonoBehaviour
{
    public int startingColonists = 3;
    public GameObject colonistPrefab;
    public string[] maleNames = { "Ivan", "Boris", "Dmitri" };
    public string[] femaleNames = { "Anna", "Maria", "Elena" };
    public string[] surnames = { "Petrov", "Ivanov", "Sidorov" };
    public bool gameStarted;
    public bool useTemplates;
    public CharacterCreator.ColonistTemplate[] templates;

    public List<Colonist> Colonists { get; private set; } = new List<Colonist>();
    private bool _spawnTriggered;

    void Start()
    {
        #if UNITY_EDITOR
        gameStarted = true;
        #endif
        Debug.Log("[ColonistSpawner] Starting spawn in 0.5s...");
        Invoke(nameof(DoSpawn), 0.5f);
    }

    void Update()
    {
        if (gameStarted && !_spawnTriggered)
        {
            DoSpawn();
            _spawnTriggered = true;
        }
    }

    private int _spawnIndex;

    void DoSpawn()
    {
        _spawnTriggered = true;
        if (!gameStarted) return;

        // Destroy old colonists
        foreach (Colonist c in Colonists)
            if (c != null) Destroy(c.gameObject);
        Colonists.Clear();
        _spawnIndex = 0;

        Debug.Log("[ColonistSpawner] DoSpawn called");
        GridManager grid = FindObjectOfType<GridManager>();
        if (grid == null) { Debug.LogError("[ColonistSpawner] No GridManager!"); return; }

        // Find safe spawn positions (avoid water, find land surface)
        int[] xs = { 50, 48, 52, 45, 55, 40, 60 };
        int[] zs = { 50, 52, 48, 55, 45, 60, 40 };

        for (int i = 0; i < startingColonists; i++)
        {
            // Find highest solid block at column
            int gx = xs[i], gz = zs[i];
            int topY = -1;

            for (int gy = grid.Height - 1; gy >= 1; gy--)
            {
                BlockType here = grid.GetBlock(gx, gy, gz);
                BlockType below = grid.GetBlock(gx, gy - 1, gz);

                // Found surface: air here, solid below
                if (here == BlockType.Air)
                {
                    bool solid = below == BlockType.Grass || below == BlockType.Dirt ||
                                 below == BlockType.Stone || below == BlockType.Snow ||
                                 below == BlockType.Sand || below == BlockType.Gravel;
                    if (solid) { topY = gy; break; }
                    // Water is not solid surface — keep searching higher
                }
            }

            if (topY < 0) topY = 15;
            Debug.Log($"[ColonistSpawner] Spawn #{i} at grid({gx},{topY},{gz})"); // fallback

            Vector3 worldPos = grid.GridToWorld(gx, topY, gz);
            worldPos.y += 0.8f; // above surface
            SpawnColonist(worldPos);
        }
    }

    public Colonist SpawnColonist(Vector3 pos)
    {
        GameObject go;
        if (colonistPrefab != null)
        {
            go = Instantiate(colonistPrefab, pos, Quaternion.identity);
        }
        else
        {
            // Prefab unavailable (GUID mismatch across PCs) — create programmatically
            go = new GameObject("Colonist");
            go.transform.position = pos;
            go.AddComponent<Colonist>();
            go.AddComponent<ColonistAI>();
            go.AddComponent<NeedsSystem>();
            go.AddComponent<ColonistSchedule>();
            go.AddComponent<Inventory>();
            go.AddComponent<MentalState>();
            go.AddComponent<BuildBlocker>();
            go.AddComponent<WaterInteraction>();
            go.AddComponent<ColonistGravity>();
        }
        go.transform.localScale = Vector3.one * 0.8f;
        // Ensure collider for selection raycasts
        if (go.GetComponent<Collider>() == null)
        {
            CapsuleCollider cc = go.AddComponent<CapsuleCollider>();
            cc.height = 2f; cc.radius = 0.5f; cc.center = Vector3.up;
        }
        Renderer r = go.GetComponent<Renderer>();
        if (r != null) r.material.color = Color.red;
        Colonist c = go.GetComponent<Colonist>();
        if (c == null) { Destroy(go); return null; }

        // Use template if available
        if (useTemplates && templates != null && _spawnIndex < templates.Length)
        {
            var t = templates[_spawnIndex];
            c.colonistName = t.colonistName;
            c.age = t.age;
            c.isMale = t.isMale;
            c.perk = t.perk;
            c.flaw = t.flaw;
            c.constructionSkill = t.skills[0];
            c.miningSkill = t.skills[1];
            c.cookingSkill = t.skills[2];
            c.intellectualSkill = t.skills[3];
            c.medicineSkill = t.skills[4];
            c.meleeSkill = t.skills[5];
            c.rangedSkill = t.skills[6];
            c.craftingSkill = t.skills[7];
            c.farmingSkill = t.skills[8];
            c.socialSkill = t.skills[9];
            c.animalHandlingSkill = t.skills[10];
            c.huntingSkill = t.skills[11];
            c.tradingSkill = t.skills[12];
            c.artisticSkill = t.skills[13];
        }
        else
        {
            c.isMale = Random.value > 0.5f;
            c.colonistName = (c.isMale
                ? maleNames[Random.Range(0, maleNames.Length)]
                : femaleNames[Random.Range(0, femaleNames.Length)])
                + " " + surnames[Random.Range(0, surnames.Length)];
            c.age = Random.Range(18, 50);
        }
        _spawnIndex++;
        Colonists.Add(c);

        // Give starting resources
        GiveStartingResources(c);

        return c;
    }

    /// <summary>
    /// Gives each colonist starting food, water, and basic tools.
    /// Amounts based on the Going Medieval design doc.
    /// </summary>
    void GiveStartingResources(Colonist c)
    {
        Inventory inv = c.gameObject.GetComponent<Inventory>();
        if (inv == null) inv = c.gameObject.AddComponent<Inventory>();

        // Food — 3 days worth
        inv.AddItem(ItemType.RationPack, 5);
        inv.AddItem(ItemType.Bread, 2);
        inv.AddItem(ItemType.Berries, 4);

        // Basic tools (spread across colonists)
        if (_spawnIndex == 1) inv.AddItem(ItemType.StonePickaxe, 1);
        else if (_spawnIndex == 2) inv.AddItem(ItemType.StoneAxeTool, 1);
        else inv.AddItem(ItemType.Knife, 1);

        inv.AddItem(ItemType.Bandage, 3);
    }

    public void RemoveColonist(Colonist c) => Colonists.Remove(c);

    /// <summary>Force re-spawn after world regeneration.</summary>
    public void ResetSpawn()
    {
        _spawnTriggered = false;
        _spawnIndex = 0;
        foreach (Colonist c in Colonists)
            if (c != null) Destroy(c.gameObject);
        Colonists.Clear();
        gameStarted = false;
    }
}
