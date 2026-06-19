using UnityEngine;

/// <summary>
/// B key toggles between BUILD and SELECT mode.
/// SELECT mode: LMB selects nearest colonist to mouse ray.
/// Ghost preview shows where block will be placed.
/// </summary>
public class SelectionManager : MonoBehaviour
{
    public bool buildMode = true;
    public Colonist selectedColonist;

    private BuildManager _build;
    private Camera _cam;
    private ColonistSpawner _spawner;

    void Start()
    {
        _build = FindObjectOfType<BuildManager>();
        _cam = Camera.main;
        _spawner = FindObjectOfType<ColonistSpawner>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            buildMode = !buildMode;
            if (_build != null) _build.enabled = buildMode;
        }

        if (buildMode) return;
        if (!Input.GetMouseButtonDown(0)) return;
        if (_cam == null || _spawner == null) return;

        // Simple selection: find colonist nearest to mouse ray
        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        Colonist best = null;
        float bestDist = 4f;

        for (int i = 0; i < _spawner.Colonists.Count; i++)
        {
            Colonist c = _spawner.Colonists[i];
            if (c == null || c.currentState == ColonistState.Dead) continue;

            // Distance from ray to colonist position
            Vector3 toColonist = c.transform.position - ray.origin;
            float alongRay = Vector3.Dot(toColonist, ray.direction);
            if (alongRay < 0) continue; // behind camera

            Vector3 closestPoint = ray.origin + ray.direction * alongRay;
            float dist = Vector3.Distance(c.transform.position, closestPoint);

            if (dist < bestDist)
            {
                bestDist = dist;
                best = c;
            }
        }

        selectedColonist = best;
        if (selectedColonist != null)
            Debug.Log($"[Select] {selectedColonist.colonistName} selected (dist={bestDist:F1})");
    }

    void OnGUI()
    {
        GUIStyle ms = new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold };
        ms.normal.textColor = buildMode ? Color.green : Color.cyan;
        GUI.Label(new Rect(Screen.width - 130, 28, 125, 24), buildMode ? "BUILD [B]" : "SELECT [B]", ms);

        if (selectedColonist == null) return;
        Colonist c = selectedColonist;
        GUI.Box(new Rect(Screen.width - 230, Screen.height / 2f - 50, 220, 85),
            $"{c.colonistName}  Age:{c.age}\nHP:{c.health:F0}/{c.maxHealth:F0}  Mood:{c.mood:F0}\nHunger:{c.hunger:F0}  Fatigue:{c.fatigue:F0}");
    }
}
