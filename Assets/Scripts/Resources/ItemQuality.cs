using UnityEngine;

/// <summary>Tracks item quality tiers: Normal, Good, Excellent, Masterwork.</summary>
public class ItemQuality : MonoBehaviour
{
    public enum Quality { Normal, Good, Excellent, Masterwork }
    
    /// <summary>Roll quality based on crafter's skill.</summary>
    public static Quality RollQuality(int skillLevel)
    {
        float roll = Random.value;
        if (skillLevel >= 18 && roll < 0.1f) return Quality.Masterwork;
        if (skillLevel >= 14 && roll < 0.2f) return Quality.Excellent;
        if (skillLevel >= 10 && roll < 0.4f) return Quality.Good;
        return Quality.Normal;
    }

    /// <summary>Get stat multiplier for quality tier.</summary>
    public static float GetMultiplier(Quality q) => q switch
    {
        Quality.Masterwork => 2.0f,
        Quality.Excellent => 1.5f,
        Quality.Good => 1.2f,
        _ => 1.0f,
    };

    /// <summary>Get durability multiplier.</summary>
    public static float GetDurabilityMult(Quality q) => q switch
    {
        Quality.Masterwork => 3f,
        Quality.Excellent => 2f,
        Quality.Good => 1.5f,
        _ => 1f,
    };
}

/// <summary>Adds durability to items. Tools/weapons/armor degrade and break.</summary>
public class ItemDurability : MonoBehaviour
{
    private Inventory _inv;

    void Awake() { _inv = GetComponent<Inventory>(); }

    /// <summary>Degrade weapon/tool on use. Returns true if broke.</summary>
    public bool UseWeapon()
    {
        if (_inv == null) return false;
        var eq = GetComponent<Equipment>();
        if (eq == null) return false;
        // 5% chance to lose 1 durability on weapon
        if (Random.value < 0.05f && eq.weapon.itemType != 0)
        {
            Debug.Log($"[Durability] {name}'s weapon degraded");
            return Random.value < 0.1f; // 10% chance to break when degrading
        }
        return false;
    }

    /// <summary>Degrade tool on use.</summary>
    public bool UseTool()
    {
        var eq = GetComponent<Equipment>();
        if (eq == null || eq.tool.itemType == 0) return false;
        if (Random.value < 0.05f)
        {
            Debug.Log($"[Durability] {name}'s tool degraded");
            return Random.value < 0.1f;
        }
        return false;
    }
}
