using UnityEngine;
using System.Collections.Generic;

/// <summary>Real repair system — tracks damaged blocks, colonists fix them.</summary>
public class RepairSystem : MonoBehaviour
{
    private GridManager _grid;
    private Dictionary<Vector3Int, float> _damaged = new(); // pos -> damage (0-100)
    private float _timer;

    void Awake() { _grid = GetComponent<GridManager>(); if (_grid == null) _grid = FindFirstObjectByType<GridManager>(); }

    void Update()
    {
        _timer += Time.unscaledDeltaTime;
        if (_timer < 5f) return;
        _timer = 0f;

        var spawner = FindFirstObjectByType<ColonistSpawner>();
        if (spawner == null || _grid == null) return;

        // Randomly damage some blocks (wear and tear)
        if (Random.value < 0.3f)
        {
            int rx = Random.Range(1, _grid.Width - 1);
            int rz = Random.Range(1, _grid.Depth - 1);
            for (int y = _grid.Height - 1; y > 1; y--)
            {
                if (_grid.GetBlock(rx, y, rz) != BlockType.Air)
                {
                    Vector3Int p = new(rx, y, rz);
                    if (!_damaged.ContainsKey(p)) _damaged[p] = 0;
                    _damaged[p] += Random.Range(5, 20);
                    break;
                }
            }
        }

        // Repair with colonists
        foreach (var c in spawner.Colonists)
        {
            if (c.currentState == ColonistState.Dead || c.constructionSkill < 3) continue;
            Vector3Int cp = _grid.WorldToGrid(c.transform.position);

            // Find damaged blocks nearby
            Vector3Int? toRepair = null;
            float bestDist = 5f;
            var keys = new List<Vector3Int>(_damaged.Keys);
            foreach (var dp in keys)
            {
                float d = Vector3Int.Distance(dp, cp);
                if (d < bestDist) { bestDist = d; toRepair = dp; }
            }

            if (toRepair != null)
            {
                _damaged[toRepair.Value] -= c.constructionSkill * 2f;
                if (_damaged[toRepair.Value] <= 0) _damaged.Remove(toRepair.Value);
                c.currentState = ColonistState.Working;
                break;
            }
        }
    }

    /// <summary>Check if a block is damaged.</summary>
    public bool IsDamaged(Vector3Int pos) => _damaged.ContainsKey(pos);
    public float GetDamage(Vector3Int pos) => _damaged.TryGetValue(pos, out float d) ? d : 0;
}
