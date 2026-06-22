using UnityEngine;
using System.Collections.Generic;

/// <summary>Animal system: wild animals spawn, can be hunted, tamed, bred.</summary>
public class AnimalManager : MonoBehaviour
{
    private GridManager _grid;
    private List<Animal> _animals = new();
    private float _timer;

    public enum AnimalType { Deer, Wolf, Bear, Chicken, Goat, Cow, Sheep, Pig, Horse, Rabbit, Fox, Boar, Camel, Mammoth, Llama, Ostrich, Tiger, Crocodile, GiantSpider, Eagle }
    
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

    /// <summary>Visual colors for each animal type.</summary>
    private static readonly System.Collections.Generic.Dictionary<AnimalType, Color> AnimalColors = new()
    {
        {AnimalType.Deer, new Color(0.55f, 0.35f, 0.15f)},
        {AnimalType.Wolf, new Color(0.3f, 0.3f, 0.3f)},
        {AnimalType.Bear, new Color(0.25f, 0.15f, 0.05f)},
        {AnimalType.Chicken, Color.white},
        {AnimalType.Goat, new Color(0.6f, 0.5f, 0.4f)},
        {AnimalType.Cow, new Color(0.4f, 0.3f, 0.2f)},
        {AnimalType.Sheep, new Color(0.9f, 0.9f, 0.85f)},
        {AnimalType.Pig, new Color(0.95f, 0.75f, 0.8f)},
        {AnimalType.Horse, new Color(0.35f, 0.2f, 0.1f)},
        {AnimalType.Rabbit, new Color(0.7f, 0.5f, 0.3f)},
        {AnimalType.Fox, new Color(0.9f, 0.4f, 0.1f)},
        {AnimalType.Boar, new Color(0.3f, 0.2f, 0.1f)},
        {AnimalType.Camel, new Color(0.85f, 0.7f, 0.4f)},
        {AnimalType.Mammoth, new Color(0.4f, 0.25f, 0.15f)},
        {AnimalType.Llama, new Color(0.8f, 0.7f, 0.55f)},
        {AnimalType.Ostrich, new Color(0.2f, 0.2f, 0.2f)},
        {AnimalType.Tiger, new Color(0.95f, 0.5f, 0.05f)},
        {AnimalType.Crocodile, new Color(0.15f, 0.4f, 0.15f)},
        {AnimalType.GiantSpider, new Color(0.1f, 0.05f, 0.05f)},
        {AnimalType.Eagle, new Color(0.4f, 0.25f, 0.05f)},
    };

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
        string planetStr = PlayerPrefs.GetString("PlanetType", "Earthlike");

        // Weighted animal types by planet
        var weights = GetPlanetWeights(planetStr);
        AnimalType[] allTypes = (AnimalType[])System.Enum.GetValues(typeof(AnimalType));
        int totalWeight = 0;
        foreach (var w in weights.Values) totalWeight += w;

        for (int i = 0; i < 25; i++)
        {
            int x = rng.Next(5, _grid.Width - 5);
            int z = rng.Next(5, _grid.Depth - 5);
            for (int y = _grid.Height - 1; y > 2; y--)
            {
                if (_grid.GetBlock(x, y, z) == BlockType.Air && _grid.GetBlock(x, y - 1, z) == BlockType.Grass)
                {
                    // Weighted random selection
                    int roll = rng.Next(totalWeight);
                    AnimalType chosen = AnimalType.Deer;
                    int cumulative = 0;
                    foreach (var kvp in weights)
                    {
                        cumulative += kvp.Value;
                        if (roll < cumulative) { chosen = kvp.Key; break; }
                    }
                    SpawnAnimal(chosen, new Vector3Int(x, y, z));
                    break;
                }
            }
        }
        Debug.Log($"[Animals] Spawned {_animals.Count} wild animals (planet: {planetStr})");
    }

    Dictionary<AnimalType, int> GetPlanetWeights(string planet)
    {
        var w = new Dictionary<AnimalType, int>();
        // Default weights
        foreach (AnimalType t in System.Enum.GetValues(typeof(AnimalType)))
            w[t] = 1;

        switch (planet)
        {
            case "DesertWorld":
                w[AnimalType.Camel] = 8; w[AnimalType.Deer] = 0; w[AnimalType.Bear] = 0;
                w[AnimalType.Fox] = 4; w[AnimalType.Rabbit] = 4; w[AnimalType.Boar] = 2;
                break;
            case "IceWorld":
                w[AnimalType.Mammoth] = 6; w[AnimalType.Wolf] = 5; w[AnimalType.Fox] = 3;
                w[AnimalType.Rabbit] = 4; w[AnimalType.Deer] = 3; w[AnimalType.Bear] = 4;
                w[AnimalType.Camel] = 0; w[AnimalType.Crocodile] = 0; w[AnimalType.Tiger] = 0;
                break;
            case "JungleWorld":
                w[AnimalType.Tiger] = 6; w[AnimalType.Crocodile] = 5; w[AnimalType.GiantSpider] = 4;
                w[AnimalType.Ostrich] = 4; w[AnimalType.Eagle] = 3; w[AnimalType.Boar] = 3;
                w[AnimalType.Camel] = 0; w[AnimalType.Mammoth] = 0;
                break;
            case "DeadWorld":
                w[AnimalType.GiantSpider] = 8; w[AnimalType.Wolf] = 4; w[AnimalType.Fox] = 2;
                w[AnimalType.Rabbit] = 2; w[AnimalType.Eagle] = 3;
                w[AnimalType.Deer] = 0; w[AnimalType.Chicken] = 0; w[AnimalType.Cow] = 0;
                break;
            default: // Earthlike — balanced
                w[AnimalType.Deer] = 4; w[AnimalType.Rabbit] = 4; w[AnimalType.Wolf] = 2;
                w[AnimalType.Bear] = 1; w[AnimalType.Fox] = 2; w[AnimalType.Boar] = 2;
                break;
        }
        return w;
    }

    void SpawnAnimal(AnimalType type, Vector3Int pos)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        go.name = $"Animal_{type}_{_animals.Count}";
        go.transform.position = _grid.GridToWorld(pos.x, pos.y, pos.z);
        float size = type switch
        {
            AnimalType.Mammoth => 1.8f,
            AnimalType.Bear => 1.5f,
            AnimalType.Horse or AnimalType.Camel or AnimalType.Tiger or AnimalType.Ostrich => 1.2f,
            AnimalType.Cow or AnimalType.Llama or AnimalType.Crocodile => 1.0f,
            AnimalType.Deer or AnimalType.Wolf or AnimalType.Sheep or AnimalType.Pig or AnimalType.Boar => 0.8f,
            AnimalType.Goat or AnimalType.Fox or AnimalType.Eagle => 0.6f,
            AnimalType.Chicken or AnimalType.Rabbit => 0.35f,
            AnimalType.GiantSpider => 0.4f,
            _ => 0.7f
        };
        go.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f) * size;
        go.transform.rotation = Quaternion.Euler(90, 0, 0);
        Destroy(go.GetComponent<Collider>());
        var r = go.GetComponent<Renderer>();
        if (r != null && AnimalColors.TryGetValue(type, out Color col)) r.material.color = col;
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
        AnimalType.Camel => (ItemType.RawMeat, 7),
        AnimalType.Mammoth => (ItemType.RawMeat, 15),
        AnimalType.Llama => (ItemType.RawMeat, 5),
        AnimalType.Ostrich => (ItemType.RawMeat, 4),
        AnimalType.Tiger => (ItemType.RawMeat, 6),
        AnimalType.Crocodile => (ItemType.RawMeat, 7),
        AnimalType.GiantSpider => (ItemType.RawMeat, 1),
        AnimalType.Eagle => (ItemType.RawMeat, 1),
        AnimalType.Horse => (ItemType.RawMeat, 8),
        _ => (ItemType.RawMeat, 2),
    };
}
