using UnityEngine;
using System.Collections.Generic;

/// <summary>Religion system: faith need, rituals, holy buildings.</summary>
public class ReligionSystem : MonoBehaviour
{
    private GridManager _grid;
    private List<RitualSite> _sites = new();
    private float _ritualCooldown;

    public struct RitualSite
    {
        public Vector3Int pos;
        public RitualType type;
        public float lastUsed;
    }

    public enum RitualType { Prayer, Sacrifice, Harvest, Blessing, Wedding, Funeral, Festival }
    public enum Belief { CultOfAncestors, NatureWorship, BloodCult, SunWorship, MechGod, AncientOnes, Atheist }

    void Awake() { _grid = GetComponent<GridManager>(); if (_grid == null) _grid = FindObjectOfType<GridManager>(); }

    /// <summary>Register a holy building (call when block placed).</summary>
    public void RegisterSite(Vector3Int pos)
    {
        BlockType b = _grid.GetBlock(pos.x, pos.y, pos.z);
        RitualType t = b switch
        {
            BlockType.Marble => RitualType.Prayer, // altar
            BlockType.Obsidian => RitualType.Sacrifice, // sacrificial altar
            BlockType.GoldOre => RitualType.Blessing,
            _ => RitualType.Prayer,
        };
        _sites.Add(new RitualSite { pos = pos, type = t });
    }

    /// <summary>Perform a ritual. Returns mood bonus for all colonists.</summary>
    public float PerformRitual(RitualType type, Vector3Int pos, Belief belief)
    {
        if (_ritualCooldown > 0) return 0;
        _ritualCooldown = 120f; // 2 min cooldown

        float bonus = type switch
        {
            RitualType.Prayer => 15f,
            RitualType.Sacrifice => 25f,
            RitualType.Harvest => 20f,
            RitualType.Blessing => 20f,
            RitualType.Wedding => 30f,
            RitualType.Funeral => 10f,
            RitualType.Festival => 35f,
            _ => 10f,
        };

        // Faith conflicts reduce bonus
        if (belief == Belief.Atheist) return 0;

        Debug.Log($"[Religion] Ritual {type} performed — +{bonus} mood");
        return bonus;
    }

    /// <summary>Find nearest holy site to a position.</summary>
    public Vector3Int? FindNearestSite(Vector3 pos)
    {
        Vector3Int? best = null;
        float bestDist = float.MaxValue;
        foreach (var s in _sites)
        {
            float d = Vector3.Distance(_grid.GridToWorld(s.pos.x, s.pos.y, s.pos.z), pos);
            if (d < bestDist) { bestDist = d; best = s.pos; }
        }
        return best;
    }

    /// <summary>Daily faith decay — pray at altar to restore.</summary>
    public static float GetFaithDecayRate(Belief b) => b switch
    {
        Belief.CultOfAncestors => 0.5f,
        Belief.BloodCult => 0.8f,
        Belief.Atheist => 0f,
        _ => 0.3f,
    };

    void Update()
    {
        if (_ritualCooldown > 0) _ritualCooldown -= Time.unscaledDeltaTime;
    }
}
