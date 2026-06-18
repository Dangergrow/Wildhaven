using UnityEngine;
using UnityEngine.InputSystem;

public class BuildManager : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private Camera _cam;
    [SerializeField] private BlockType _selectedType = BlockType.Dirt;

    private Vector3Int _placementPos;
    private bool _canPlace;

    void Update()
    {
        if (Keyboard.current == null || Mouse.current == null || _gridManager == null || _cam == null) return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame) _selectedType = BlockType.Dirt;
        if (Keyboard.current.digit2Key.wasPressedThisFrame) _selectedType = BlockType.Grass;
        if (Keyboard.current.digit3Key.wasPressedThisFrame) _selectedType = BlockType.Stone;
        if (Keyboard.current.digit4Key.wasPressedThisFrame) _selectedType = BlockType.Wood;
        if (Keyboard.current.digit5Key.wasPressedThisFrame) _selectedType = BlockType.Glass;
        if (Keyboard.current.digit6Key.wasPressedThisFrame) _selectedType = BlockType.StoneBrick;
        if (Keyboard.current.digit7Key.wasPressedThisFrame) _selectedType = BlockType.WoodPlanks;
        if (Keyboard.current.digit8Key.wasPressedThisFrame) _selectedType = BlockType.Sand;
        if (Keyboard.current.digit9Key.wasPressedThisFrame) _selectedType = BlockType.Snow;

        if (Keyboard.current.f5Key.wasPressedThisFrame) _gridManager.SaveWorld();
        if (Keyboard.current.f9Key.wasPressedThisFrame) _gridManager.LoadWorld();

        Ray ray = _cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        var hit = _gridManager.RaycastGrid(ray);
        if (hit == null) return;

        Vector3 hw = _gridManager.GridToWorld(hit.Value.x, hit.Value.y, hit.Value.z);
        Vector3 dir = (_cam.transform.position - hw).normalized;
        Vector3Int[] offs = { new(0,1,0), new(0,-1,0), new(1,0,0), new(-1,0,0), new(0,0,1), new(0,0,-1) };
        int best = 0; float bestDot = -1;
        for (int i = 0; i < 6; i++) { float d = Vector3.Dot(offs[i], dir); if (d > bestDot) { bestDot = d; best = i; } }
        Vector3Int ap = new(hit.Value.x + offs[best].x, hit.Value.y + offs[best].y, hit.Value.z + offs[best].z);

        if (Mouse.current.leftButton.wasPressedThisFrame
            && _gridManager.InBounds(ap.x, ap.y, ap.z)
            && _gridManager.GetBlock(ap.x, ap.y, ap.z) == BlockType.Air)
            _gridManager.SetBlock(ap.x, ap.y, ap.z, _selectedType);

        if (Mouse.current.rightButton.wasPressedThisFrame)
            _gridManager.RemoveBlock(hit.Value.x, hit.Value.y, hit.Value.z);
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 500, 30), $"Block: {_selectedType} [1-9]  LMB/RMB  F5=Save F9=Load");
    }
}
