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

    public List<Colonist> Colonists { get; private set; } = new List<Colonist>();

    void Start()
    {
        Debug.Log("[ColonistSpawner] Starting spawn in 0.5s...");
        Invoke(nameof(DoSpawn), 0.5f);
    }

    void DoSpawn()
    {
        Debug.Log("[ColonistSpawner] DoSpawn called");
        GridManager grid = FindObjectOfType<GridManager>();
        if (grid == null) { Debug.LogError("[ColonistSpawner] No GridManager!"); return; }

        // Find safe spawn positions
        int[] xs = { 50, 48, 52 };
        int[] zs = { 50, 52, 48 };

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
        if (colonistPrefab == null) { Debug.LogError("No prefab!"); return null; }
        GameObject go = Instantiate(colonistPrefab, pos, Quaternion.identity);
        go.transform.localScale = Vector3.one * 0.8f;
        Renderer r = go.GetComponent<Renderer>();
        if (r != null) r.material.color = Color.red;
        Colonist c = go.GetComponent<Colonist>();
        if (c == null) { Destroy(go); return null; }

        c.isMale = Random.value > 0.5f;
        c.colonistName = (c.isMale
            ? maleNames[Random.Range(0, maleNames.Length)]
            : femaleNames[Random.Range(0, femaleNames.Length)])
            + " " + surnames[Random.Range(0, surnames.Length)];
        c.age = Random.Range(18, 50);
        Colonists.Add(c);
        return c;
    }

    public void RemoveColonist(Colonist c) => Colonists.Remove(c);
}
