using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/// <summary>Fullscreen main menu. Blocks all game input until dismissed.</summary>
public class MainMenu : MonoBehaviour
{
    private Canvas _canvas;
    private bool _started;
    private DayCycle _day;

    void Awake()
    {
        _day = FindFirstObjectByType<DayCycle>();
        if (_day != null) _day.gameSpeed = 0f; // pause immediately
    }

    void Start()
    {
        // Create fullscreen Canvas
        var go = new GameObject("__MainMenuCanvas__");
        var rt = go.AddComponent<RectTransform>();
        _canvas = go.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 999;
        var scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        go.AddComponent<GraphicRaycaster>();

        // Create EventSystem — REQUIRED for UI buttons to work
        var esGo = new GameObject("EventSystem");
        esGo.AddComponent<UnityEngine.EventSystems.EventSystem>();
        esGo.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

        // Dark background filling entire screen
        var bgGo = new GameObject("__Bg__");
        bgGo.AddComponent<RectTransform>();
        var bg = bgGo.AddComponent<Image>();
        bg.rectTransform.SetParent(_canvas.transform, false);
        bg.rectTransform.anchorMin = Vector2.zero;
        bg.rectTransform.anchorMax = Vector2.one;
        bg.rectTransform.offsetMin = Vector2.zero;
        bg.rectTransform.offsetMax = Vector2.zero;
        bg.color = new Color(0.03f, 0.03f, 0.06f, 1f);

        // Title
        MakeText("WILDHAVEN", 44, new Vector2(0.5f, 0.75f), Color.white);
        MakeText("Colony Simulator", 18, new Vector2(0.5f, 0.69f), new Color(0.6f, 0.6f, 0.6f));

        // Buttons
        MakeButton("New Game", new Vector2(0.5f, 0.55f), () => {
            var savePath = System.IO.Path.Combine(Application.persistentDataPath, "game.sav");
            if (System.IO.File.Exists(savePath)) System.IO.File.Delete(savePath);
            StartGame();
        });
        MakeButton("Continue", new Vector2(0.5f, 0.45f), () => {
            var gsm = FindFirstObjectByType<GameSaveManager>();
            if (gsm != null && gsm.HasSave) gsm.LoadGame();
            StartGame();
        });
        MakeButton("Quit", new Vector2(0.5f, 0.35f), () => Application.Quit());

        // Disable all game input until menu closes
        var bm = FindFirstObjectByType<BuildManager>();
        var sm = FindFirstObjectByType<SelectionManager>();
        if (bm != null) bm.enabled = false;
        if (sm != null) sm.enabled = false;
    }

    void Update()
    {
        if (_started) return;
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            Application.Quit();
    }

    void MakeText(string msg, int size, Vector2 anchor, Color color)
    {
        var tGo = new GameObject("__Txt__");
        tGo.AddComponent<RectTransform>();
        var txt = tGo.AddComponent<Text>();
        txt.rectTransform.SetParent(_canvas.transform, false);
        txt.rectTransform.anchorMin = anchor;
        txt.rectTransform.anchorMax = anchor;
        txt.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        txt.rectTransform.sizeDelta = new Vector2(400, 50);
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.fontSize = size;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = color;
        txt.text = msg;
    }

    void MakeButton(string label, Vector2 anchor, System.Action onClick)
    {
        var btnGo = new GameObject("__Btn__");
        btnGo.AddComponent<RectTransform>();
        var img = btnGo.AddComponent<Image>();
        img.rectTransform.SetParent(_canvas.transform, false);
        img.rectTransform.anchorMin = anchor;
        img.rectTransform.anchorMax = anchor;
        img.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        img.rectTransform.sizeDelta = new Vector2(220, 50);

        var btn = btnGo.AddComponent<Button>();
        btn.targetGraphic = img;
        var colors = btn.colors;
        colors.normalColor = new Color(0.15f, 0.15f, 0.2f, 1f);
        colors.highlightedColor = new Color(0.3f, 0.4f, 0.3f, 1f);
        colors.pressedColor = new Color(0.1f, 0.2f, 0.1f, 1f);
        btn.colors = colors;

        var tGo = new GameObject("__Lbl__");
        tGo.AddComponent<RectTransform>();
        var txt = tGo.AddComponent<Text>();
        txt.rectTransform.SetParent(btnGo.transform, false);
        txt.rectTransform.anchorMin = Vector2.zero;
        txt.rectTransform.anchorMax = Vector2.one;
        txt.rectTransform.offsetMin = Vector2.zero;
        txt.rectTransform.offsetMax = Vector2.zero;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.fontSize = 22;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.white;
        txt.text = label;

        btn.onClick.AddListener(() => onClick());
    }

    void StartGame()
    {
        _started = true;
        Destroy(_canvas.gameObject);
        if (_day != null) _day.gameSpeed = 1f;
        var bm = FindFirstObjectByType<BuildManager>();
        var sm = FindFirstObjectByType<SelectionManager>();
        if (bm != null) bm.enabled = true;
        if (sm != null) sm.enabled = true;
        // Show game UI
        var hud = FindFirstObjectByType<CanvasHUD>();
        if (hud != null) hud.Show();
        var bar = FindFirstObjectByType<GameBar>();
        if (bar != null) bar.Show();
    }
}
