using UnityEngine;
using UnityEngine.UI;

/// <summary>Basic settings: audio, video, language, keybinds placeholder.</summary>
public class GameSettings : MonoBehaviour
{
    public float musicVolume = 0.8f;
    public float sfxVolume = 1f;
    public int qualityLevel = 2;
    public bool fullscreen = true;
    public int fpsCap = 0; // 0=unlimited, 30, 60, 120
    public int languageIndex; // 0=ENG, 1=RUS
    public bool showKeybinds;

    private Canvas _canvas;
    private int _page;

    void Start()
    {
        var go = new GameObject("SettingsCanvas");
        _canvas = go.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 2000;
        go.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        go.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
        go.AddComponent<GraphicRaycaster>();
        _canvas.gameObject.SetActive(false);
        BuildUI();
    }

    public void Show() { _canvas.gameObject.SetActive(true); }
    public void Hide() { _canvas.gameObject.SetActive(false); }

    void BuildUI()
    {
        var bgGo = new GameObject("Bg"); bgGo.AddComponent<RectTransform>();
        var bg = bgGo.AddComponent<Image>();
        bg.transform.SetParent(_canvas.transform);
        bg.rectTransform.anchorMin = Vector2.zero; bg.rectTransform.anchorMax = Vector2.one;
        bg.color = new Color(0.05f, 0.05f, 0.08f, 0.95f);

        Txt("SETTINGS", new Vector2(0.5f, 0.92f), 30);

        // Audio
        Txt("AUDIO", new Vector2(0.5f, 0.83f), 22);
        AddRow("Music:", new Vector2(0.5f, 0.77f), () => musicVolume = Mathf.Max(0, musicVolume - 0.1f), () => musicVolume = Mathf.Min(1, musicVolume + 0.1f), $"{musicVolume:F1}");
        AddRow("SFX:", new Vector2(0.5f, 0.71f), () => sfxVolume = Mathf.Max(0, sfxVolume - 0.1f), () => sfxVolume = Mathf.Min(1, sfxVolume + 0.1f), $"{sfxVolume:F1}");

        // Video
        Txt("VIDEO", new Vector2(0.5f, 0.63f), 22);
        AddRow("Quality:", new Vector2(0.5f, 0.57f), () => { qualityLevel = Mathf.Max(0, qualityLevel - 1); QualitySettings.SetQualityLevel(qualityLevel); },
            () => { qualityLevel = Mathf.Min(2, qualityLevel + 1); QualitySettings.SetQualityLevel(qualityLevel); },
            qualityLevel == 0 ? "Low" : qualityLevel == 1 ? "Med" : "High");
        AddRow("Fullscreen:", new Vector2(0.5f, 0.51f), null, null, fullscreen ? "ON" : "OFF");
        Btn("Toggle", new Vector2(0.38f, 0.51f), () => { fullscreen = !fullscreen; Screen.fullScreen = fullscreen; });
        AddRow("FPS Cap:", new Vector2(0.5f, 0.45f),
            () => { fpsCap = fpsCap == 120 ? 60 : fpsCap == 60 ? 30 : fpsCap == 30 ? 0 : 120; Application.targetFrameRate = fpsCap; },
            () => { fpsCap = fpsCap == 0 ? 30 : fpsCap == 30 ? 60 : fpsCap == 60 ? 120 : 0; Application.targetFrameRate = fpsCap; },
            fpsCap == 0 ? "Unlimited" : fpsCap.ToString());

        // Language
        Txt("LANGUAGE", new Vector2(0.5f, 0.37f), 22);
        string[] langs = { "ENG", "RUS" };
        AddRow("Language:", new Vector2(0.5f, 0.31f),
            () => languageIndex = (languageIndex + langs.Length - 1) % langs.Length,
            () => languageIndex = (languageIndex + 1) % langs.Length,
            langs[languageIndex]);

        // Keybinds
        Btn("KEY BINDINGS", new Vector2(0.5f, 0.22f), () => showKeybinds = !showKeybinds);

        Btn("CLOSE", new Vector2(0.5f, 0.10f), Hide);
    }

    void Txt(string msg, Vector2 anchor, int size)
    {
        var tGo = new GameObject("T"); tGo.AddComponent<RectTransform>();
        var t = tGo.AddComponent<Text>();
        t.transform.SetParent(_canvas.transform);
        t.rectTransform.anchorMin = t.rectTransform.anchorMax = anchor;
        t.rectTransform.sizeDelta = new Vector2(400, size + 15);
        t.font = UIFont.Get(); t.fontSize = size; t.alignment = TextAnchor.MiddleCenter; t.color = Color.white; t.text = msg;
    }

    void AddRow(string label, Vector2 pos, System.Action onLeft, System.Action onRight, string value)
    {
        Txt($"{label} {value}", pos, 16);
        if (onLeft != null) Btn("-", new Vector2(pos.x - 0.18f, pos.y), onLeft);
        if (onRight != null) Btn("+", new Vector2(pos.x + 0.18f, pos.y), onRight);
    }

    void Btn(string label, Vector2 pos, System.Action onClick)
    {
        var btnGo = new GameObject("B"); btnGo.AddComponent<RectTransform>();
        var btn = btnGo.AddComponent<Button>();
        btn.transform.SetParent(_canvas.transform);
        var rt = btn.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = pos; rt.sizeDelta = new Vector2(120, 32);
        var c = btn.colors;
        c.normalColor = new Color(0.15f, 0.15f, 0.22f, 1f);
        c.highlightedColor = new Color(0.3f, 0.5f, 0.3f, 1f);
        btn.colors = c;

        var txtGo = new GameObject("L"); txtGo.AddComponent<RectTransform>();
        var txt = txtGo.AddComponent<Text>();
        txt.transform.SetParent(btn.transform);
        txt.rectTransform.anchorMin = txt.rectTransform.anchorMax = Vector2.one * 0.5f;
        txt.rectTransform.sizeDelta = new Vector2(120, 32);
        txt.font = UIFont.Get(); txt.fontSize = 14; txt.alignment = TextAnchor.MiddleCenter; txt.color = Color.white; txt.text = label;
        btn.onClick.AddListener(() => onClick());
    }
}

/// <summary>Simple signal fire: when built, calls trader. Costs 50 copper.</summary>
public class SignalFire : MonoBehaviour
{
    public static SignalFire Instance;
    void Awake() { Instance = this; }
    public bool CallTrader(Vector3Int firePos)
    {
        var econ = FindFirstObjectByType<EconomyManager>();
        if (econ == null || !econ.ModifyMoney(-50)) return false;
        var tradeUI = FindFirstObjectByType<TradeUI>();
        if (tradeUI != null) tradeUI.Show();
        var hud = FindFirstObjectByType<CanvasHUD>();
        if (hud != null) hud.AddNotification("Trader arriving!");
        return true;
    }
}
