using UnityEngine;
using System.Collections.Generic;

/// <summary>Fire spread system + seasonal effects.</summary>
public class FireAndSeasons : MonoBehaviour
{
    private GridManager _grid;
    private float _timer;
    private HashSet<Vector3Int> _burning = new();
    public int currentSeason; // 0=Spring, 1=Summer, 2=Autumn, 3=Winter
    private string[] _seasonNames = { "Spring", "Summer", "Autumn", "Winter" };

    void Awake() { _grid = GetComponent<GridManager>(); if (_grid == null) _grid = FindFirstObjectByType<GridManager>(); }

    void Update()
    {
        _timer += Time.unscaledDeltaTime;
        if (_timer < 3f) return;
        _timer = 0f;

        var day = FindFirstObjectByType<DayCycle>();
        if (day != null) currentSeason = (day.day / 15) % 4; // ~15 days per season

        SpreadFire();
    }

    /// <summary>Ignite a block.</summary>
    public void Ignite(Vector3Int pos)
    {
        if (_grid.GetBlock(pos.x, pos.y, pos.z) == BlockType.Wood || _grid.GetBlock(pos.x, pos.y, pos.z) == BlockType.WoodPlanks)
            _burning.Add(pos);
    }

    void SpreadFire()
    {
        var newBurning = new HashSet<Vector3Int>();
        var toRemove = new List<Vector3Int>();

        foreach (var pos in _burning)
        {
            // 10% chance to destroy the burning block
            if (Random.value < 0.1f)
            {
                _grid.RemoveBlock(pos.x, pos.y, pos.z);
                toRemove.Add(pos);
                continue;
            }

            // Spread to neighbors
            Vector3Int[] dirs = { new(1,0,0), new(-1,0,0), new(0,1,0), new(0,-1,0), new(0,0,1), new(0,0,-1) };
            foreach (var d in dirs)
            {
                Vector3Int n = pos + d;
                BlockType b = _grid.GetBlock(n.x, n.y, n.z);
                if (b == BlockType.Wood || b == BlockType.WoodPlanks)
                    if (Random.value < 0.3f) newBurning.Add(n);
            }
        }

        foreach (var p in toRemove) _burning.Remove(p);
        foreach (var p in newBurning) _burning.Add(p);
    }

    /// <summary>Get temperature modifier for current season.</summary>
    public float GetSeasonTempMod()
    {
        return currentSeason switch { 0 => 0f, 1 => 5f, 2 => 0f, 3 => -10f };
    }

    /// <summary>Get farming speed modifier for season.</summary>
    public float GetSeasonFarmMod()
    {
        return currentSeason switch { 0 => 1f, 1 => 1.2f, 2 => 0.8f, 3 => 0.3f };
    }

    public string SeasonName => _seasonNames[currentSeason];
    public bool IsBurning => _burning.Count > 0;
}
