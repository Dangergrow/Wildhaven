using UnityEngine;

/// <summary>
/// Shows a ghost preview of where a block will be placed.
/// Also handles colonist selection on click.
/// </summary>
public class SelectionManager : MonoBehaviour
{
    [Header("Ghost")]
    public GameObject ghostCube;
    public Color ghostColor = new Color(1, 1, 1, 0.3f);

    [Header("Selection")]
    public Colonist selectedColonist;
    public GameObject selectionIndicator;

    private GridManager _grid;
    private BuildManager _build;
    private Camera _cam;

    void Start()
    {
        _grid = FindObjectOfType<GridManager>();
        _build = FindObjectOfType<BuildManager>();
        _cam = Camera.main;

        // Create ghost cube
        if (ghostCube == null)
        {
            ghostCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ghostCube.name = "BuildGhost";
            ghostCube.transform.localScale = Vector3.one * 0.95f;
            Renderer r = ghostCube.GetComponent<Renderer>();
            r.material = new Material(r.material);
            r.material.color = ghostColor;
            Destroy(ghostCube.GetComponent<Collider>());
        }

        // Selection indicator
        if (selectionIndicator == null)
        {
            selectionIndicator = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            selectionIndicator.name = "SelectionRing";
            selectionIndicator.transform.localScale = new Vector3(1.2f, 0.1f, 1.2f);
            selectionIndicator.GetComponent<Renderer>().material.color = Color.green;
            Destroy(selectionIndicator.GetComponent<Collider>());
            selectionIndicator.SetActive(false);
        }
    }

    void Update()
    {
        UpdateGhost();
        HandleClick();
    }

    void UpdateGhost()
    {
        if (_grid == null || _cam == null) { ghostCube.SetActive(false); return; }

        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        var hit = _grid.RaycastGrid(ray);
        if (hit == null) { ghostCube.SetActive(false); return; }

        // Find placement position (same logic as BuildManager)
        Vector3 hw = _grid.GridToWorld(hit.Value.x, hit.Value.y, hit.Value.z);
        Vector3 dir = (_cam.transform.position - hw).normalized;
        Vector3Int[] offs = { new(0,1,0), new(0,-1,0), new(1,0,0), new(-1,0,0), new(0,0,1), new(0,0,-1) };
        int best = 0; float bestDot = -1;
        for (int i = 0; i < 6; i++) { float d = Vector3.Dot(offs[i], dir); if (d > bestDot) { bestDot = d; best = i; } }
        Vector3Int placePos = new(hit.Value.x + offs[best].x, hit.Value.y + offs[best].y, hit.Value.z + offs[best].z);

        if (_grid.InBounds(placePos.x, placePos.y, placePos.z) && _grid.GetBlock(placePos.x, placePos.y, placePos.z) == BlockType.Air)
        {
            ghostCube.SetActive(true);
            ghostCube.transform.position = _grid.GridToWorld(placePos.x, placePos.y, placePos.z);
            ghostCube.GetComponent<Renderer>().material.color = ghostColor;
        }
        else
        {
            ghostCube.SetActive(false);
        }
    }

    [Header("Build Mode")]
    public bool buildMode = true; // B to toggle

    void HandleClick()
    {
        // Toggle build mode
        if (Input.GetKeyDown(KeyCode.B)) { buildMode = !buildMode; Debug.Log($"[Selection] Build mode: {buildMode}"); }
        if (buildMode) return; // let BuildManager handle clicks in build mode

        if (!Input.GetMouseButtonDown(0)) return;
        if (_cam == null) return;

        try
        {
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit, 100f)) return;
            if (hit.collider == null || hit.collider.gameObject == null) return;

            Colonist colonist = hit.collider.GetComponentInParent<Colonist>();
            if (colonist != null) { SelectColonist(colonist); return; }

            Enemy enemy = hit.collider.GetComponentInParent<Enemy>();
            if (enemy != null) { SelectEnemy(enemy); return; }

            DeselectAll();
        }
        catch (System.Exception) { /* ignore raycast errors */ }
    }

    void SelectColonist(Colonist c)
    {
        DeselectAll();
        selectedColonist = c;
        selectionIndicator.SetActive(true);
        selectionIndicator.transform.position = c.transform.position + Vector3.up * 0.8f;
        selectionIndicator.transform.SetParent(c.transform);
        Debug.Log($"[Selection] Selected {c.colonistName}");
    }

    void SelectEnemy(Enemy e)
    {
        DeselectAll();
        selectionIndicator.SetActive(true);
        selectionIndicator.transform.position = e.transform.position + Vector3.up * 0.8f;
        selectionIndicator.transform.SetParent(e.transform);
        selectionIndicator.GetComponent<Renderer>().material.color = Color.red;
        Debug.Log($"[Selection] Target: {e.enemyName}");
    }

    void DeselectAll()
    {
        selectedColonist = null;
        selectionIndicator.SetActive(false);
        selectionIndicator.transform.SetParent(null);
        selectionIndicator.GetComponent<Renderer>().material.color = Color.green;
    }

    /// <summary>Draws basic info for selected unit.</summary>
    void OnGUI()
    {
        // Build mode indicator
        GUIStyle modeStyle = new GUIStyle(GUI.skin.label) { fontSize = 12, fontStyle = FontStyle.Bold };
        modeStyle.normal.textColor = buildMode ? Color.green : Color.yellow;
        GUI.Label(new Rect(Screen.width - 110, 28, 105, 20), buildMode ? "BUILD [B]" : "SELECT [B]", modeStyle);

        if (selectedColonist == null) return;
        Colonist c = selectedColonist;
        GUIStyle s = new GUIStyle(GUI.skin.box) { fontSize = 12, alignment = TextAnchor.UpperLeft };
        string info = $"{c.colonistName} ({c.age})\nHP: {c.health:F0}/{c.maxHealth:F0}  Mood: {c.mood:F0}\nHunger: {c.hunger:F0}  Fatigue: {c.fatigue:F0}\nSkills: Con{c.constructionSkill} Min{c.miningSkill} Cok{c.cookingSkill} Med{c.medicineSkill}";
        GUI.Box(new Rect(Screen.width - 220, Screen.height / 2f - 60, 210, 100), info, s);
    }
}
