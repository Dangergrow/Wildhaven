using UnityEngine;

/// <summary>
/// LMB on colonist = select (green ring). LMB on empty space = deselect.
/// </summary>
public class SelectionManager : MonoBehaviour
{
    public Colonist selectedColonist;
    public GameObject ring;

    private Camera _cam;
    private ColonistSpawner _spawner;
    private BuildManager _build;
    private bool _buildMode = true;

    void Start()
    {
        _cam = Camera.main;
        _spawner = FindObjectOfType<ColonistSpawner>();
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
        // B = toggle build/select
        if (Input.GetKeyDown(KeyCode.B))
        {
            _buildMode = !_buildMode;
            if (_build != null) _build.enabled = _buildMode;
        }

        if (_buildMode) return; // building mode — don't select
        if (!Input.GetMouseButtonDown(0)) return;
        if (_cam == null || _spawner == null) return;

        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);

        // Find colonist nearest to ray
        Colonist best = null;
        float bestDist = 3f;

        foreach (Colonist c in _spawner.Colonists)
        {
            if (c == null) continue;
            Vector3 to = c.transform.position - ray.origin;
            float t = Vector3.Dot(to, ray.direction);
            if (t < 0) continue;
            Vector3 closest = ray.origin + ray.direction * t;
            float d = Vector3.Distance(c.transform.position, closest);
            if (d < bestDist) { bestDist = d; best = c; }
        }

        selectedColonist = best;
        ring.SetActive(best != null);
        if (best != null)
        {
            ring.transform.position = best.transform.position + Vector3.up * 0.9f;
            ring.transform.SetParent(best.transform);
            Debug.Log($"[Select] {best.colonistName}");
        }
        else
        {
            ring.transform.SetParent(null);
        }
    }

    void OnGUI()
    {
        if (selectedColonist == null) return;
        Colonist c = selectedColonist;
        GUI.Box(new Rect(Screen.width - 230, Screen.height / 2f - 50, 220, 85),
            $"� {c.colonistName}  Age:{c.age}\nHP:{c.health:F0}/{c.maxHealth:F0}  Mood:{c.mood:F0}\nHunger:{c.hunger:F0}  Fatigue:{c.fatigue:F0}");
        GUI.Label(new Rect(10, Screen.height - 40, 400, 30), "B = toggle build/select | LMB = select colonist");
        GUIStyle ms = new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold };
        ms.normal.textColor = _buildMode ? Color.green : Color.cyan;
        GUI.Label(new Rect(Screen.width - 130, 5, 125, 24), _buildMode ? "BUILD" : "SELECT", ms);
    }
}
