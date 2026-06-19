using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Trap placed in the world. Triggers when enemies walk over it.
/// Different trap types have different effects.
/// </summary>
public class Trap : MonoBehaviour
{
    public enum TrapType
    {
        Spikes,         // deals damage
        Pit,            // slows enemies
        FireTrap,       // deals fire damage over time
        Explosive,      // deals heavy area damage, then destroys itself
    }

    [Header("Trap Properties")]
    public TrapType trapType = TrapType.Spikes;
    public float damage = 30f;
    public DamageType damageType = DamageType.Pierce;
    public float triggerCooldown = 3f; // seconds before can trigger again
    public float slowAmount = 0.5f; // speed multiplier when slowed
    public float slowDuration = 3f;

    [Header("State")]
    public float lastTriggerTime;
    public bool isArmed = true;

    void OnTriggerEnter(Collider other)
    {
        if (!isArmed) return;
        if (Time.time - lastTriggerTime < triggerCooldown) return;

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy == null) return;

        Trigger(enemy);
    }

    /// <summary>
    /// Triggers the trap on an enemy.
    /// </summary>
    void Trigger(Enemy enemy)
    {
        lastTriggerTime = Time.time;

        switch (trapType)
        {
            case TrapType.Spikes:
                enemy.TakeDamage(damage, damageType);
                Debug.Log($"[Trap] Spikes hit {enemy.enemyName} for {damage} dmg");
                break;

            case TrapType.Pit:
                enemy.moveSpeed *= slowAmount;
                Invoke(nameof(RestoreSpeed), slowDuration);
                Debug.Log($"[Trap] Pit slowed {enemy.enemyName} for {slowDuration}s");
                break;

            case TrapType.FireTrap:
                enemy.TakeDamage(damage * 0.5f, DamageType.Fire);
                Debug.Log($"[Trap] Fire trap hit {enemy.enemyName}");
                break;

            case TrapType.Explosive:
                // Damage all enemies in radius
                Collider[] hits = Physics.OverlapSphere(transform.position, 3f);
                foreach (Collider hit in hits)
                {
                    Enemy e = hit.GetComponent<Enemy>();
                    if (e != null) e.TakeDamage(damage, DamageType.Explosive);
                }
                Debug.Log($"[Trap] Explosive trap triggered!");
                Destroy(gameObject);
                return;
        }
    }

    void RestoreSpeed()
    {
        // TODO: track which enemy was slowed and restore
    }
}
