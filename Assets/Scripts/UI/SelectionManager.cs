using UnityEngine;

/// <summary>
/// Ghost preview for building + colonist/enemy selection on click.
/// B = toggle build/select mode.
/// </summary>
public class SelectionManager : MonoBehaviour
{
    public bool buildMode = true;
    public GameObject ghostCube;
    public Color ghostColor = new Color(1, 1, 1, 0.3f);
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

        if (ghostCube == null)
        {
            ghostCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ghostCube.name = "BuildGhost";
            ghostCube.transform.localScale = Vector3.one * 0.95f;
            ghostCube.GetComponent<Renderer>().material.color = ghostColor;
            Destroy(ghostCube.GetComponent<Collider>());
        }

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
        HandleInput();
    }

    void UpdateGhost()
    {
        if (_grid == null || _cam == null) { ghostCube.SetActive(false); return; }

        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        var hit = _grid.RaycastGrid(ray);
        if (hit == null) { ghostCube.SetActive(false); return; }

        Vector3 hw = _grid.GridToWorld(hit.Value.x, hit.Value.y, hit.Value.z);
        Vector3 dir = (_cam.transform.position - hw).normalized;
        Vector3Int[] offs = { new(0,1,0), new(0,-1,0), new(1,0,0), new(-1,0,0), new(0,0,1), new(0,0,-1) };
        int best = 0; float bestDot = -1;
        for (int i = 0; i < 6; i++) { float d = Vector3.Dot(offs[i], dir); if (d > bestDot) { bestDot = d; best = i; } }
        Vector3Int placePos = new(hit.Value.x + offs[best].x, hit.Value.y + offs[best].y, hit.Value.z + offs[best].z);

        bool canPlace = _grid.InBounds(placePos.x, placePos.y, placePos.z)
            && _grid.GetBlock(placePos.x, placePos.y, placePos.z) == BlockType.Air
            && !BuildBlocker.IsOccupied(placePos.x, placePos.y, placePos.z);

        ghostCube.SetActive(true);
        ghostCube.transform.position = _grid.GridToWorld(placePos.x, placePos.y, placePos.z);
        ghostCube.GetComponent<Renderer>().material.color = canPlace ? ghostColor : Color.red;
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            buildMode = !buildMode;
            if (_build != null) _build.enabled = buildMode;
        }

        if (buildMode || !Input.GetMouseButtonDown(0)) return;
        if (_cam == null) return;

        // Select nearest colonist to mouse click position
        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        var gridHit = _grid.RaycastGrid(ray);
        if (gridHit == null) return;

        Vector3 clickWorld = _grid.GridToWorld(gridHit.Value.x, gridHit.Value.y, gridHit.Value.z);
        ColonistSpawner spawner = FindObjectOfType<ColonistSpawner>();
        if (spawner == null) return;

        Colonist nearest = null;
        float minDist = 2f;
        foreach (Colonist c in spawner.Colonists)
        {
            if (c == null) continue;
            float d = Vector3.Distance(clickWorld, c.transform.position);
            if (d < minDist) { minDist = d; nearest = c; }
        }

        if (nearest != null) SelectColonist(nearest);
        else DeselectAll();
    }

    void SelectColonist(Colonist c)
    {
        DeselectAll();
        selectedColonist = c;
        selectionIndicator.SetActive(true);
        selectionIndicator.transform.position = c.transform.position + Vector3.up * 0.8f;
        selectionIndicator.transform.SetParent(c.transform);
    }

    void SelectEnemy(Enemy e)
    {
        DeselectAll();
        selectionIndicator.SetActive(true);
        selectionIndicator.transform.position = e.transform.position + Vector3.up * 0.8f;
        selectionIndicator.transform.SetParent(e.transform);
        selectionIndicator.GetComponent<Renderer>().material.color = Color.red;
    }

    void DeselectAll()
    {
        selectedColonist = null;
        selectionIndicator.SetActive(false);
        selectionIndicator.transform.SetParent(null);
        selectionIndicator.GetComponent<Renderer>().material.color = Color.green;
    }

    void OnGUI()
    {
        GUIStyle ms = new GUIStyle(GUI.skin.label) { fontSize = 12, fontStyle = FontStyle.Bold };
        ms.normal.textColor = buildMode ? Color.green : Color.yellow;
        GUI.Label(new Rect(Screen.width - 110, 28, 105, 20), buildMode ? "BUILD [B]" : "SELECT [B]", ms);

        if (selectedColonist == null) return;
        Colonist c = selectedColonist;
        GUI.Box(new Rect(Screen.width - 220, Screen.height / 2f - 60, 210, 100),
            $"{c.colonistName}\nHP:{c.health:F0} Mood:{c.mood:F0}\nHunger:{c.hunger:F0} Fatigue:{c.fatigue:F0}");
    }
}
