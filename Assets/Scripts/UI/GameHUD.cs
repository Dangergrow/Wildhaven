using UnityEngine;

/// <summary>
/// Simple HUD overlay: time, colonist stats, build panel.
/// </summary>
public class GameHUD : MonoBehaviour
{
    private DayCycle _day;
    private ColonistSpawner _sp;
    private GUIStyle _label, _small, _bold;

    void Start()
    {
        _day = FindObjectOfType<DayCycle>();
        _sp = FindObjectOfType<ColonistSpawner>();
    }

    void OnGUI()
    {
        if (_day == null || _sp == null) return;

        if (_label == null)
        {
            _label = new GUIStyle(GUI.skin.label) { fontSize = 13, normal = { textColor = Color.white } };
            _small = new GUIStyle(GUI.skin.label) { fontSize = 10, normal = { textColor = Color.gray } };
            _bold = new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold, normal = { textColor = Color.white } };
        }

        DrawTimeBar();
        DrawColonistPanel();
        DrawBuildBar();
        DrawBlockIndicator();
    }

    void DrawTimeBar()
    {
        Rect r = new Rect(Screen.width / 2 - 90, 4, 180, 34);
        GUI.Box(r, "");
        string spd = _day.IsPaused ? "PAUSED" : $"{_day.gameSpeed}x";
        GUI.Label(new Rect(r.x + 4, r.y + 2, r.width - 8, 18), $"Day {_day.day}  {_day.hour:D2}:{_day.minute:D2}  [{spd}]", _bold);
        string season = $"Season: {_day.season} | Space=pause Num1-3=speed";
        GUI.Label(new Rect(r.x + 4, r.y + 20, r.width - 8, 12), season, _small);
    }

    void DrawColonistPanel()
    {
        if (_sp.Colonists.Count == 0) return;
        int count = _sp.Colonists.Count;
        int panelH = 20 + count * 70;
        Rect panel = new Rect(6, 45, 200, panelH);
        GUI.Box(panel, "COLONISTS");

        int y = panel.y + 20;
        foreach (Colonist c in _sp.Colonists)
        {
            if (c == null) continue;
            GUI.Label(new Rect(panel.x + 5, y, 190, 15), $"{c.colonistName} ({c.age})", _bold); y += 16;
            GUI.Label(new Rect(panel.x + 8, y, 184, 13), $"HP:{c.health:F0}  Hung:{c.hunger:F0}  Fat:{c.fatigue:F0}", _small); y += 14;
            DrawBarG(new Rect(panel.x + 5, y, 190, 8), c.health / c.maxHealth, Color.red); y += 10;
            DrawBarG(new Rect(panel.x + 5, y, 190, 8), c.mood / 100f, new Color(0.2f, 0.7f, 0.2f)); y += 12;
            GUI.Label(new Rect(panel.x + 8, y, 184, 12), $"Skills B:{c.constructionSkill} M:{c.miningSkill} C:{c.cookingSkill}", _small); y += 12;
            GUI.Label(new Rect(panel.x + 8, y, 184, 12), $"       R:{c.rangedSkill} F:{c.farmingSkill} S:{c.socialSkill}", _small); y += 16;
        }
    }

    void DrawBuildBar()
    {
        Rect r = new Rect(4, Screen.height - 28, Screen.width - 8, 24);
        GUI.Box(r, "");
        GUI.Label(new Rect(r.x + 4, r.y + 4, r.width - 8, 16), "Build: 1=Dirt 2=Grass 3=Stone 4=Wood 5=Planks 6=Brick 7=Glass 8=Snow 9=Sand | LMB=Place RMB=Destroy | F5=Save F9=Load", _small);
    }

    void DrawBlockIndicator()
    {
        BuildManager bm = FindObjectOfType<BuildManager>();
        if (bm == null) return;
        Rect r = new Rect(Screen.width - 100, 4, 95, 22);
        GUI.Box(r, "");
        GUI.Label(new Rect(r.x + 4, r.y + 3, 87, 16), $"Block: {bm.SelectedType}", _small);
    }

    void DrawBarG(Rect r, float fill, Color c)
    {
        GUI.color = new Color(0.15f, 0.15f, 0.15f);
        GUI.Box(r, "");
        GUI.color = c;
        GUI.Box(new Rect(r.x + 1, r.y + 1, (r.width - 2) * Mathf.Clamp01(fill), r.height - 2), "");
        GUI.color = Color.white;
    }
}
