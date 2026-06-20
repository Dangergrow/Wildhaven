using UnityEngine;

/// <summary>
/// Water interaction: colonists float with head above water. Prevents drowning.
/// Replaces the need for complicated physics in water.
/// </summary>
public class WaterInteraction : MonoBehaviour
{
    private GridManager _grid;
    private Colonist _colonist;

    void Awake()
    {
        _grid = GetComponent<GridManager>();
        if (_grid == null) _grid = FindObjectOfType<GridManager>();
        _colonist = GetComponent<Colonist>();
        if (_colonist == null) _colonist = gameObject.AddComponent<Colonist>();
    }

    void Update()
    {
        if (_grid == null || _colonist == null) return;
        if (_colonist.currentState == ColonistState.Dead) return;

        Vector3Int pos = _grid.WorldToGrid(transform.position);
        BlockType here = _grid.GetBlock(pos.x, pos.y, pos.z);
        BlockType below = _grid.GetBlock(pos.x, pos.y - 1, pos.z);

        bool inWater = here == BlockType.Water || below == BlockType.Water;

        if (inWater)
        {
            // Float — keep colonist at water surface
            Vector3 waterSurface = _grid.GridToWorld(pos.x, pos.y, pos.z);
            float waterTop = waterSurface.y + _grid.BlockSize * 0.5f;
            transform.position = new Vector3(transform.position.x, waterTop + 0.8f, transform.position.z);

            // Slow movement in water
            if (_colonist != null)
                _colonist.moveSpeedMultiplier = 0.3f;
        }
        else
        {
            if (_colonist != null)
                _colonist.moveSpeedMultiplier = 1f;
        }
    }
}
