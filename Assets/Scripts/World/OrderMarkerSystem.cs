using UnityEngine;
using System.Collections.Generic;

/// <summary>F4 Orders mode: place order markers on the grid for colonists to execute.</summary>
public class OrderMarkerSystem : MonoBehaviour
{
    public enum OrderKind { Chop, Mine, Harvest, Hunt, Haul, Deconstruct }

    [System.Serializable]
    public class Order
    {
        public OrderKind kind;
        public Vector3Int gridPos;
        public GameObject marker;
        public bool isActive = true;
    }

    public List<Order> orders = new();
    public bool isPlacing;
    public OrderKind selectedKind = OrderKind.Mine;
    private GridManager _grid;
    private Camera _cam;

    void Start()
    {
        _grid = FindFirstObjectByType<GridManager>();
        _cam = Camera.main ?? FindFirstObjectByType<Camera>();
    }

    void Update()
    {
        if (!isPlacing) return;
        if (_cam == null || _grid == null) return;
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = _cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            var hit = _grid.RaycastGrid(ray);
            if (hit != null) PlaceOrder(selectedKind, new Vector3Int(hit.Value.x, hit.Value.y, hit.Value.z));
        }
    }

    void PlaceOrder(OrderKind kind, Vector3Int pos)
    {
        // Remove existing order at same position
        orders.RemoveAll(o => o.gridPos == pos && o.marker != null && DestroyImmediate(o.marker));

        Color c = kind switch
        {
            OrderKind.Chop => Color.green,
            OrderKind.Mine => new Color(0.6f, 0.3f, 0.1f),
            OrderKind.Harvest => Color.yellow,
            OrderKind.Hunt => Color.red,
            OrderKind.Haul => Color.cyan,
            OrderKind.Deconstruct => Color.magenta,
            _ => Color.white
        };

        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
        marker.transform.position = _grid.GridToWorld(pos.x, pos.y, pos.z) + Vector3.up * 0.5f;
        marker.transform.localScale = Vector3.one * 0.6f;
        marker.transform.rotation = Quaternion.Euler(0, 45, 0);
        marker.GetComponent<Renderer>().material.color = c;
        Destroy(marker.GetComponent<Collider>());
        marker.name = $"Order_{kind}_{pos}";

        orders.Add(new Order { kind = kind, gridPos = pos, marker = marker });
    }

    /// <summary>Get nearest active order within range of a position.</summary>
    public Order GetNearestOrder(Vector3 worldPos, float range = 5f)
    {
        Order best = null; float bestDist = range;
        foreach (Order o in orders)
        {
            if (!o.isActive) continue;
            Vector3 wp = _grid.GridToWorld(o.gridPos.x, o.gridPos.y, o.gridPos.z);
            float d = Vector3.Distance(worldPos, wp);
            if (d < bestDist) { bestDist = d; best = o; }
        }
        return best;
    }

    /// <summary>Execute an order. Removes the marker and block/item if applicable.</summary>
    public bool ExecuteOrder(Order order, ColonistAI worker)
    {
        if (order == null || !order.isActive) return false;
        if (!_grid.InBounds(order.gridPos.x, order.gridPos.y, order.gridPos.z)) return false;

        switch (order.kind)
        {
            case OrderKind.Mine:
                BlockType b = _grid.GetBlock(order.gridPos.x, order.gridPos.y, order.gridPos.z);
                if (b != BlockType.Air && b != BlockType.Water)
                {
                    _grid.RemoveBlock(order.gridPos.x, order.gridPos.y, order.gridPos.z);
                    BlockDropManager drop = FindFirstObjectByType<BlockDropManager>();
                    if (drop != null) drop.SpawnDrop(order.gridPos, b);
                }
                break;
            case OrderKind.Chop:
                // Chop nearby wood blocks
                for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                for (int dz = -1; dz <= 1; dz++)
                {
                    Vector3Int p = order.gridPos + new Vector3Int(dx, dy, dz);
                    if (_grid.InBounds(p.x, p.y, p.z) && _grid.GetBlock(p.x, p.y, p.z) == BlockType.Wood)
                    {
                        _grid.RemoveBlock(p.x, p.y, p.z);
                        Inventory inv = worker != null ? worker.GetComponent<Inventory>() : null;
                        if (inv != null) inv.AddItem(ItemType.WoodLog, 3);
                    }
                }
                break;
            case OrderKind.Harvest:
                PlantGrowth[] plants = FindObjectsOfType<PlantGrowth>();
                foreach (PlantGrowth p in plants)
                {
                    if (p == null) continue;
                    Vector3Int pg = _grid.WorldToGrid(p.transform.position);
                    if (Vector3Int.Distance(pg, order.gridPos) <= 2)
                    {
                        Inventory inv = worker != null ? worker.GetComponent<Inventory>() : null;
                        if (inv != null) inv.AddItem(p.cropType, p.growth >= 1f ? Random.Range(2, 5) : 1);
                        Destroy(p.gameObject);
                    }
                }
                break;
            case OrderKind.Hunt:
                AnimalManager am = FindFirstObjectByType<AnimalManager>();
                if (am != null) am.Hunt(order.gridPos, worker != null ? worker.GetComponent<Colonist>() : null);
                break;
            case OrderKind.Deconstruct:
                if (_grid.GetBlock(order.gridPos.x, order.gridPos.y, order.gridPos.z) != BlockType.Air)
                    _grid.RemoveBlock(order.gridPos.x, order.gridPos.y, order.gridPos.z);
                break;
        }

        order.isActive = false;
        if (order.marker != null) Destroy(order.marker);
        return true;
    }
}
