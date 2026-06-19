using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// B = toggle build/select mode.
/// In select mode: LMB = select colonist under cursor via Physics.Raycast.
/// </summary>
public class SelectionManager : MonoBehaviour
{
    public Colonist selectedColonist;
    public GameObject ring;
    private BuildManager _build;
    private Camera _cam;
    private bool _buildMode = true;
    private bool _showContextMenu;
    private bool _contextMenuDrawn;
    private Vector2 _contextMenuPos;
    private Vector3 _contextWorldPos;
    private bool _contextOnBlock;
    private int _pendingOrder; // 0=none, 1=move, 2=mine

    void Start()
    {
        _build = FindObjectOfType<BuildManager>();
        _cam = Camera.main;
        if (_cam == null) _cam = FindObjectOfType<Camera>();
        if (ring == null)
        {
            ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ring.name = "Ring";
            ring.transform.localScale = new Vector3(1.5f, 0.05f, 1.5f);
            ring.GetComponent<Renderer>().material.color = Color.green;
            Destroy(ring.GetComponent<Collider>());
            ring.SetActive(false);
        }
    }

    void Update()
    {
        if (Keyboard.current == null || Mouse.current == null) return;

        // Toggle mode
        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            _buildMode = !_buildMode;
            if (_build != null) _build.enabled = _buildMode;
            if (!_buildMode) Deselect();
        }

        if (_buildMode) return;
        if (_cam == null) return;

        // LMB — select colonist (skip if context menu is open)
        if (Mouse.current.leftButton.wasPressedThisFrame && !_showContextMenu)
        {
            Ray ray = _cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out RaycastHit hit, 300f)) { Deselect(); return; }
            Colonist c = hit.collider.GetComponentInParent<Colonist>();
            if (c != null) Select(c); else Deselect();
        }

        // RMB — order selected colonist
        if (Mouse.current.rightButton.wasPressedThisFrame && selectedColonist != null)
        {
            var ai = selectedColonist.GetComponent<ColonistAI>();
            if (ai == null) return;
            _showContextMenu = true;
            _contextMenuPos = Mouse.current.position.ReadValue();
            GridManager gm = FindObjectOfType<GridManager>();
            if (gm != null)
            {
                _contextWorldPos = GetMouseWorldPosition();
                Vector3Int gp = gm.WorldToGrid(_contextWorldPos);
                _contextOnBlock = gm.InBounds(gp.x, gp.y, gp.z) && gm.GetBlock(gp.x, gp.y, gp.z) != BlockType.Air;
            }
        }

        // Execute pending order from context menu (once per click, from Update)
        if (_pendingOrder != 0 && selectedColonist != null)
        {
            var ai = selectedColonist.GetComponent<ColonistAI>();
            if (ai != null)
            {
                if (_pendingOrder == 1) ai.GiveOrder(ColonistAI.OrderType.Move, _contextWorldPos);
                if (_pendingOrder == 2) ai.GiveOrder(ColonistAI.OrderType.Mine, _contextWorldPos);
            }
            _pendingOrder = 0;
        }
    }

    void Select(Colonist c)
    {
        if (selectedColonist == c) return;
        selectedColonist = c;
        ring.SetActive(true);
        ring.transform.position = c.transform.position + Vector3.up * 0.9f;
        ring.transform.SetParent(c.transform);
    }

    void Deselect()
    {
        selectedColonist = null;
        ring.SetActive(false);
        ring.transform.SetParent(null);
    }

    void OnGUI()
    {
        var s = new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold };
        s.normal.textColor = _buildMode ? Color.green : Color.cyan;
        GUI.Label(new Rect(Screen.width - 130, 5, 125, 24), _buildMode ? "BUILD [B]" : "SELECT [B]", s);

        if (selectedColonist != null)
        {
            Colonist c = selectedColonist;
            GUI.Box(new Rect(Screen.width - 230, Screen.height / 2 - 45, 220, 80),
                $"{c.colonistName}  Age:{c.age}\nHP:{c.health:F0}/{c.maxHealth:F0}  Mood:{c.mood:F0}\nHunger:{c.hunger:F0}  Fatigue:{c.fatigue:F0}");
        }

        // Context menu
        if (_showContextMenu && selectedColonist != null)
        {
            var ai = selectedColonist.GetComponent<ColonistAI>();
            if (ai == null) return;
            float w = 140, h = _contextOnBlock ? 70 : 40;
            Rect r = new Rect(_contextMenuPos.x, Screen.height - _contextMenuPos.y, w, h);
            GUI.Box(r, "");
            if (GUI.Button(new Rect(r.x + 5, r.y + 5, w - 10, 22), "Move Here"))
            { _pendingOrder = 1; _showContextMenu = false; }
            if (_contextOnBlock && GUI.Button(new Rect(r.x + 5, r.y + 30, w - 10, 22), "Mine"))
            { _pendingOrder = 2; _showContextMenu = false; }
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        GridManager gm = FindObjectOfType<GridManager>();
        Ray ray = _cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        var hit = gm != null ? gm.RaycastGrid(ray) : null;
        if (hit != null) return gm.GridToWorld(hit.Value.x, hit.Value.y, hit.Value.z);
        Plane p = new Plane(Vector3.up, Vector3.zero);
        if (p.Raycast(ray, out float d)) return ray.GetPoint(d);
        return ray.origin + ray.direction * 50f;
    }
}
