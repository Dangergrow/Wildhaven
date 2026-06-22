using UnityEngine;

/// <summary>
/// Esc = toggle pause menu overlay. Continue, Save/Load, Settings, Main Menu, Quit.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    private bool _paused;
    private DayCycle _day;
    private Rect _windowRect = new Rect(Screen.width / 2f - 120, Screen.height / 2f - 100, 240, 200);

    void Start()
    {
        _day = FindFirstObjectByType<DayCycle>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _paused = !_paused;
            if (_day != null) _day.gameSpeed = _paused ? 0f : 1f;
        }
    }

    void OnGUI()
    {
        if (!_paused) return;
        _windowRect = GUI.Window(0, _windowRect, DrawPauseWindow, "PAUSE");
    }

    void DrawPauseWindow(int id)
    {
        GUILayout.Space(10);
        if (GUILayout.Button("Continue", GUILayout.Height(30))) { _paused = false; if (_day != null) _day.gameSpeed = 1f; }
        if (GUILayout.Button("Save Game", GUILayout.Height(30)))
        {
            GameSaveManager sv = FindFirstObjectByType<GameSaveManager>();
            if (sv != null) sv.SaveGame();
        }
        if (GUILayout.Button("Load Game", GUILayout.Height(30)))
        {
            GameSaveManager sv = FindFirstObjectByType<GameSaveManager>();
            if (sv != null) sv.LoadGame();
        }
        if (GUILayout.Button("Main Menu", GUILayout.Height(30)))
        {
            _paused = false;
            if (_day != null) _day.gameSpeed = 1f;
            // Reload scene
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
        if (GUILayout.Button("Quit", GUILayout.Height(30))) Application.Quit();
    }
}
