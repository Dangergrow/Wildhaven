using UnityEngine;
using System.IO;
using System.Collections.Generic;

/// <summary>Full game save/load: world + colonists + inventory + research + time.</summary>
public class GameSaveManager : MonoBehaviour
{
    private GridManager _grid;
    private DayCycle _day;
    private ColonistSpawner _spawner;

    string SavePath => Path.Combine(Application.persistentDataPath, "game.sav");

    void Awake()
    {
        _grid = FindObjectOfType<GridManager>();
        _day = FindObjectOfType<DayCycle>();
        _spawner = FindObjectOfType<ColonistSpawner>();
    }

    /// <summary>Save everything to disk.</summary>
    public void SaveGame()
    {
        using (var w = new BinaryWriter(File.Open(SavePath, FileMode.Create)))
        {
            w.Write(new byte[]{0x57, 0x48, 0x53, 0x56}); // "WHSV" magic
            w.Write(2); // version

            // World
            _grid.SaveToStream(w);

            // Time
            w.Write(_day != null ? _day.day : 1);
            w.Write(_day != null ? _day.hour : 8);
            w.Write((byte)(_day != null ? _day.minute : 0));
            w.Write(_day != null ? _day.gameSpeed : 1f);

            // Colonists
            var colonists = _spawner != null ? _spawner.Colonists : new List<Colonist>();
            w.Write(colonists.Count);
            foreach (var c in colonists) SaveColonist(w, c);
        }
        Debug.Log($"[Save] Full game saved: {SavePath}");
    }

    /// <summary>Load everything from disk.</summary>
    public void LoadGame()
    {
        if (!File.Exists(SavePath)) return;
        using (var r = new BinaryReader(File.OpenRead(SavePath)))
        {
            if (r.ReadByte() != 0x57 || r.ReadByte() != 0x48 || r.ReadByte() != 0x53 || r.ReadByte() != 0x56) return;
            int ver = r.ReadInt32();

            // World
            _grid.LoadFromStream(r);
            _grid.BuildAllChunks();

            // Time
            if (_day != null) { _day.day = r.ReadInt32(); _day.hour = r.ReadInt32(); _day.minute = r.ReadByte(); _day.gameSpeed = r.ReadSingle(); }

            // Colonists
            int count = r.ReadInt32();
            if (_spawner != null)
            {
                // Remove existing colonists
                foreach (var c in _spawner.Colonists.ToArray())
                    Destroy(c.gameObject);
                _spawner.Colonists.Clear();
            }
            for (int i = 0; i < count; i++)
                LoadColonist(r);
        }
        Debug.Log($"[Load] Full game loaded: {SavePath}");
    }

    public bool HasSave => File.Exists(SavePath);

    void SaveColonist(BinaryWriter w, Colonist c)
    {
        w.Write(c.colonistName ?? "Unknown");
        w.Write(c.age); w.Write(c.isMale);
        w.Write(c.health); w.Write(c.maxHealth);
        w.Write(c.mood); w.Write(c.hunger); w.Write(c.thirst);
        w.Write(c.fatigue); w.Write(c.comfort); w.Write(c.social); w.Write(c.recreation); w.Write(c.faith);
        w.Write((int)c.currentState);
        w.Write(c.factionId);

        // Position
        var pos = c.transform.position;
        w.Write(pos.x); w.Write(pos.y); w.Write(pos.z);

        // Inventory
        var inv = c.GetComponent<Inventory>();
        w.Write(inv != null ? 1 : 0);
        if (inv != null)
        {
            int itemCount = 0;
            foreach (var slot in inv.Slots) if (slot.amount > 0) itemCount++;
            w.Write(itemCount);
            foreach (var slot in inv.Slots)
                if (slot.amount > 0) { w.Write((int)slot.itemType); w.Write(slot.amount); }
        }

        // Research — skip for now
    }

    void LoadColonist(BinaryReader r)
    {
        if (_spawner == null || _spawner.colonistPrefab == null) return;

        string name = r.ReadString();
        int age = r.ReadInt32(); bool male = r.ReadBoolean();
        float hp = r.ReadSingle(), maxHp = r.ReadSingle();
        float mood = r.ReadSingle(), hunger = r.ReadSingle(), thirst = r.ReadSingle();
        float fatigue = r.ReadSingle(), comfort = r.ReadSingle(), social = r.ReadSingle(), rec = r.ReadSingle(), faith = r.ReadSingle();
        ColonistState state = (ColonistState)r.ReadInt32();
        int faction = r.ReadInt32();
        float px = r.ReadSingle(), py = r.ReadSingle(), pz = r.ReadSingle();

        var go = Instantiate(_spawner.colonistPrefab, new Vector3(px, py, pz), Quaternion.identity);
        var c = go.GetComponent<Colonist>();
        if (c == null) { Destroy(go); return; }

        c.colonistName = name; c.age = age; c.isMale = male;
        c.health = hp; c.maxHealth = maxHp;
        c.mood = mood; c.hunger = hunger; c.thirst = thirst;
        c.fatigue = fatigue; c.comfort = comfort; c.social = social; c.recreation = rec; c.faith = faith;
        c.currentState = state; c.factionId = faction;

        int hasInv = r.ReadInt32();
        if (hasInv == 1)
        {
            var inv = go.GetComponent<Inventory>();
            if (inv == null) inv = go.AddComponent<Inventory>();
            int itemCount = r.ReadInt32();
            for (int i = 0; i < itemCount; i++)
            {
                ItemType type = (ItemType)r.ReadInt32();
                int amount = r.ReadInt32();
                if (inv != null) inv.AddItem(type, amount);
            }
        }

        // Research skip
        // (no data saved yet)

        if (_spawner != null) _spawner.Colonists.Add(c);
    }
}
