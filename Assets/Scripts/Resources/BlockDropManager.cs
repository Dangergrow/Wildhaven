using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Maps block types to dropped items and manages item pickups in the world.
/// </summary>
public class BlockDropManager : MonoBehaviour
{
    [Header("Drops")]
    public DropEntry[] dropTable;

    [Header("Pickup")]
    public float pickupRadius = 2f;
    public float flySpeed = 5f;

    private GridManager _grid;
    private List<WorldItem> _worldItems = new List<WorldItem>();

    void Awake()
    {
        _grid = GetComponent<GridManager>();
        if (dropTable == null || dropTable.Length == 0)
        {
            dropTable = new DropEntry[]
            {
                new DropEntry { blockType = BlockType.Dirt, dropItem = ItemType.StoneBlock, amount = 1 },
                new DropEntry { blockType = BlockType.Grass, dropItem = ItemType.StoneBlock, amount = 1 },
                new DropEntry { blockType = BlockType.Stone, dropItem = ItemType.StoneBlock, amount = 1 },
                new DropEntry { blockType = BlockType.Wood, dropItem = ItemType.WoodLog, amount = 2 },
                new DropEntry { blockType = BlockType.IronOre, dropItem = ItemType.IronOre, amount = 1 },
                new DropEntry { blockType = BlockType.CopperOre, dropItem = ItemType.CopperOre, amount = 1 },
                new DropEntry { blockType = BlockType.Coal, dropItem = ItemType.Coal, amount = 2 },
                new DropEntry { blockType = BlockType.Snow, dropItem = ItemType.StoneBlock, amount = 1 },
                new DropEntry { blockType = BlockType.Gravel, dropItem = ItemType.StoneBlock, amount = 1 },
                new DropEntry { blockType = BlockType.Sand, dropItem = ItemType.StoneBlock, amount = 1 },
                new DropEntry { blockType = BlockType.WoodPlanks, dropItem = ItemType.WoodLog, amount = 2 },
                new DropEntry { blockType = BlockType.StoneBrick, dropItem = ItemType.StoneBlock, amount = 2 },
            };
        }
    }

    void Start()
    {
        // Subscribe to block removal events
        if (_grid != null)
            _grid.OnBlockRemoved += OnBlockRemoved;
    }

    void OnDestroy()
    {
        if (_grid != null)
            _grid.OnBlockRemoved -= OnBlockRemoved;
    }

    void Update()
    {
        // Fly items toward nearest colonist
        ColonistSpawner spawner = FindObjectOfType<ColonistSpawner>();
        if (spawner == null || spawner.Colonists.Count == 0) return;

        for (int i = _worldItems.Count - 1; i >= 0; i--)
        {
            WorldItem item = _worldItems[i];
            if (item == null || item.gameObject == null) { _worldItems.RemoveAt(i); continue; }
            Colonist nearest = FindNearestColonist(item.transform.position, spawner);

            if (nearest != null && Vector3.Distance(item.transform.position, nearest.transform.position) < pickupRadius)
            {
                // Fly to colonist
                item.transform.position = Vector3.MoveTowards(
                    item.transform.position,
                    nearest.transform.position,
                    flySpeed * Time.deltaTime);

                if (Vector3.Distance(item.transform.position, nearest.transform.position) < 0.3f)
                {
                    // Add to colonist inventory
                    Inventory inv = nearest.GetComponent<Inventory>();
                    if (inv != null)
                        inv.AddItem(item.itemType, item.amount);

                    Destroy(item.gameObject);
                    _worldItems.RemoveAt(i);
                }
            }
        }
    }

    void OnBlockRemoved(int x, int y, int z, BlockType blockType)
    {
        // Find matching drop
        foreach (DropEntry entry in dropTable)
        {
            if (entry.blockType == blockType)
            {
                Vector3 pos = _grid.GridToWorld(x, y, z);
                SpawnItem(entry.dropItem, entry.amount, pos);
                break;
            }
        }
    }

    void SpawnItem(ItemType type, int amount, Vector3 position)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = position + Vector3.up * 0.3f;
        go.transform.localScale = Vector3.one * 0.25f;
        go.GetComponent<Renderer>().material.color = type switch
        {
            ItemType.WoodLog => new Color(0.55f, 0.27f, 0.07f),
            ItemType.StoneBlock => Color.gray,
            ItemType.IronOre => new Color(0.7f, 0.4f, 0.3f),
            ItemType.CopperOre => new Color(0.8f, 0.5f, 0.3f),
            ItemType.Coal => Color.black,
            ItemType.Berries => Color.red,
            ItemType.Wheat => Color.yellow,
            _ => Color.yellow,
        };
        WorldItem item = go.AddComponent<WorldItem>();
        item.itemType = type;
        item.amount = amount;
        item.spawnVelocity = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(1f, 3f), Random.Range(-0.5f, 0.5f));
        _worldItems.Add(item);
    }

    Colonist FindNearestColonist(Vector3 pos, ColonistSpawner spawner)
    {
        Colonist nearest = null;
        float minDist = pickupRadius;
        foreach (Colonist c in spawner.Colonists)
        {
            if (c == null || c.currentState == ColonistState.Dead) continue;
            float d = Vector3.Distance(pos, c.transform.position);
            if (d < minDist) { minDist = d; nearest = c; }
        }
        return nearest;
    }
}

[System.Serializable]
public struct DropEntry
{
    public BlockType blockType;
    public ItemType dropItem;
    public int amount;
}

/// <summary>
/// Represents an item GameObject dropped in the world.
/// Has physics: gravity, bouncing, settling on ground.
/// </summary>
public class WorldItem : MonoBehaviour
{
    public ItemType itemType;
    public int amount;
    public Vector3 spawnVelocity;
    public bool hasLanded;

    private float _lifetime = 30f;
    private float _bounceTimer;

    void Update()
    {
        _lifetime -= Time.deltaTime;
        if (_lifetime < 0f) { Destroy(gameObject); return; }

        if (!hasLanded)
        {
            // Gravity
            spawnVelocity.y -= 12f * Time.deltaTime;
            transform.position += spawnVelocity * Time.deltaTime;

            // Check ground
            GridManager grid = FindObjectOfType<GridManager>();
            if (grid != null)
            {
                Vector3Int below = grid.WorldToGrid(transform.position + Vector3.down * 0.2f);
                BlockType block = grid.GetBlock(below.x, below.y, below.z);
                if (block != BlockType.Air && block != BlockType.Water && spawnVelocity.y < 0f)
                {
                    // Land
                    Vector3 ground = grid.GridToWorld(below.x, below.y, below.z);
                    transform.position = new Vector3(transform.position.x, ground.y + 0.15f + grid.BlockSize * 0.5f, transform.position.z);
                    spawnVelocity = Vector3.zero;
                    hasLanded = true;
                }
            }
        }
    }
}
