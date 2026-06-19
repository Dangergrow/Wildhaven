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

        // RMB — direct order: ground = move, block = mine
        if (Mouse.current.rightButton.wasPressedThisFrame && selectedColonist != null)
        {
            var ai = selectedColonist.GetComponent<ColonistAI>();
            if (ai == null) return;
            Vector3 worldPos = GetMouseWorldPosition();
            GridManager gm = FindObjectOfType<GridManager>();
            if (gm != null)
            {
                Vector3Int gp = gm.WorldToGrid(worldPos);
                if (gm.InBounds(gp.x, gp.y, gp.z) && gm.GetBlock(gp.x, gp.y, gp.z) != BlockType.Air)
                    ai.GiveOrder(ColonistAI.OrderType.Mine, worldPos);
                else
                    ai.GiveOrder(ColonistAI.OrderType.Move, worldPos);
            }
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
