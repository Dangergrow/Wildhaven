using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Spawns initial colonists and manages colonist list.
/// </summary>
public class ColonistSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public int startingColonists = 3;
    public GameObject colonistPrefab;
    public Vector3 spawnCenter = new Vector3(50f, 30f, 50f);
    public float spawnRadius = 8f;

    [Header("Random Name Pool")]
    public string[] maleNames = new string[] { "Ivan", "Boris", "Dmitri", "Alexei", "Sergei", "Nikolai", "Viktor" };
    public string[] femaleNames = new string[] { "Anna", "Maria", "Elena", "Olga", "Natasha", "Svetlana", "Irina" };
    public string[] surnames = new string[] { "Petrov", "Ivanov", "Sidorov", "Kuznetsov", "Smirnov", "Volkov", "Morozov" };

    public List<Colonist> Colonists { get; private set; } = new List<Colonist>();

    private void Start()
    {
        SpawnInitialColonists();
    }

    private void SpawnInitialColonists()
    {
        for (int i = 0; i < startingColonists; i++)
        {
            SpawnColonist(GetRandomSpawnPosition());
        }
    }

    public Colonist SpawnColonist(Vector3 position)
    {
        if (colonistPrefab == null)
        {
            Debug.LogError("[ColonistSpawner] No colonist prefab assigned!");
            return null;
        }

        GameObject go = Instantiate(colonistPrefab, position, Quaternion.identity);
        Colonist colonist = go.GetComponent<Colonist>();
        if (colonist == null)
        {
            Debug.LogError("[ColonistSpawner] Prefab has no Colonist component!");
            Destroy(go);
            return null;
        }

        colonist.isMale = Random.value > 0.5f;
        colonist.colonistName = GenerateName(colonist.isMale);
        colonist.age = Random.Range(18, 50);
        colonist.constructionSkill = Random.Range(0, 8);
        colonist.miningSkill = Random.Range(0, 8);
        colonist.cookingSkill = Random.Range(0, 6);
        colonist.intellectualSkill = Random.Range(0, 6);
        colonist.medicineSkill = Random.Range(0, 4);
        colonist.meleeSkill = Random.Range(0, 6);
        colonist.rangedSkill = Random.Range(0, 6);
        colonist.craftingSkill = Random.Range(0, 6);
        colonist.farmingSkill = Random.Range(0, 8);
        colonist.socialSkill = Random.Range(0, 6);
        colonist.animalHandlingSkill = Random.Range(0, 4);
        colonist.huntingSkill = Random.Range(0, 6);
        colonist.tradingSkill = Random.Range(0, 4);
        colonist.artisticSkill = Random.Range(0, 4);
        colonist.perk = Random.value < 0.3f ? (Perk)Random.Range(1, System.Enum.GetValues(typeof(Perk)).Length) : Perk.None;
        colonist.flaw = Random.value < 0.3f ? (Flaw)Random.Range(1, System.Enum.GetValues(typeof(Flaw)).Length) : Flaw.None;

        Colonists.Add(colonist);
        Debug.Log($"[ColonistSpawner] Spawned {colonist.colonistName} ({colonist.age}) at {position}");
        return colonist;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        GridManager grid = FindObjectOfType<GridManager>();
        for (int attempt = 0; attempt < 50; attempt++)
        {
            Vector2 circle = Random.insideUnitCircle * spawnRadius;
            Vector3 origin = spawnCenter + new Vector3(circle.x, 60f, circle.y);

            if (!Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 80f))
                continue;

            // Check what block is at the hit point — avoid water
            if (grid != null)
            {
                Vector3Int blockPos = grid.WorldToGrid(hit.point);
                BlockType block = grid.GetBlock(blockPos.x, blockPos.y, blockPos.z);
                // Only spawn on solid ground, not water
                if (block == BlockType.Water || block == BlockType.Air)
                    continue;
            }

            return hit.point + Vector3.up * 1.5f;
        }
        Debug.LogError("[ColonistSpawner] Could not find valid spawn position");
        return spawnCenter + Vector3.up * 3f;
    }

    private string GenerateName(bool male)
    {
        string first = male ? maleNames[Random.Range(0, maleNames.Length)] : femaleNames[Random.Range(0, femaleNames.Length)];
        string last = surnames[Random.Range(0, surnames.Length)];
        return $"{first} {last}";
    }

    public void RemoveColonist(Colonist colonist)
    {
        Colonists.Remove(colonist);
    }
}
