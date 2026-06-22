using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

/// <summary>
/// B = toggle build (green) / select (cyan) mode.
/// Select mode: LMB selects nearest colonist to cursor.
/// </summary>
public class SelectionManager : MonoBehaviour
{
    public Colonist selectedColonist;
    private BuildManager _build;
    private ColonistSpawner _spawner;
    private Camera _cam;
    private GridManager _grid;
    private bool _buildMode = true;

    void Start()
    {
        _build = FindObjectOfType<BuildManager>();
        _spawner = FindObjectOfType<ColonistSpawner>();
        _grid = FindObjectOfType<GridManager>();
        _cam = Camera.main ?? FindObjectOfType<Camera>();
    }

    void Update()
    {
        if (Keyboard.current == null || Mouse.current == null) return;

        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            _buildMode = !_buildMode;
            if (_build != null) _build.enabled = _buildMode;
            if (!_buildMode) Deselect();
        }

        if (_buildMode) return;
        if (_cam == null || _grid == null || _spawner == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
            HandleSelect();
        if (Mouse.current.rightButton.wasPressedThisFrame)
            HandleColonistRMB();
    }

    void HandleSelect()
    {
        Ray ray = _cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        var gridHit = _grid.RaycastGrid(ray);
        if (gridHit == null) { Deselect(); return; }

        Vector3 clickWorld = _grid.GridToWorld(gridHit.Value.x, gridHit.Value.y, gridHit.Value.z);
        Colonist best = null; float bestDist = 2.5f;
        foreach (Colonist c in _spawner.Colonists)
        {
            if (c == null || c.currentState == ColonistState.Dead) continue;
            float d = Vector3.Distance(clickWorld, c.transform.position);
            if (d < bestDist) { bestDist = d; best = c; }
        }
        selectedColonist = best;
    }

    void HandleColonistRMB()
    {
        if (selectedColonist == null) return;
        // Only show menu if RMB is near the selected colonist
        Ray ray = _cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        var gridHit = _grid.RaycastGrid(ray);
        if (gridHit == null) return;
        Vector3 clickWorld = _grid.GridToWorld(gridHit.Value.x, gridHit.Value.y, gridHit.Value.z);
        float dist = Vector3.Distance(clickWorld, selectedColonist.transform.position);
        if (dist > 2f) { Deselect(); return; }
        Vector2 mousePos = Mouse.current.position.ReadValue();
        float menuH = (selectedColonist.currentState == ColonistState.Incapacitated) ? 100 : 130;
        _showMenu = true;
        _menuRect = new Rect(mousePos.x, Screen.height - mousePos.y, 180, menuH);
        _menuTarget = selectedColonist.gameObject;
        _menuColonist = selectedColonist;
    }

    private bool _showMenu;
    private Rect _menuRect;
    private GameObject _menuTarget;
    private Colonist _menuColonist;

    void CloseMenu()
    {
        _showMenu = false; _menuTarget = null; _menuColonist = null;
    }

    void Select(Colonist c)
    {
        selectedColonist = c;
    }

    void Deselect()
    {
        selectedColonist = null;
    }

    void OnGUI()
    {
        GUIStyle s = new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold };
        s.normal.textColor = _buildMode ? Color.green : Color.cyan;
        GUI.Label(new Rect(Screen.width - 130, 5, 125, 24), _buildMode ? "BUILD [B]" : "SELECT [B]", s);

        // Context menu
        if (_showMenu && _menuColonist != null)
        {
            Colonist c = _menuColonist;
            GUIStyle btn = new GUIStyle(GUI.skin.button) { fontSize = 13, alignment = TextAnchor.MiddleLeft };
            btn.margin = new RectOffset(0, 0, 0, 0);
            Rect r = _menuRect;
            GUI.Box(new Rect(r.x, r.y, r.width, 24), c.colonistName, GUI.skin.box);
            r.y += 24; float h = 22;

            if (c.currentState != ColonistState.Incapacitated)
            {
                if (GUI.Button(new Rect(r.x, r.y, r.width, h), "Move here", btn))
                { OrderMove(); CloseMenu(); }
                r.y += h;
                if (GUI.Button(new Rect(r.x, r.y, r.width, h), "Attack nearest", btn))
                { OrderAttack(); CloseMenu(); }
                r.y += h;
                if (GUI.Button(new Rect(r.x, r.y, r.width, h), "Pick up", btn))
                { OrderPickup(); CloseMenu(); }
                r.y += h;
                if (GUI.Button(new Rect(r.x, r.y, r.width, h), "Prioritize work", btn))
                { OrderPrioritize(); CloseMenu(); }
                r.y += h;
            }
            if (GUI.Button(new Rect(r.x, r.y, r.width, h), "Heal", btn))
            { OrderHeal(); CloseMenu(); }
            r.y += h;
            if (GUI.Button(new Rect(r.x, r.y, r.width, h), "Deselect", btn))
            { Deselect(); CloseMenu(); }
        }

        if (selectedColonist == null || _showMenu) return;
        Colonist ci = selectedColonist;
        if (ci == null) return;
        // 2D ring indicator at colonist screen position (no terrain occlusion)
        Vector3 sp = _cam.WorldToScreenPoint(ci.transform.position + Vector3.up * 0.9f);
        if (sp.z > 0)
        {
            float ringR = 12f;
            GUI.color = Color.green;
            GUI.Label(new Rect(sp.x - ringR, Screen.height - sp.y - ringR, ringR * 2, ringR * 2),
                "O", new GUIStyle(GUI.skin.label) { fontSize = 28, alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
            GUI.color = Color.white;
        }
        GUI.Box(new Rect(Screen.width - 230, Screen.height / 2f - 45, 220, 80),
            $"{ci.colonistName}  Age:{ci.age}\nHP:{ci.health:F0}/{ci.maxHealth:F0}  Mood:{ci.mood:F0}\nHunger:{ci.hunger:F0}  Fatigue:{ci.fatigue:F0}");
    }

    void OrderMove()
    {
        ColonistAI ai = _menuColonist != null ? _menuColonist.GetComponent<ColonistAI>() : null;
        if (ai != null) ai.GiveOrder(ColonistAI.OrderType.Move, GetCursorWorldPos());
    }

    void OrderAttack()
    {
        ColonistAI ai = _menuColonist != null ? _menuColonist.GetComponent<ColonistAI>() : null;
        Enemy nearest = FindClosestEnemy();
        if (nearest != null && ai != null)
        {
            ai.GiveOrder(ColonistAI.OrderType.Attack, nearest.transform.position);
            ai.orderEnemy = nearest;
        }
    }

    void OrderPickup()
    {
        ColonistAI ai = _menuColonist != null ? _menuColonist.GetComponent<ColonistAI>() : null;
        if (ai != null) ai.GiveOrder(ColonistAI.OrderType.Haul, GetCursorWorldPos());
    }

    void OrderPrioritize()
    {
        ColonistAI ai = _menuColonist != null ? _menuColonist.GetComponent<ColonistAI>() : null;
        if (ai != null) ai.GiveOrder(ColonistAI.OrderType.Prioritize, GetCursorWorldPos());
    }

    void OrderHeal()
    {
        if (_menuColonist == null) return;
        _menuColonist.health = Mathf.Min(_menuColonist.health + 20, _menuColonist.maxHealth);
    }

    Vector3 GetCursorWorldPos()
    {
        Ray ray = _cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        var hit = _grid.RaycastGrid(ray);
        if (hit != null) return _grid.GridToWorld(hit.Value.x, hit.Value.y, hit.Value.z) + Vector3.up * 0.5f;
        return _cam.transform.position + _cam.transform.forward * 10f;
    }

    Enemy FindClosestEnemy()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        Enemy best = null; float bestDist = 15f;
        if (_menuColonist == null) return null;
        Vector3 cp = _menuColonist.transform.position;
        foreach (Enemy e in enemies)
        {
            float d = Vector3.Distance(cp, e.transform.position);
            if (d < bestDist) { bestDist = d; best = e; }
        }
        return best;
    }

    /// <summary>LMB click outside menu closes the menu.</summary>
    void LateUpdate()
    {
        if (_showMenu && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            CloseMenu();
    }
}
