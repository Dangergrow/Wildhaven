using UnityEngine;
using System.Collections.Generic;

/// <summary>Spawns forageable items on terrain surface: berries, mushrooms, herbs.</summary>
public class ForageSpawner : MonoBehaviour
{
    private GridManager _grid;
    private List<ForageItem> _items = new();

    [System.Serializable]
    public class ForageItem
    {
        public ItemType type;
        public Vector3Int pos;
        public float respawnDay;
    }

    void Awake()
    {
        _grid = GetComponent<GridManager>();
        if (_grid == null) _grid = FindObjectOfType<GridManager>();
        SpawnInitial();
    }

    void SpawnInitial()
    {
        if (_grid == null) return;
        var rng = new System.Random(42);
        for (int i = 0; i < 50; i++)
        {
            int x = rng.Next(5, _grid.Width - 5);
            int z = rng.Next(5, _grid.Depth - 5);
            // Find surface
            for (int y = _grid.Height - 1; y > 2; y--)
            {
                if (_grid.GetBlock(x, y, z) == BlockType.Air && _grid.GetBlock(x, y - 1, z) == BlockType.Grass)
                {
                    ItemType type = rng.Next(3) switch { 0 => ItemType.Berries, 1 => ItemType.Mushroom, _ => ItemType.MedicalHerb };
                    _items.Add(new ForageItem { type = type, pos = new Vector3Int(x, y, z) });
                    break;
                }
            }
        }
        Debug.Log($"[Forage] Spawned {_items.Count} forage items");
    }

    /// <summary>Try to harvest at a position. Returns item and amount, or null.</summary>
    public (ItemType, int)? TryHarvest(Vector3Int pos)
    {
        for (int i = _items.Count - 1; i >= 0; i--)
        {
            if (_items[i].pos == pos)
            {
                var item = _items[i];
                int amount = item.type == ItemType.Berries ? 3 : (item.type == ItemType.Mushroom ? 2 : 1);
                _items.RemoveAt(i);
                return (item.type, amount);
            }
        }
        return null;
    }

    /// <summary>Check if position has a forage item (for visual indicator).</summary>
    public bool HasForage(Vector3Int pos)
    {
        foreach (var i in _items) if (i.pos == pos) return true;
        return false;
    }

    /// <summary>Draw forage indicators in editor.</summary>
    void OnDrawGizmos()
    {
        if (_grid == null) return;
        Gizmos.color = Color.green;
        foreach (var i in _items)
        {
            Vector3 p = _grid.GridToWorld(i.pos.x, i.pos.y, i.pos.z);
            Gizmos.DrawWireSphere(p, 0.3f);
        }
    }
}
