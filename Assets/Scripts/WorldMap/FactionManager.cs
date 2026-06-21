using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages faction relationships and reputation.
/// Reputation ranges from -100 (hated) to +100 (allied).
/// </summary>
public class FactionManager : MonoBehaviour
{
    [Header("Factions")]
    public FactionDef[] factions;

    [Header("Player")]
    public int playerFactionId = -999;

    /// <summary>Player's reputation with each faction. Key = factionId.</summary>
    public Dictionary<int, int> Reputation { get; private set; } = new Dictionary<int, int>();

    /// <summary>Average reputation across all factions (-100 to +100).</summary>
    public float GetAverageReputation()
    {
        if (Reputation.Count == 0) return 0;
        float sum = 0;
        foreach (var r in Reputation.Values) sum += r;
        return sum / Reputation.Count;
    }

    void Awake()
    {
        if (factions == null || factions.Length == 0)
        {
            // Load from WorldMapGenerator
            WorldMapGenerator wmg = FindObjectOfType<WorldMapGenerator>();
            if (wmg != null) factions = wmg.factions;
        }

        // Initialize reputations
        if (factions != null)
        {
            foreach (FactionDef f in factions)
            {
                if (f == null) continue;
                // Start neutral
                if (!Reputation.ContainsKey(f.id))
                    Reputation[f.id] = 0;
            }
        }
    }

    /// <summary>
    /// Modifies reputation with a faction.
    /// </summary>
    public void ChangeReputation(int factionId, int delta)
    {
        if (!Reputation.ContainsKey(factionId))
            Reputation[factionId] = 0;

        Reputation[factionId] = Mathf.Clamp(Reputation[factionId] + delta, -100, 100);

        string relation = GetRelationLabel(factionId);
        Debug.Log($"[FactionManager] Reputation with {GetFactionName(factionId)}: {Reputation[factionId]} ({relation})");
    }

    /// <summary>
    /// Returns the relationship label for a faction.
    /// </summary>
    public string GetRelationLabel(int factionId)
    {
        int rep = Reputation.ContainsKey(factionId) ? Reputation[factionId] : 0;
        if (rep <= -75) return "War";
        if (rep <= -25) return "Hostile";
        if (rep <= 25) return "Neutral";
        if (rep <= 75) return "Friendly";
        return "Allied";
    }

    /// <summary>
    /// Checks if a faction is hostile.
    /// </summary>
    public bool IsHostile(int factionId)
    {
        int rep = Reputation.ContainsKey(factionId) ? Reputation[factionId] : 0;
        return rep <= -25;
    }

    /// <summary>
    /// Checks if a faction is allied.
    /// </summary>
    public bool IsAllied(int factionId)
    {
        int rep = Reputation.ContainsKey(factionId) ? Reputation[factionId] : 0;
        return rep >= 75;
    }

    /// <summary>
    /// Gets faction name by ID.
    /// </summary>
    public string GetFactionName(int factionId)
    {
        if (factions == null) return "Unknown";
        foreach (FactionDef f in factions)
        {
            if (f != null && f.id == factionId) return f.name;
        }
        return "Unknown";
    }

    /// <summary>
    /// Returns all factions sorted by reputation (best first).
    /// </summary>
    public List<FactionDef> GetFactionsByReputation()
    {
        List<FactionDef> sorted = new List<FactionDef>();
        if (factions == null) return sorted;

        sorted.AddRange(factions);
        sorted.Sort((a, b) =>
        {
            int ra = Reputation.ContainsKey(a.id) ? Reputation[a.id] : 0;
            int rb = Reputation.ContainsKey(b.id) ? Reputation[b.id] : 0;
            return rb.CompareTo(ra);
        });
        return sorted;
    }
}
