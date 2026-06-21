using UnityEngine;

/// <summary>
/// Prevents blocks from being placed at colonist's position.
/// Checks if any colonist is at the target placement coordinates.
/// </summary>
public class BuildBlocker : MonoBehaviour
{
    private static System.Collections.Generic.HashSet<Vector3Int> _occupiedPositions
        = new System.Collections.Generic.HashSet<Vector3Int>();

    private GridManager _grid;
    private Vector3Int _lastPos;

    void Start()
    {
        _grid = FindObjectOfType<GridManager>();
    }

    void Update()
    {
        if (_grid == null) return;
        Vector3Int current = _grid.WorldToGrid(transform.position);

        // Update occupied positions
        if (current != _lastPos)
        {
            _occupiedPositions.Remove(_lastPos);
            _occupiedPositions.Add(current);
            _lastPos = current;
        }
    }

    void OnDestroy()
    {
        _occupiedPositions.Remove(_lastPos);
    }

    /// <summary>Returns true if a colonist occupies this grid position.</summary>
    public static bool IsOccupied(int x, int y, int z)
    {
        return _occupiedPositions.Contains(new Vector3Int(x, y, z));
    }
}
