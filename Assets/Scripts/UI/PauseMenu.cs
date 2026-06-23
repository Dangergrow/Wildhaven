using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>Esc = toggle pause menu overlay. Continue, Save/Load, Settings, Main Menu, Quit.</summary>
public class PauseMenu : MonoBehaviour
{
    private bool _paused;
    private DayCycle _day;
    private Rect _windowRect = new Rect(Screen.width / 2f - 120, Screen.height / 2f - 100, 240, 220);

    void Start()
    {
        _day = FindFirstObjectByType<DayCycle>();
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
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
        if (GUILayout.Button(L10n.Get("pause_continue"), GUILayout.Height(30))) { _paused = false; if (_day != null) _day.gameSpeed = 1f; }
        if (GUILayout.Button(L10n.Get("pause_save"), GUILayout.Height(30)))
        {
            GameSaveManager sv = FindFirstObjectByType<GameSaveManager>();
            if (sv != null) sv.SaveGame();
        }
        if (GUILayout.Button(L10n.Get("pause_load"), GUILayout.Height(30)))
        {
            GameSaveManager sv = FindFirstObjectByType<GameSaveManager>();
            if (sv != null) sv.LoadGame();
        }
        if (GUILayout.Button(L10n.Get("pause_settings"), GUILayout.Height(30)))
        {
            GameSettings gs = FindFirstObjectByType<GameSettings>();
            if (gs != null) gs.Show();
            _paused = false;
            if (_day != null) _day.gameSpeed = 1f;
        }
        if (GUILayout.Button(L10n.Get("pause_menu"), GUILayout.Height(30)))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
        if (GUILayout.Button(L10n.Get("pause_quit"), GUILayout.Height(30))) Application.Quit();
    }
}
