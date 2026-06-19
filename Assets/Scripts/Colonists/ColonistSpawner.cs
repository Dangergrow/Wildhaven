using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Spawns initial colonists and manages colonist list.
/// Uses physics raycast against world MeshCollider.
/// </summary>
public class ColonistSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public int startingColonists = 3;
    public GameObject colonistPrefab;
    public Vector3 spawnCenter = new Vector3(50f, 50f, 50f);
    public float spawnRadius = 10f;

    [Header("Random Name Pool")]
    public string[] maleNames = { "Ivan", "Boris", "Dmitri", "Alexei", "Sergei", "Nikolai", "Viktor" };
    public string[] femaleNames = { "Anna", "Maria", "Elena", "Olga", "Natasha", "Svetlana", "Irina" };
    public string[] surnames = { "Petrov", "Ivanov", "Sidorov", "Kuznetsov", "Smirnov", "Volkov", "Morozov" };

    public List<Colonist> Colonists { get; private set; } = new List<Colonist>();

    private void Start()
    {
        SpawnInitialColonists();
    }

    private void SpawnInitialColonists()
    {
        for (int i = 0; i < startingColonists; i++)
        {
            Vector3 pos = GetRandomSpawnPosition();
            SpawnColonist(pos);
        }
    }

    public Colonist SpawnColonist(Vector3 position)
    {
        if (colonistPrefab == null) { Debug.LogError("[ColonistSpawner] No prefab!"); return null; }

        GameObject go = Instantiate(colonistPrefab, position, Quaternion.identity);
        Colonist colonist = go.GetComponent<Colonist>();
        if (colonist == null) { Debug.LogError("[ColonistSpawner] Prefab missing Colonist component!"); Destroy(go); return null; }

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

        Colonists.Add(colonist);
        Debug.Log($"[ColonistSpawner] Spawned {colonist.colonistName} ({colonist.age}) at {position}");
        return colonist;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        for (int attempt = 0; attempt < 100; attempt++)
        {
            Vector2 circle = Random.insideUnitCircle * spawnRadius;
            Vector3 origin = spawnCenter + new Vector3(circle.x, 0f, circle.y);

            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 80f))
            {
                return hit.point + Vector3.up * 0.5f;
            }
        }
        Debug.LogError("[ColonistSpawner] Raycast failed. Is MeshCollider on World?");
        return spawnCenter + Vector3.up * 2f;
    }

    private string GenerateName(bool male)
    {
        string first = male ? maleNames[Random.Range(0, maleNames.Length)] : femaleNames[Random.Range(0, femaleNames.Length)];
        return $"{first} {surnames[Random.Range(0, surnames.Length)]}";
    }

    public void RemoveColonist(Colonist c) => Colonists.Remove(c);
}
