using UnityEngine;

/// <summary>
/// Handles water interaction and death effects for units.
/// Falls into water = slow + teleport to shore.
/// Death = fade out + destruction.
/// </summary>
public class UnitPhysics : MonoBehaviour
{
    [Header("Water")]
    public float waterSlowMultiplier = 0.3f;
    public float waterPushForce = 2f;

    [Header("Death")]
    public float deathFadeTime = 1.5f;

    private GridManager _grid;
    private Colonist _colonist;

    void Awake()
    {
        _grid = FindObjectOfType<GridManager>();
        _colonist = GetComponent<Colonist>();
    }

    void Update()
    {
        if (_grid == null) return;

        // Check if in water
        Vector3 pos = transform.position;
        Vector3Int gridPos = _grid.WorldToGrid(pos);
        BlockType current = _grid.GetBlock(gridPos.x, gridPos.y, gridPos.z);
        BlockType below = _grid.GetBlock(gridPos.x, gridPos.y - 1, gridPos.z);

        if (current == BlockType.Water || below == BlockType.Water)
        {
            // Push toward nearest shore
            Vector3 shore = FindNearestShore(pos);
            transform.position = Vector3.MoveTowards(transform.position, shore, waterPushForce * Time.deltaTime);

            // Slow down
            if (_colonist != null)
                _colonist.moveSpeedMultiplier = waterSlowMultiplier;
        }
        else
        {
            if (_colonist != null)
                _colonist.moveSpeedMultiplier = 1f;
        }

        // Death check
        if (_colonist != null && _colonist.currentState == ColonistState.Dead)
        {
            HandleDeath();
        }
    }

    Vector3 FindNearestShore(Vector3 pos)
    {
        Vector3 best = pos + Vector3.up * 3f;
        float bestDist = float.MaxValue;

        for (int dx = -5; dx <= 5; dx++)
        {
            for (int dz = -5; dz <= 5; dz++)
            {
                Vector3Int check = _grid.WorldToGrid(pos + new Vector3(dx, 0, dz));
                BlockType b = _grid.GetBlock(check.x, check.y, check.z);
                if (b != BlockType.Water && b != BlockType.Air && b != BlockType.Bedrock)
                {
                    Vector3 shore = _grid.GridToWorld(check.x, check.y, check.z);
                    float d = Vector3.Distance(pos, shore);
                    if (d < bestDist) { bestDist = d; best = shore + Vector3.up * 0.5f; }
                }
            }
        }
        return best;
    }

    void HandleDeath()
    {
        Renderer r = GetComponent<Renderer>();
        if (r != null)
        {
            Color c = r.material.color;
            c.a -= Time.deltaTime / deathFadeTime;
            r.material.color = c;

            if (c.a <= 0.05f)
                Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject, deathFadeTime);
        }
    }
}
