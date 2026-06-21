using UnityEngine;
using System.Collections.Generic;

/// <summary>Animal system: wild animals spawn, can be hunted, tamed, bred.</summary>
public class AnimalManager : MonoBehaviour
{
    private GridManager _grid;
    private List<Animal> _animals = new();
    private float _timer;

    public enum AnimalType { Deer, Wolf, Bear, Chicken, Goat, Cow, Sheep, Pig, Horse, Rabbit, Fox, Boar }
    
    [System.Serializable]
    public class Animal
    {
        public AnimalType type;
        public GameObject gameObject;
        public bool isTamed;
        public float health = 100;
        public float hunger = 100;
        public float growth; // 0-1 for breeding readiness
        public Vector3Int gridPos;
    }

    void Awake()
    {
        _grid = GetComponent<GridManager>();
        if (_grid == null) _grid = FindFirstObjectByType<GridManager>();
        SpawnInitial();
    }

    void SpawnInitial()
    {
        if (_grid == null) return;
        var rng = new System.Random(System.DateTime.Now.Millisecond);
        for (int i = 0; i < 15; i++)
        {
            int x = rng.Next(5, _grid.Width - 5);
            int z = rng.Next(5, _grid.Depth - 5);
            for (int y = _grid.Height - 1; y > 2; y--)
            {
                if (_grid.GetBlock(x, y, z) == BlockType.Air && _grid.GetBlock(x, y - 1, z) == BlockType.Grass)
                {
                    SpawnAnimal((AnimalType)rng.Next(6), new Vector3Int(x, y, z)); // 6 wild types
                    break;
                }
            }
        }
        Debug.Log($"[Animals] Spawned {_animals.Count} wild animals");
    }

    void SpawnAnimal(AnimalType type, Vector3Int pos)
    {
        var go = new GameObject($"Animal_{type}_{_animals.Count}");
        go.transform.position = _grid.GridToWorld(pos.x, pos.y, pos.z);
        var animal = new Animal { type = type, gameObject = go, gridPos = pos };
        _animals.Add(animal);
    }

    void Update()
    {
        _timer += Time.unscaledDeltaTime;
        if (_timer < 10f) return;
        _timer = 0f;

        // Animal behavior: wander, breed if tamed
        foreach (var a in _animals)
        {
            if (a.health <= 0) continue;
            // Wander
            Vector3Int newPos = a.gridPos + new Vector3Int(Random.Range(-1, 2), 0, Random.Range(-1, 2));
            if (_grid.InBounds(newPos.x, newPos.y, newPos.z) && _grid.GetBlock(newPos.x, newPos.y, newPos.z) == BlockType.Air)
            {
                a.gridPos = newPos;
                a.gameObject.transform.position = _grid.GridToWorld(newPos.x, newPos.y, newPos.z);
            }
            a.hunger -= 1f;

            // Tamed animals breed (10% chance)
            if (a.isTamed && a.growth >= 1f && Random.value < 0.1f)
            {
                SpawnAnimal(a.type, a.gridPos);
                a.growth = 0f;
            }
            if (a.isTamed) a.growth += 0.05f;
        }

        // Remove dead animals
        _animals.RemoveAll(a => a.health <= 0);
    }

    /// <summary>Hunt an animal at grid position. Returns loot.</summary>
    public (ItemType, int)? Hunt(Vector3Int pos, Colonist hunter)
    {
        foreach (var a in _animals)
        {
            if (Vector3Int.Distance(a.gridPos, pos) <= 1)
            {
                float dmg = 20 + hunter.meleeSkill * 2;
                a.health -= dmg;
                if (a.health <= 0)
                {
                    var loot = GetLoot(a.type);
                    Object.Destroy(a.gameObject);
                    return loot;
                }
            }
        }
        return null;
    }

    /// <summary>Tame a wild animal.</summary>
    public bool Tame(Vector3Int pos, Colonist tamer)
    {
        foreach (var a in _animals)
        {
            if (Vector3Int.Distance(a.gridPos, pos) <= 1 && !a.isTamed)
            {
                float chance = tamer.animalHandlingSkill * 0.05f; // 5% per skill level
                if (Random.value < chance) { a.isTamed = true; return true; }
            }
        }
        return false;
    }

    (ItemType, int) GetLoot(AnimalType t) => t switch
    {
        AnimalType.Deer => (ItemType.RawMeat, 5),
        AnimalType.Wolf => (ItemType.RawMeat, 3),
        AnimalType.Bear => (ItemType.RawMeat, 8),
        AnimalType.Chicken => (ItemType.RawMeat, 2),
        AnimalType.Goat => (ItemType.RawMeat, 4),
        AnimalType.Cow => (ItemType.RawMeat, 10),
        AnimalType.Sheep => (ItemType.RawMeat, 5),
        AnimalType.Pig => (ItemType.RawMeat, 6),
        AnimalType.Rabbit => (ItemType.RawMeat, 1),
        AnimalType.Fox => (ItemType.RawMeat, 2),
        AnimalType.Boar => (ItemType.RawMeat, 4),
        _ => (ItemType.RawMeat, 2),
    };
}
