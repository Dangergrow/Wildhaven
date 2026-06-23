using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>Central UI manager: ONE master canvas, all screens as child panels.</summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Canvas MasterCanvas { get; private set; }

    private GameObject _mainMenuPanel;
    private GameObject _settingsPanel;
    private GameObject _worldSettingsPanel;
    private GameObject _characterPanel;

    private Stack<GameObject> _screenStack = new();

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        var go = new GameObject("MasterCanvas");
        MasterCanvas = go.AddComponent<Canvas>();
        MasterCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        MasterCanvas.sortingOrder = 0;
        go.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        go.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
        go.AddComponent<GraphicRaycaster>();

        // Single EventSystem
        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
    }

    public void RegisterPanel(string name, GameObject panel)
    {
        panel.transform.SetParent(MasterCanvas.transform, false);
        switch (name)
        {
            case "MainMenu": _mainMenuPanel = panel; break;
            case "Settings": _settingsPanel = panel; break;
            case "WorldSettings": _worldSettingsPanel = panel; break;
            case "CharacterCreator": _characterPanel = panel; break;
        }
    }

    public void ShowScreen(string name)
    {
        // Hide current top screen
        if (_screenStack.Count > 0)
            _screenStack.Peek().SetActive(false);

        GameObject panel = name switch
        {
            "MainMenu" => _mainMenuPanel,
            "Settings" => _settingsPanel,
            "WorldSettings" => _worldSettingsPanel,
            "CharacterCreator" => _characterPanel,
            _ => null
        };

        if (panel != null)
        {
            panel.SetActive(true);
            panel.transform.SetAsLastSibling(); // bring to front
            _screenStack.Push(panel);
        }
    }

    public void HideScreen(string name)
    {
        GameObject panel = name switch
        {
            "MainMenu" => _mainMenuPanel,
            "Settings" => _settingsPanel,
            "WorldSettings" => _worldSettingsPanel,
            "CharacterCreator" => _characterPanel,
            _ => null
        };
        if (panel != null) panel.SetActive(false);

        // Pop from stack if it's the top
        if (_screenStack.Count > 0 && _screenStack.Peek() == panel)
            _screenStack.Pop();

        // Show previous screen
        if (_screenStack.Count > 0)
            _screenStack.Peek().SetActive(true);
    }

    public void CloseAll()
    {
        while (_screenStack.Count > 0)
            _screenStack.Pop().SetActive(false);
    }
}
