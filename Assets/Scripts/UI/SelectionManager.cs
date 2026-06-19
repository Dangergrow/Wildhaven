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
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        // Raycast from mouse to find colonist
        Ray ray = _cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out RaycastHit hit, 300f)) { Deselect(); return; }

        Colonist c = hit.collider.GetComponentInParent<Colonist>();
        if (c != null) Select(c); else Deselect();
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
}
