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
        Vector3Int below = _grid.WorldToGrid(pos + Vector3.down * 1.2f); // check deeper
        BlockType blockBelow = _grid.GetBlock(below.x, below.y, below.z);

        bool onGround = blockBelow != BlockType.Air && blockBelow != BlockType.Water;

        if (!onGround)
        {
            _velocity.y -= gravity * Time.deltaTime;
            transform.position += _velocity * Time.deltaTime;
            // Safety: don't fall below world
            if (transform.position.y < -5f) transform.position = new Vector3(transform.position.x, 15f, transform.position.z);
        }
        else
        {
            _velocity.y = 0f;
            Vector3 groundPos = _grid.GridToWorld(below.x, below.y, below.z);
            float surfaceY = groundPos.y + _grid.BlockSize * 0.5f;
            if (transform.position.y < surfaceY + 0.1f)
                transform.position = new Vector3(transform.position.x, surfaceY + 0.7f, transform.position.z);
        }
    }
}
