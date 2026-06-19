using UnityEngine;

/// <summary>
/// Enemy controller. Handles combat behavior, targeting, and death.
/// Works with CombatManager for coordinated attacks.
/// </summary>
public class Enemy : MonoBehaviour
{
    [Header("Identity")]
    public EnemyType enemyType;
    public string enemyName = "Bandit";

    [Header("Stats")]
    public float maxHealth = 100f;
    public float health = 100f;
    public float moveSpeed = 2f;
    public float attackRange = 1.5f;
    public float attackDamage = 10f;
    public float attackCooldown = 1f;
    public DamageType damageType = DamageType.Slash;
    public float armorRating; // reduces incoming damage by this amount

    [Header("AI")]
    public float sightRange = 20f;
    public float fleeThreshold = 0.2f; // flee when health < 20%
    public CombatState state = CombatState.Idle;

    [Header("Target")]
    public Colonist currentTarget;
    public float lastAttackTime;

    [Header("Loot")]
    public ItemType[] lootItems;
    public int[] lootAmounts;

    // Components
    private ColonistSpawner _colonistSpawner;

    void Start()
    {
        health = maxHealth;
        _colonistSpawner = FindObjectOfType<ColonistSpawner>();
    }

    void Update()
    {
        if (state == CombatState.Dead) return;

        // Check if should flee
        if (health / maxHealth < fleeThreshold)
        {
            state = CombatState.Fleeing;
            return;
        }

        // Find nearest colonist
        currentTarget = FindNearestColonist();
        if (currentTarget == null)
        {
            state = CombatState.Idle;
            return;
        }

        float dist = Vector3.Distance(transform.position, currentTarget.transform.position);

        if (dist > sightRange)
        {
            state = CombatState.Idle;
            return;
        }

        if (dist <= attackRange && Time.time - lastAttackTime > attackCooldown)
        {
            Attack();
        }
        else if (dist > attackRange)
        {
            MoveToward(currentTarget.transform.position);
        }
    }

    Colonist FindNearestColonist()
    {
        if (_colonistSpawner == null) return null;
        Colonist nearest = null;
        float minDist = sightRange;
        foreach (Colonist c in _colonistSpawner.Colonists)
        {
            if (c == null || c.currentState == ColonistState.Dead) continue;
            float d = Vector3.Distance(transform.position, c.transform.position);
            if (d < minDist) { minDist = d; nearest = c; }
        }
        return nearest;
    }

    void MoveToward(Vector3 target)
    {
        state = CombatState.Moving;
        Vector3 dir = (target - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;
    }

    void Attack()
    {
        if (currentTarget == null) return;
        state = CombatState.Attacking;
        lastAttackTime = Time.time;

        float damage = attackDamage;
        bool killed = currentTarget.TakeDamage(damage);

        Debug.Log($"[Combat] {enemyName} attacks {currentTarget.colonistName} for {damage:F0} dmg. Health: {currentTarget.health:F0}");

        if (killed)
        {
            Debug.Log($"[Combat] {currentTarget.colonistName} killed by {enemyName}!");
            currentTarget = null;
        }
    }

    /// <summary>
    /// Takes damage from a colonist or trap. Returns true if killed.
    /// </summary>
    public bool TakeDamage(float amount, DamageType type)
    {
        float effective = Mathf.Max(1f, amount - armorRating);
        health -= effective;

        if (health <= 0f)
        {
            health = 0f;
            Die();
            return true;
        }
        return false;
    }

    void Die()
    {
        state = CombatState.Dead;
        DropLoot();
        Debug.Log($"[Combat] {enemyName} died.");
        Destroy(gameObject, 1f);
    }

    void DropLoot()
    {
        if (lootItems == null || lootItems.Length == 0) return;
        for (int i = 0; i < lootItems.Length; i++)
        {
            // Spawn loot as yellow cubes (like BlockDropManager does)
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.transform.position = transform.position + Vector3.up * (i + 1) * 0.3f;
            go.transform.localScale = Vector3.one * 0.2f;
            go.GetComponent<Renderer>().material.color = Color.green;
            WorldItem wi = go.AddComponent<WorldItem>();
            wi.itemType = lootItems[i];
            wi.amount = lootAmounts[i];
        }
    }
}
