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
        _grid = FindObjectOfType<GridManager>();
    }

    void Update()
    {
        if (_grid == null) return;

        Vector3 pos = transform.position;
        Vector3Int below = _grid.WorldToGrid(pos + Vector3.down * 0.6f);
        BlockType blockBelow = _grid.GetBlock(below.x, below.y, below.z);

        bool onGround = blockBelow != BlockType.Air && blockBelow != BlockType.Water;

        if (!onGround)
        {
            // Fall
            _velocity.y -= gravity * Time.deltaTime;
            transform.position += _velocity * Time.deltaTime;
        }
        else
        {
            // Landed — snap to ground
            _velocity.y = 0f;
            Vector3 groundPos = _grid.GridToWorld(below.x, below.y, below.z);
            float surfaceY = groundPos.y + _grid.BlockSize * 0.5f;
            if (transform.position.y < surfaceY + 0.1f)
                transform.position = new Vector3(transform.position.x, surfaceY + 0.6f, transform.position.z);
        }
    }
}
