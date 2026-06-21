using UnityEngine;
using System.Collections.Generic;

/// <summary>Medical beds, roof auto-build, hauling, cleaning — colony maintenance.</summary>
public class ColonyServices : MonoBehaviour
{
    private GridManager _grid;
    private ColonistSpawner _spawner;

    // Medical beds
    public HashSet<Vector3Int> medicalBeds = new();
    // Stockpile positions
    public HashSet<Vector3Int> stockpilePositions = new();
    // Dirty positions (need cleaning)
    private HashSet<Vector3Int> _dirtyPositions = new();

    void Awake()
    {
        _grid = GetComponent<GridManager>();
        if (_grid == null) _grid = FindFirstObjectByType<GridManager>();
        _spawner = FindFirstObjectByType<ColonistSpawner>();
    }

    /// <summary>Designate a medical bed at position.</summary>
    public void DesignateMedicalBed(Vector3Int pos) => medicalBeds.Add(pos);
    public void RemoveMedicalBed(Vector3Int pos) => medicalBeds.Remove(pos);

    /// <summary>Mark position as dirty (needs cleaning).</summary>
    public void MarkDirty(Vector3Int pos) => _dirtyPositions.Add(pos);

    /// <summary>Find nearest medical bed to a wounded colonist.</summary>
    public Vector3Int? FindMedicalBed(Vector3 pos)
    {
        Vector3Int? best = null;
        float bestDist = float.MaxValue;
        foreach (var bed in medicalBeds)
        {
            float d = Vector3.Distance(_grid.GridToWorld(bed.x, bed.y, bed.z), pos);
            if (d < bestDist) { bestDist = d; best = bed; }
        }
        return best;
    }

    /// <summary>Auto-heal wounded colonists. Called periodically.</summary>
    public void AutoHeal()
    {
        if (_spawner == null) return;
        foreach (var c in _spawner.Colonists)
        {
            if (c.health < c.maxHealth * 0.7f && c.currentState != ColonistState.Dead)
            {
                var bed = FindMedicalBed(c.transform.position);
                if (bed != null)
                {
                    var doc = FindDoctor();
                    if (doc != null)
                    {
                        float dist = Vector3.Distance(doc.transform.position, c.transform.position);
                        if (dist < 5f)
                        {
                            var med = GetComponent<MedicineSystem>();
                            if (med == null) med = gameObject.AddComponent<MedicineSystem>();
                            med.Treat(c, c.GetComponent<Inventory>());
                        }
                    }
                }
            }
        }
    }

    Colonist FindDoctor()
    {
        if (_spawner == null) return null;
        Colonist best = null; int bestSkill = 0;
        foreach (var c in _spawner.Colonists)
        {
            if (c.currentState == ColonistState.Dead) continue;
            if (c.medicineSkill > bestSkill) { bestSkill = c.medicineSkill; best = c; }
        }
        return best;
    }

    /// <summary>Try to haul item to nearest stockpile.</summary>
    public bool TryHaul(Vector3 itemPos, ItemType item)
    {
        if (_spawner == null || stockpilePositions.Count == 0) return false;
        Colonist hauler = null; float bestDist = float.MaxValue;
        foreach (var c in _spawner.Colonists)
        {
            if (c.currentState == ColonistState.Dead || c.currentState == ColonistState.Sleeping) continue;
            float d = Vector3.Distance(c.transform.position, itemPos);
            if (d < bestDist && d < 20f) { bestDist = d; hauler = c; }
        }
        if (hauler == null) return false;

        var inv = hauler.GetComponent<Inventory>();
        if (inv != null) inv.AddItem(item, 1);
        return true;
    }

    /// <summary>Try to clean a dirty position.</summary>
    public bool TryClean(Vector3 colonistPos)
    {
        if (_dirtyPositions.Count == 0) return false;
        Vector3Int? target = null; float best = float.MaxValue;
        foreach (var d in _dirtyPositions)
        {
            float dist = Vector3.Distance(_grid.GridToWorld(d.x, d.y, d.z), colonistPos);
            if (dist < best && dist < 5f) { best = dist; target = d; }
        }
        if (target != null)
        {
            _dirtyPositions.Remove(target.Value);
            return true;
        }
        return false;
    }

    /// <summary>Auto-build roof over enclosed rooms.</summary>
    public void AutoRoof()
    {
        if (_grid == null) return;
        for (int x = 1; x < _grid.Width - 1; x++)
        for (int z = 1; z < _grid.Depth - 1; z++)
        {
            // Check if 4 walls surround this position
            bool hasWalls = true;
            Vector3Int[] dirs = { new(1,0,0), new(-1,0,0), new(0,0,1), new(0,0,-1) };
            int maxY = 0;
            foreach (var d in dirs)
            {
                int wallY = FindWallHeight(x + d.x, z + d.z);
                if (wallY < 0) { hasWalls = false; break; }
                if (wallY > maxY) maxY = wallY;
            }
            if (!hasWalls) continue;

            // Place roof block above the highest wall
            if (_grid.GetBlock(x, maxY + 1, z) == BlockType.Air)
                _grid.SetBlock(x, maxY + 1, z, BlockType.Wood);
        }
    }

    int FindWallHeight(int x, int z)
    {
        for (int y = _grid.Height - 1; y > 0; y--)
            if (_grid.GetBlock(x, y, z) != BlockType.Air)
                return y;
        return -1;
    }
}
