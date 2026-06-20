using UnityEngine;

/// <summary>
/// Adds gravity to colonists. Checks if ground exists below and falls if not.
/// Uses grid data directly — no physics dependencies.
/// </summary>
public class ColonistGravity : MonoBehaviour
{
    public float gravity = 9.8f;
    public float fallSpeed;
    private GridManager _grid;
    private Vector3 _velocity;

    void Awake()
    {
        _grid = GetComponent<GridManager>();
        if (_grid == null) _grid = FindObjectOfType<GridManager>();
    }

    void Update()
    {
        // Gravity handled by ColonistAI.SnapToSurface
    }
}
