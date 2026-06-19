using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages enemy raids — spawns waves of enemies near the colony.
/// Raid difficulty scales with colony wealth and time.
/// </summary>
public class RaidManager : MonoBehaviour
{
    [Header("Raid Settings")]
    public float minRaidInterval = 120f; // seconds at 1x speed
    public float maxRaidInterval = 300f;
    public float timeSinceLastRaid;
    public float raidInterval; // randomized each time

    [Header("Difficulty Scaling")]
    public int currentRaidWave;
    public float wealthMultiplier = 0.01f; // more wealth = harder raids

    [Header("Spawn")]
    public float spawnDistance = 30f; // how far from colony center
    public GameObject enemyPrefab; // optional prefab for enemies

    [Header("AI")]
    public GameObject banditPrefab;

    private float _timer;
    private ColonistSpawner _colonistSpawner;
    private int _totalColonyWealth;

    void Start()
    {
        _colonistSpawner = FindObjectOfType<ColonistSpawner>();
        raidInterval = Random.Range(minRaidInterval, maxRaidInterval);
        timeSinceLastRaid = 0f;
    }

    void Update()
    {
        if (DayCycle.Instance != null && DayCycle.Instance.IsPaused) return;

        _timer += Time.deltaTime * (DayCycle.Instance != null ? DayCycle.Instance.gameSpeed : 1f);
        if (_timer >= raidInterval)
        {
            _timer = 0f;
            timeSinceLastRaid = 0f;
            StartRaid();
        }
        timeSinceLastRaid += Time.deltaTime;
    }

    /// <summary>
    /// Starts a raid — spawns enemies near the colony.
    /// Amount scales with colony wealth and wave number.
    /// </summary>
    public void StartRaid()
    {
        currentRaidWave++;
        int enemyCount = 1 + currentRaidWave + Mathf.FloorToInt(_totalColonyWealth * wealthMultiplier);
        enemyCount = Mathf.Min(enemyCount, 20); // cap at 20 enemies

        Debug.Log($"[RaidManager] Wave {currentRaidWave}: spawning {enemyCount} enemies");

        Vector3 center = _colonistSpawner != null
            ? _colonistSpawner.Colonists[0]?.transform.position ?? Vector3.zero
            : new Vector3(50, 15, 50);

        for (int i = 0; i < enemyCount; i++)
        {
            SpawnEnemy(center);
        }

        // Reset timer
        raidInterval = Random.Range(minRaidInterval, maxRaidInterval);
    }

    /// <summary>
    /// Spawns a single enemy at a random position near the target.
    /// </summary>
    void SpawnEnemy(Vector3 target)
    {
        Vector2 circle = Random.insideUnitCircle.normalized * spawnDistance;
        Vector3 spawnPos = target + new Vector3(circle.x, 0f, circle.y);

        // Raycast to ground
        if (Physics.Raycast(spawnPos + Vector3.up * 20f, Vector3.down, out RaycastHit hit, 50f))
            spawnPos = hit.point + Vector3.up * 0.5f;
        else
            spawnPos.y = target.y;

        // Choose enemy type based on wave
        EnemyType type = EnemyType.BanditMelee;
        if (currentRaidWave >= 3) type = (EnemyType)(Random.Range(0, 3)); // melee, ranged, or boss
        if (currentRaidWave >= 6) type = (EnemyType)(Random.Range(0, 5)); // add wolfs, spiders
        if (currentRaidWave >= 10) type = (EnemyType)(Random.Range(0, 8)); // add cultists, mutants

        GameObject go = banditPrefab != null
            ? Instantiate(banditPrefab, spawnPos, Quaternion.identity)
            : GameObject.CreatePrimitive(PrimitiveType.Capsule);

        Enemy enemy = go.GetComponent<Enemy>();
        if (enemy == null) enemy = go.AddComponent<Enemy>();

        enemy.enemyType = type;
        enemy.enemyName = $"{type} (Wave {currentRaidWave})";
        enemy.maxHealth = 50f + currentRaidWave * 20f;
        enemy.health = enemy.maxHealth;
        enemy.attackDamage = 5f + currentRaidWave * 3f;
        enemy.moveSpeed = 2f + currentRaidWave * 0.2f;

        // Color based on type
        Renderer r = go.GetComponent<Renderer>();
        if (r != null)
        {
            r.material.color = type switch
            {
                EnemyType.BanditMelee => Color.red,
                EnemyType.BanditRanged => Color.magenta,
                EnemyType.BanditBoss => Color.black,
                EnemyType.Wolf => Color.gray,
                EnemyType.Bear => Color.yellow,
                _ => Color.red,
            };
        }

        go.transform.localScale = Vector3.one * 0.9f;

        Debug.Log($"[RaidManager] Spawned {enemy.enemyName} at {spawnPos}");
    }

    /// <summary>
    /// Updates colony wealth for difficulty scaling.
    /// </summary>
    public void UpdateWealth(int wealth)
    {
        _totalColonyWealth = wealth;
    }
}
