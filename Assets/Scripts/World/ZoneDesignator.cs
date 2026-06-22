using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Simple zone placement: F3 = Zone mode, LMB = place zone marker.
/// Zones are visual markers only for now — they define areas for colonists.
/// </summary>
public class ZoneDesignator : MonoBehaviour
{
    public ZoneMarker.ZoneType currentZoneType = ZoneMarker.ZoneType.Stockpile;
    private GridManager _grid;
    private Camera _cam;
    private bool _active;

    void Start()
    {
        _grid = FindObjectOfType<GridManager>();
        _cam = Camera.main ?? FindObjectOfType<Camera>();
    }

    void Update()
    {
        if (Keyboard.current == null) return;
        if (Keyboard.current.f3Key.wasPressedThisFrame) _active = !_active;
        if (!_active) return;

        // Number keys switch zone type in Zone mode
        if (Keyboard.current.digit1Key.wasPressedThisFrame) currentZoneType = ZoneMarker.ZoneType.Stockpile;
        if (Keyboard.current.digit2Key.wasPressedThisFrame) currentZoneType = ZoneMarker.ZoneType.Dump;
        if (Keyboard.current.digit3Key.wasPressedThisFrame) currentZoneType = ZoneMarker.ZoneType.Farm;
        if (Keyboard.current.digit4Key.wasPressedThisFrame) currentZoneType = ZoneMarker.ZoneType.Room;

        if (Mouse.current.leftButton.wasPressedThisFrame && _cam != null)
        {
            Ray ray = _cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            var hit = _grid.RaycastGrid(ray);
            if (hit != null)
            {
                PlaceZone(hit.Value);
            }
        }
    }

    void PlaceZone(Vector3Int pos)
    {
        // Create a visible zone marker GameObject
        GameObject zoneObj = GameObject.CreatePrimitive(PrimitiveType.Quad);
        zoneObj.name = $"Zone_{currentZoneType}_{pos.x}_{pos.z}";
        zoneObj.transform.position = _grid.GridToWorld(pos.x, pos.y, pos.z) + Vector3.up * 0.05f;
        zoneObj.transform.rotation = Quaternion.Euler(90, 0, 0);
        zoneObj.transform.localScale = Vector3.one * 0.9f;

        // Color by zone type
        Renderer r = zoneObj.GetComponent<Renderer>();
        r.material = new Material(r.material);
        r.material.color = currentZoneType switch
        {
            ZoneMarker.ZoneType.Stockpile => new Color(0.3f, 0.2f, 0.1f, 0.4f),
            ZoneMarker.ZoneType.Dump => new Color(0.4f, 0.1f, 0.1f, 0.4f),
            ZoneMarker.ZoneType.Farm => new Color(0.2f, 0.5f, 0.1f, 0.4f),
            ZoneMarker.ZoneType.Room => new Color(0.3f, 0.3f, 0.4f, 0.4f),
            _ => new Color(0.5f, 0.5f, 0.5f, 0.4f)
        };

        Destroy(zoneObj.GetComponent<Collider>());

        // Register with ZoneMarker system
        var zm = zoneObj.AddComponent<ZoneMarker>();
        zm.gridPos = pos;
        zm.zoneType = currentZoneType;
        zm.width = 1;
        zm.height = 1;

        Debug.Log($"[Zone] Placed {currentZoneType} at {pos}");
    }

    void OnGUI()
    {
        if (!_active) return;
        GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height - 60, 200, 55), "ZONE MODE [F3]");
        GUI.Label(new Rect(Screen.width / 2 - 90, Screen.height - 52, 180, 20), $"Type: {currentZoneType}");
        GUI.Label(new Rect(Screen.width / 2 - 90, Screen.height - 35, 180, 20), "1=Stock 2=Dump 3=Farm 4=Room");
        GUI.Label(new Rect(Screen.width / 2 - 90, Screen.height - 20, 180, 15), "LMB = place zone");
    }
}
