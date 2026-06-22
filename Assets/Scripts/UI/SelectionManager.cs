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
    public GameObject ring;
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
    }

    void HandleSelect()
    {
        // Use grid raycast (same as BuildManager — reliable)
        Ray ray = _cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        var gridHit = _grid.RaycastGrid(ray);
        if (gridHit == null) { Deselect(); return; }

        Vector3 clickWorld = _grid.GridToWorld(gridHit.Value.x, gridHit.Value.y, gridHit.Value.z);

        // Find nearest colonist within 2.5 blocks
        Colonist best = null;
        float bestDist = 2.5f;
        foreach (Colonist c in _spawner.Colonists)
        {
            if (c == null || c.currentState == ColonistState.Dead) continue;
            float d = Vector3.Distance(clickWorld, c.transform.position);
            if (d < bestDist) { bestDist = d; best = c; }
        }

        selectedColonist = best;
        ring.SetActive(best != null);
        if (best != null)
        {
            ring.transform.position = best.transform.position + Vector3.up * 0.9f;
            ring.transform.SetParent(best.transform);
        }
        else ring.transform.SetParent(null);
    }

    void Select(Colonist c)
    {
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
        GUIStyle s = new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold };
        s.normal.textColor = _buildMode ? Color.green : Color.cyan;
        GUI.Label(new Rect(Screen.width - 130, 5, 125, 24), _buildMode ? "BUILD [B]" : "SELECT [B]", s);

        if (selectedColonist == null) return;
        Colonist c = selectedColonist;
        GUI.Box(new Rect(Screen.width - 230, Screen.height / 2f - 45, 220, 80),
            $"{c.colonistName}  Age:{c.age}\nHP:{c.health:F0}/{c.maxHealth:F0}  Mood:{c.mood:F0}\nHunger:{c.hunger:F0}  Fatigue:{c.fatigue:F0}");
    }
}
