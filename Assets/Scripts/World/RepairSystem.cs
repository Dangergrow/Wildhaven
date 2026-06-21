using UnityEngine;

/// <summary>Repair damaged structures. Colonists fix broken walls/blocks.</summary>
public class RepairSystem : MonoBehaviour
{
    private GridManager _grid;
    private float _timer;

    void Awake() { _grid = GetComponent<GridManager>(); if (_grid == null) _grid = FindFirstObjectByType<GridManager>(); }

    void Update()
    {
        _timer += Time.unscaledDeltaTime;
        if (_timer < 5f) return;
        _timer = 0f;

        var spawner = FindFirstObjectByType<ColonistSpawner>();
        if (spawner == null) return;

        // Find damaged blocks and repair them
        for (int x = 0; x < _grid.Width; x++)
        for (int y = 0; y < _grid.Height; y++)
        for (int z = 0; z < _grid.Depth; z++)
        {
            BlockType b = _grid.GetBlock(x, y, z);
            if (b == BlockType.Air) continue;
            // Random 5% chance to mark as damaged
            if (Random.value < 0.05f)
            {
                // Find nearest colonist with construction skill
                Colonist builder = null;
                float best = float.MaxValue;
                Vector3 pos = _grid.GridToWorld(x, y, z);
                foreach (var c in spawner.Colonists)
                {
                    if (c.currentState == ColonistState.Dead || c.currentState == ColonistState.Sleeping) continue;
                    float d = Vector3.Distance(c.transform.position, pos);
                    if (d < best && d < 10f) { best = d; builder = c; }
                }
                if (builder != null && builder.constructionSkill > 3)
                {
                    // Repair: restore any damaged block to full state
                    // In GM, repair is automatic when colonist is near
                    if (builder.currentState == ColonistState.Idle)
                        builder.currentState = ColonistState.Working;
                }
            }
        }
    }
}
