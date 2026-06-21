using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class BuildManager : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private Camera _cam;
    [SerializeField] private BlockType _selectedType = BlockType.Dirt;
    public BlockType SelectedType => _selectedType;
    public void SetSelectedType(BlockType t) => _selectedType = t;

    private Vector3Int _placementPos;
    private bool _canPlace;
    private bool _isDragging;
    private Vector3Int _dragStart;
    private HashSet<Vector3Int> _placedThisDrag = new();

    void Update()
    {
        if (Keyboard.current == null || Mouse.current == null || _gridManager == null || _cam == null) return;

        // Block selection: 1-9 basic, Shift+1-9 advanced
        if (Keyboard.current.digit1Key.wasPressedThisFrame) _selectedType = Keyboard.current.leftShiftKey.isPressed ? BlockType.Marble : BlockType.Dirt;
        if (Keyboard.current.digit2Key.wasPressedThisFrame) _selectedType = Keyboard.current.leftShiftKey.isPressed ? BlockType.Obsidian : BlockType.Grass;
        if (Keyboard.current.digit3Key.wasPressedThisFrame) _selectedType = Keyboard.current.leftShiftKey.isPressed ? BlockType.CopperOre : BlockType.Stone;
        if (Keyboard.current.digit4Key.wasPressedThisFrame) _selectedType = Keyboard.current.leftShiftKey.isPressed ? BlockType.GoldOre : BlockType.Wood;
        if (Keyboard.current.digit5Key.wasPressedThisFrame) _selectedType = Keyboard.current.leftShiftKey.isPressed ? BlockType.IronOre : BlockType.Glass;
        if (Keyboard.current.digit6Key.wasPressedThisFrame) _selectedType = Keyboard.current.leftShiftKey.isPressed ? BlockType.Coal : BlockType.StoneBrick;
        if (Keyboard.current.digit7Key.wasPressedThisFrame) _selectedType = Keyboard.current.leftShiftKey.isPressed ? BlockType.Ice : BlockType.WoodPlanks;
        if (Keyboard.current.digit8Key.wasPressedThisFrame) _selectedType = Keyboard.current.leftShiftKey.isPressed ? BlockType.Clay : BlockType.Sand;
        if (Keyboard.current.digit9Key.wasPressedThisFrame) _selectedType = Keyboard.current.leftShiftKey.isPressed ? BlockType.Gravel : BlockType.Snow;

        if (Keyboard.current.f5Key.wasPressedThisFrame) { var gsm = FindFirstObjectByType<GameSaveManager>(); if (gsm != null) gsm.SaveGame(); }
        if (Keyboard.current.f9Key.wasPressedThisFrame) { var gsm = FindFirstObjectByType<GameSaveManager>(); if (gsm != null) gsm.LoadGame(); }

        Ray ray = _cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        var hit = _gridManager.RaycastGrid(ray);
        if (hit == null) return;

        // Ctrl+C copy, Ctrl+V paste
        if (Keyboard.current.leftCtrlKey.isPressed)
        {
            if (Keyboard.current.cKey.wasPressedThisFrame)
            {
                var clip = _gridManager.GetComponent<StructureClipboard>();
                if (clip == null) clip = _gridManager.gameObject.AddComponent<StructureClipboard>();
                clip.Copy(new Vector3Int(hit.Value.x, hit.Value.y, hit.Value.z));
            }
            if (Keyboard.current.vKey.wasPressedThisFrame)
            {
                var clip = _gridManager.GetComponent<StructureClipboard>();
                if (clip != null && clip.HasCopy)
                    clip.Paste(new Vector3Int(hit.Value.x, hit.Value.y, hit.Value.z));
            }
        }

        Vector3 hw = _gridManager.GridToWorld(hit.Value.x, hit.Value.y, hit.Value.z);
        Vector3 dir = (_cam.transform.position - hw).normalized;
        Vector3Int[] offs = { new(0,1,0), new(0,-1,0), new(1,0,0), new(-1,0,0), new(0,0,1), new(0,0,-1) };
        int best = 0; float bestDot = -1;
        for (int i = 0; i < 6; i++) { float d = Vector3.Dot(offs[i], dir); if (d > bestDot) { bestDot = d; best = i; } }
        Vector3Int ap = new(hit.Value.x + offs[best].x, hit.Value.y + offs[best].y, hit.Value.z + offs[best].z);

        // Drag build: LMB held down = line/area of blocks
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            _isDragging = true;
            _dragStart = ap;
            _placedThisDrag.Clear();
            PlaceBlock(ap); // first block
        }
        if (Mouse.current.leftButton.isPressed && _isDragging && ap != _dragStart)
        {
            // Build line between drag start and current position
            foreach (var pos in Line3D(_dragStart, ap))
            {
                if (!_placedThisDrag.Contains(pos))
                {
                    PlaceBlock(pos);
                    _placedThisDrag.Add(pos);
                }
            }
        }
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            _isDragging = false;
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            if (Keyboard.current.leftShiftKey.isPressed)
            {
                // Shift+RMB = demolish 3x3 area
                for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                for (int dz = -1; dz <= 1; dz++)
                    _gridManager.RemoveBlock(hit.Value.x + dx, hit.Value.y + dy, hit.Value.z + dz);
            }
            else
                _gridManager.RemoveBlock(hit.Value.x, hit.Value.y, hit.Value.z);
        }
    }

    // OnGUI removed — use CanvasHUD instead

    void PlaceBlock(Vector3Int pos)
    {
        if (!_gridManager.InBounds(pos.x, pos.y, pos.z)) return;
        if (_gridManager.GetBlock(pos.x, pos.y, pos.z) != BlockType.Air) return;
        if (Keyboard.current.leftShiftKey.isPressed)
        {
            BlueprintManager bpm = _gridManager.GetComponent<BlueprintManager>();
            if (bpm == null) bpm = _gridManager.gameObject.AddComponent<BlueprintManager>();
            bpm.AddBlueprint(pos, _selectedType);
        }
        else _gridManager.SetBlock(pos.x, pos.y, pos.z, _selectedType);
    }

    /// <summary>Bresenham 3D line between two grid points.</summary>
    IEnumerable<Vector3Int> Line3D(Vector3Int a, Vector3Int b)
    {
        int dx = Mathf.Abs(b.x - a.x), dy = Mathf.Abs(b.y - a.y), dz = Mathf.Abs(b.z - a.z);
        int sx = a.x < b.x ? 1 : -1, sy = a.y < b.y ? 1 : -1, sz = a.z < b.z ? 1 : -1;
        int x = a.x, y = a.y, z = a.z;
        if (dx >= dy && dx >= dz)
        {
            int p1 = 2 * dy - dx, p2 = 2 * dz - dx;
            while (x != b.x) { yield return new(x, y, z); x += sx; if (p1 >= 0) { y += sy; p1 -= 2 * dx; } p1 += 2 * dy; if (p2 >= 0) { z += sz; p2 -= 2 * dx; } p2 += 2 * dz; }
        }
        else if (dy >= dx && dy >= dz)
        {
            int p1 = 2 * dx - dy, p2 = 2 * dz - dy;
            while (y != b.y) { yield return new(x, y, z); y += sy; if (p1 >= 0) { x += sx; p1 -= 2 * dy; } p1 += 2 * dx; if (p2 >= 0) { z += sz; p2 -= 2 * dy; } p2 += 2 * dz; }
        }
        else
        {
            int p1 = 2 * dx - dz, p2 = 2 * dy - dz;
            while (z != b.z) { yield return new(x, y, z); z += sz; if (p1 >= 0) { x += sx; p1 -= 2 * dz; } p1 += 2 * dx; if (p2 >= 0) { y += sy; p2 -= 2 * dz; } p2 += 2 * dy; }
        }
        yield return b;
    }
}
