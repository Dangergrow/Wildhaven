using UnityEngine;
using System.Collections.Generic;

/// <summary>Burial system: graves, corpse handling, mood effects.</summary>
public class BurialSystem : MonoBehaviour
{
    private GridManager _grid;
    private List<Vector3Int> _graves = new();
    private List<Vector3Int> _corpses = new();

    void Awake() { _grid = GetComponent<GridManager>(); if (_grid == null) _grid = FindFirstObjectByType<GridManager>(); }

    /// <summary>Designate a grave at position.</summary>
    public void AddGrave(Vector3Int pos) => _graves.Add(pos);

    /// <summary>Bury a corpse — removes corpse, uses grave, restores mood.</summary>
    public bool BuryCorpse(Vector3Int gravePos, Colonist corpse, Colonist burier)
    {
        if (!_graves.Contains(gravePos)) return false;
        if (corpse.currentState != ColonistState.Dead) return false;

        _graves.Remove(gravePos);
        // Mood bonus for proper burial
        var spawner = FindFirstObjectByType<ColonistSpawner>();
        if (spawner != null)
            foreach (var c in spawner.Colonists)
                if (c.currentState != ColonistState.Dead)
                    c.ModifyMood(3f);

        Object.Destroy(corpse.gameObject);
        Debug.Log($"[Burial] {corpse.colonistName} buried by {burier.colonistName}");
        return true;
    }

    /// <summary>Get unburied corpse count (for mood penalty).</summary>
    public int UnburiedCount => _corpses.Count;

    /// <summary>Track a corpse position.</summary>
    public void TrackCorpse(Vector3Int pos, Colonist corpse)
    {
        _corpses.Add(pos);
        // Mood penalty for unburied corpses
        var spawner = FindFirstObjectByType<ColonistSpawner>();
        if (spawner != null)
            foreach (var c in spawner.Colonists)
                if (c.currentState != ColonistState.Dead)
                    c.ModifyMood(-2f);
    }
}

/// <summary>Beauty/environment system. Sculptures, art, clean floors improve mood.</summary>
public class BeautySystem : MonoBehaviour
{
    private GridManager _grid;

    void Awake() { _grid = GetComponent<GridManager>(); if (_grid == null) _grid = FindFirstObjectByType<GridManager>(); }

    /// <summary>Calculate beauty score at a position (0-100).</summary>
    public float GetBeauty(Vector3Int pos)
    {
        float score = 20f; // baseline
        // Check nearby blocks for art/beautiful materials
        for (int dx = -3; dx <= 3; dx++)
        for (int dz = -3; dz <= 3; dz++)
        {
            Vector3Int p = new(pos.x + dx, pos.y, pos.z + dz);
            BlockType b = _grid.GetBlock(p.x, p.y, p.z);
            score += b switch
            {
                BlockType.Marble => 5f,
                BlockType.Glass => 3f,
                BlockType.GoldOre => 4f,
                BlockType.StoneBrick => 2f,
                BlockType.Dirt => -2f, // dirt is ugly
                _ => 0f,
            };
        }
        return Mathf.Clamp(score, 0, 100);
    }
}
