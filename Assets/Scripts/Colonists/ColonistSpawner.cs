using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Spawns initial colonists and manages colonist list.
/// </summary>
public class ColonistSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("Number of starting colonists")]
    public int startingColonists = 3;

    [Tooltip("Prefab for colonist (requires Colonist + ColonistAI + NeedsSystem + NavMeshAgent)")]
    public GameObject colonistPrefab;

    [Tooltip("Spawn area center")]
    public Vector3 spawnCenter = new Vector3(50f, 25f, 50f);

    [Tooltip("Spawn radius")]
    public float spawnRadius = 5f;

    [Header("Random Name Pool")]
    public string[] maleNames = new string[] { "Ivan", "Boris", "Dmitri", "Alexei", "Sergei", "Nikolai", "Viktor" };
    public string[] femaleNames = new string[] { "Anna", "Maria", "Elena", "Olga", "Natasha", "Svetlana", "Irina" };
    public string[] surnames = new string[] { "Petrov", "Ivanov", "Sidorov", "Kuznetsov", "Smirnov", "Volkov", "Morozov" };

    // All spawned colonists
    public List<Colonist> Colonists { get; private set; } = new List<Colonist>();

    private void Start()
    {
        SpawnInitialColonists();
    }

    /// <summary>
    /// Spawns the starting colonist group.
    /// </summary>
    private void SpawnInitialColonists()
    {
        for (int i = 0; i < startingColonists; i++)
        {
            SpawnColonist(GetRandomSpawnPosition());
        }
    }

    /// <summary>
    /// Spawns a single colonist at the given position.
    /// </summary>
    public Colonist SpawnColonist(Vector3 position)
    {
        if (colonistPrefab == null)
        {
            Debug.LogError("[ColonistSpawner] No colonist prefab assigned!");
            return null;
        }

        // Disable NavMeshAgent during spawn to avoid "not close to NavMesh" error
        GameObject go = Instantiate(colonistPrefab, position, Quaternion.identity);
        UnityEngine.AI.NavMeshAgent agent = go.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null) agent.enabled = false;

        Colonist colonist = go.GetComponent<Colonist>();

        // Re-enable agent and warp to nearest NavMesh
        if (agent != null)
        {
            agent.enabled = true;
            if (UnityEngine.AI.NavMesh.SamplePosition(position, out UnityEngine.AI.NavMeshHit hit, 50f, UnityEngine.AI.NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
        }
        if (colonist == null)
        {
            Debug.LogError("[ColonistSpawner] Prefab has no Colonist component!");
            Destroy(go);
            return null;
        }

        // Random identity
        colonist.isMale = Random.value > 0.5f;
        colonist.colonistName = GenerateName(colonist.isMale);
        colonist.age = Random.Range(18, 50);

        // Random skills (weighted)
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

        // Random traits (30% chance for each)
        colonist.perk = Random.value < 0.3f ? (Perk)Random.Range(1, System.Enum.GetValues(typeof(Perk)).Length) : Perk.None;
        colonist.flaw = Random.value < 0.3f ? (Flaw)Random.Range(1, System.Enum.GetValues(typeof(Flaw)).Length) : Flaw.None;

        Colonists.Add(colonist);
        Debug.Log($"[ColonistSpawner] Spawned {colonist.colonistName} ({colonist.age})");
        return colonist;
    }

    /// <summary>
    /// Generates a random name.
    /// </summary>
    private string GenerateName(bool male)
    {
        string first = male
            ? maleNames[Random.Range(0, maleNames.Length)]
            : femaleNames[Random.Range(0, femaleNames.Length)];
        string last = surnames[Random.Range(0, surnames.Length)];
        return $"{first} {last}";
    }

    /// <summary>
    /// Returns a random position near spawn center (any height — Warp will fix).
    /// </summary>
    private Vector3 GetRandomSpawnPosition()
    {
        Vector2 circle = Random.insideUnitCircle * spawnRadius;
        return spawnCenter + new Vector3(circle.x, 0f, circle.y);
    }

    /// <summary>
    /// Removes a colonist from tracking.
    /// </summary>
    public void RemoveColonist(Colonist colonist)
    {
        Colonists.Remove(colonist);
    }
}
