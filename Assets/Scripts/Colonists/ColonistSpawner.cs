using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Spawns colonists on terrain surface by querying GridManager directly.
/// Finds the highest solid block with air above — no physics needed.
/// </summary>
public class ColonistSpawner : MonoBehaviour
{
    public int startingColonists = 3;
    public GameObject colonistPrefab;
    public Vector3Int spawnOrigin = new Vector3Int(50, 0, 50); // grid coords
    public int searchRadius = 12;

    public string[] maleNames = { "Ivan", "Boris", "Dmitri", "Alexei", "Sergei" };
    public string[] femaleNames = { "Anna", "Maria", "Elena", "Olga", "Natasha" };
    public string[] surnames = { "Petrov", "Ivanov", "Sidorov", "Kuznetsov", "Smirnov" };

    public List<Colonist> Colonists { get; private set; } = new List<Colonist>();

    void Start() => Invoke(nameof(DoSpawn), 1f);

    void DoSpawn()
    {
        for (int i = 0; i < startingColonists; i++)
            SpawnColonist(FindSurface());
    }

    Vector3 FindSurface()
    {
        GridManager grid = FindObjectOfType<GridManager>();
        if (grid == null) return new Vector3(50, 20, 50);

        for (int attempt = 0; attempt < 500; attempt++)
        {
            int gx = spawnOrigin.x + Random.Range(-searchRadius, searchRadius + 1);
            int gz = spawnOrigin.z + Random.Range(-searchRadius, searchRadius + 1);

            // Scan from top down to find the FIRST air block above solid surface
            for (int gy = grid.Height - 2; gy >= 1; gy--)
            {
                BlockType below = grid.GetBlock(gx, gy - 1, gz);
                BlockType here = grid.GetBlock(gx, gy, gz);
                BlockType above = grid.GetBlock(gx, gy + 1, gz);

                bool solidBelow = below == BlockType.Grass || below == BlockType.Dirt ||
                                  below == BlockType.Stone || below == BlockType.Snow ||
                                  below == BlockType.Sand || below == BlockType.Gravel ||
                                  below == BlockType.WoodPlanks || below == BlockType.StoneBrick;

                bool isAirHere = here == BlockType.Air;
                bool isAirAbove = above == BlockType.Air;

                // Found: two air blocks above a solid block — spawn at the lower air block
                if (solidBelow && isAirHere && isAirAbove)
                {
                    Vector3 pos = grid.GridToWorld(gx, gy, gz);
                    pos.y += 0.5f; // nudge up so capsule sits ON surface, not in it
                    return pos;
                }
            }
        }

        Debug.LogError("[ColonistSpawner] No surface found");
        return new Vector3(50, 20, 50);
    }

    public Colonist SpawnColonist(Vector3 pos)
    {
        if (colonistPrefab == null) { Debug.LogError("[ColonistSpawner] No prefab!"); return null; }
        GameObject go = Instantiate(colonistPrefab, pos, Quaternion.identity);
        // scale 0.8 to fit inside 1-block space without clipping
        go.transform.localScale = Vector3.one * 0.8f;
        // Make visible
        Renderer r = go.GetComponent<Renderer>();
        if (r != null) { r.material = new Material(r.material) { color = Color.red }; }
        Colonist c = go.GetComponent<Colonist>();
        if (c == null) { Destroy(go); return null; }

        c.isMale = Random.value > 0.5f;
        c.colonistName = (c.isMale
            ? maleNames[Random.Range(0, maleNames.Length)]
            : femaleNames[Random.Range(0, femaleNames.Length)])
            + " " + surnames[Random.Range(0, surnames.Length)];
        c.age = Random.Range(18, 50);
        Colonists.Add(c);
        Debug.Log($"[ColonistSpawner] {c.colonistName} at grid pos near ({Mathf.RoundToInt(pos.x)}, {Mathf.RoundToInt(pos.y)}, {Mathf.RoundToInt(pos.z)})");
        return c;
    }

    public void RemoveColonist(Colonist c) => Colonists.Remove(c);
}
