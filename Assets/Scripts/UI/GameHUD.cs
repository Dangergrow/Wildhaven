using UnityEngine;

/// <summary>
/// Simple HUD overlay showing game time, colonist stats, and colony resources.
/// Uses OnGUI for simplicity — will be replaced with Canvas UI later.
/// </summary>
public class GameHUD : MonoBehaviour
{
    private DayCycle _dayCycle;
    private ColonistSpawner _spawner;

    void Start()
    {
        _dayCycle = FindObjectOfType<DayCycle>();
        _spawner = FindObjectOfType<ColonistSpawner>();
    }

    void OnGUI()
    {
        if (_dayCycle == null || _spawner == null) return;

        // Time bar — top center
        GUI.Box(new Rect(Screen.width / 2f - 100, 5, 200, 40), "");
        GUIStyle timeStyle = new GUIStyle(GUI.skin.label) { fontSize = 16, alignment = TextAnchor.MiddleCenter };
        string speedStr = _dayCycle.IsPaused ? "PAUSED" : $"{_dayCycle.gameSpeed}x";
        GUI.Label(new Rect(Screen.width / 2f - 95, 8, 190, 20),
            $"Day {_dayCycle.day}  {_dayCycle.hour:D2}:{_dayCycle.minute:D2}  [{speedStr}]", timeStyle);
        GUI.Label(new Rect(Screen.width / 2f - 95, 28, 190, 14),
            $"Season: {_dayCycle.season} | Space=pause  Num1-3=speed", new GUIStyle(GUI.skin.label) { fontSize = 10, alignment = TextAnchor.MiddleCenter });

        // Colonist list — left side
        if (_spawner.Colonists.Count == 0) return;

        int y = 55;
        GUI.Box(new Rect(5, y, 220, 20 + _spawner.Colonists.Count * 75), "Colonists");

        foreach (Colonist c in _spawner.Colonists)
        {
            if (c == null) continue;
            y += 22;
            GUI.Label(new Rect(10, y, 210, 16), $"{c.colonistName} ({c.age})");
            y += 16;
            GUI.Label(new Rect(15, y, 200, 14), $"HP: {c.health:F0}  Hunger: {c.hunger:F0}  Fatigue: {c.fatigue:F0}");
            y += 14;

            // Health bar
            GUI.Label(new Rect(15, y, 50, 10), "HP");
            DrawBar(new Rect(65, y, 140, 10), c.health / c.maxHealth, Color.red);
            y += 14;

            // Mood bar
            GUI.Label(new Rect(15, y, 50, 10), "Mood");
            DrawBar(new Rect(65, y, 140, 10), c.mood / 100f, Color.Lerp(Color.red, Color.green, c.mood / 100f));
            y += 16;

            GUI.Label(new Rect(15, y, 200, 12), $"Skills: B:{c.constructionSkill} M:{c.miningSkill} C:{c.cookingSkill} R:{c.rangedSkill} F:{c.farmingSkill}");
            y += 14;
        }

        // Bottom bar — build controls
        GUI.Box(new Rect(5, Screen.height - 35, Screen.width - 10, 30), "");
        GUI.Label(new Rect(10, Screen.height - 30, Screen.width - 20, 20),
            "Build: 1=Dirt 2=Grass 3=Stone 4=Wood 5=Planks 6=Brick 7=Glass 8=Snow 9=Sand | LMB=Place  RMB=Destroy | F5=Save  F9=Load",
            new GUIStyle(GUI.skin.label) { fontSize = 11, alignment = TextAnchor.MiddleCenter });

        // Top-right — selected block
        BuildManager bm = FindObjectOfType<BuildManager>();
        if (bm != null)
        {
            GUI.Box(new Rect(Screen.width - 110, 5, 105, 25), "");
            GUI.Label(new Rect(Screen.width - 105, 8, 95, 20), $"Block: {bm.SelectedType}",
                new GUIStyle(GUI.skin.label) { fontSize = 12 });
        }
    }

    void DrawBar(Rect rect, float fill, Color color)
    {
        GUI.color = Color.gray;
        GUI.Box(rect, "");
        GUI.color = color;
        GUI.Box(new Rect(rect.x + 1, rect.y + 1, (rect.width - 2) * Mathf.Clamp01(fill), rect.height - 2), "");
        GUI.color = Color.white;
    }
}
