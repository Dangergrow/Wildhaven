using UnityEngine;

/// <summary>
/// LMB selects colonist under cursor via Physics.Raycast.
/// B toggles build/select mode.
/// </summary>
public class SelectionManager : MonoBehaviour
{
    public Colonist selectedColonist;
    public GameObject ring;
    private BuildManager _build;
    private bool _buildMode = true;

    void Start()
    {
        _build = FindObjectOfType<BuildManager>();
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
        if (Input.GetKeyDown(KeyCode.B))
        {
            _buildMode = !_buildMode;
            if (_build != null) _build.enabled = _buildMode;
        }
        if (_buildMode) return;
        if (!Input.GetMouseButtonDown(0)) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 200f)) { Deselect(); return; }

        Colonist c = hit.collider.GetComponentInParent<Colonist>();
        if (c != null) Select(c); else Deselect();
    }

    void Select(Colonist c)
    {
        selectedColonist = c;
        ring.SetActive(true);
        ring.transform.position = c.transform.position + Vector3.up * 0.9f;
        ring.transform.SetParent(c.transform);
        Debug.Log($"[Select] {c.colonistName}");
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
        GUI.Label(new Rect(10, Screen.height - 40, 400, 30), "B = build/select  |  LMB = select colonist", s);

        if (selectedColonist == null) return;
        Colonist c = selectedColonist;
        GUI.Box(new Rect(Screen.width - 230, Screen.height / 2f - 45, 220, 80),
            $"{c.colonistName}  Age:{c.age}\nHP:{c.health:F0}/{c.maxHealth:F0}  Mood:{c.mood:F0}\nHunger:{c.hunger:F0}  Fatigue:{c.fatigue:F0}");
    }
}
