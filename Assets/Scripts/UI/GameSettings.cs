using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/// <summary>Simple signal fire: when built, calls trader. Settings: volume, quality.</summary>
public class SignalFire : MonoBehaviour
{
    public static SignalFire Instance;

    void Awake() { Instance = this; }

    /// <summary>Call a trader to the colony. Costs 50 copper.</summary>
    public bool CallTrader(Vector3Int firePos)
    {
        var econ = FindFirstObjectByType<EconomyManager>();
        if (econ == null || !econ.ModifyMoney(-50)) return false;

        var tradeUI = FindFirstObjectByType<TradeUI>();
        if (tradeUI != null) tradeUI.Show();

        // Notify
        var hud = FindFirstObjectByType<CanvasHUD>();
        if (hud != null) hud.AddNotification("Trader arriving!");
        return true;
    }
}

/// <summary>Basic settings: music volume, SFX volume, graphics quality.</summary>
public class GameSettings : MonoBehaviour
{
    public float musicVolume = 0.8f;
    public float sfxVolume = 1f;
    public int qualityLevel = 2; // 0=low, 1=med, 2=high
    public bool fullscreen = true;

    private Canvas _canvas;
    private bool _visible;

    void Start()
    {
        var go = new GameObject("SettingsCanvas");
        _canvas = go.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 200;
        go.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        go.AddComponent<GraphicRaycaster>();
        _canvas.gameObject.SetActive(false);
        BuildUI();
    }

    void Update()
    {
        if (Keyboard.current == null) return;
        if (Keyboard.current.escapeKey.wasPressedThisFrame && !_visible) { Show(); return; }
        if (_visible && Keyboard.current.escapeKey.wasPressedThisFrame) Hide();
    }

    void Show() { _visible = true; _canvas.gameObject.SetActive(true); }
    void Hide() { _visible = false; _canvas.gameObject.SetActive(false); }

    void BuildUI()
    {
        var bgGo = new GameObject("Bg");
        bgGo.AddComponent<RectTransform>();
        var bg = bgGo.AddComponent<Image>();
        bg.transform.SetParent(_canvas.transform);
        bg.rectTransform.anchorMin = Vector2.zero; bg.rectTransform.anchorMax = Vector2.one;
        bg.color = new Color(0.05f, 0.05f, 0.08f, 0.95f);

        MakeText("SETTINGS", new Vector2(0.5f, 0.9f), 30);
        MakeText("Music Volume", new Vector2(0.3f, 0.7f), 18);
        MakeText("SFX Volume", new Vector2(0.3f, 0.6f), 18);
        MakeText("Quality", new Vector2(0.3f, 0.5f), 18);
        MakeText("Fullscreen", new Vector2(0.3f, 0.4f), 18);
        MakeText("ESC to close", new Vector2(0.5f, 0.15f), 14);

        AddBtn("Music -", new Vector2(0.55f, 0.7f), () => musicVolume = Mathf.Max(0, musicVolume - 0.1f));
        AddBtn("Music +", new Vector2(0.65f, 0.7f), () => musicVolume = Mathf.Min(1, musicVolume + 0.1f));
        AddBtn("SFX -", new Vector2(0.55f, 0.6f), () => sfxVolume = Mathf.Max(0, sfxVolume - 0.1f));
        AddBtn("SFX +", new Vector2(0.65f, 0.6f), () => sfxVolume = Mathf.Min(1, sfxVolume + 0.1f));
        AddBtn("Q -", new Vector2(0.55f, 0.5f), () => { qualityLevel = Mathf.Max(0, qualityLevel - 1); QualitySettings.SetQualityLevel(qualityLevel); });
        AddBtn("Q +", new Vector2(0.65f, 0.5f), () => { qualityLevel = Mathf.Min(2, qualityLevel + 1); QualitySettings.SetQualityLevel(qualityLevel); });
        AddBtn("Toggle", new Vector2(0.6f, 0.4f), () => { fullscreen = !fullscreen; Screen.fullScreen = fullscreen; });
        AddBtn("Resume", new Vector2(0.5f, 0.25f), Hide);
    }

    void MakeText(string msg, Vector2 anchor, int size)
    {
        var tGo = new GameObject("Txt");
        tGo.AddComponent<RectTransform>();
        var t = tGo.AddComponent<Text>();
        t.transform.SetParent(_canvas.transform);
        t.rectTransform.anchorMin = t.rectTransform.anchorMax = anchor;
        t.rectTransform.sizeDelta = new Vector2(300, 35);
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = size; t.alignment = TextAnchor.MiddleCenter; t.color = Color.white; t.text = msg;
    }

    void AddBtn(string label, Vector2 pos, System.Action onClick)
    {
        var btnGo = new GameObject("Btn");
        btnGo.AddComponent<RectTransform>();
        var btn = btnGo.AddComponent<Button>();
        btn.transform.SetParent(_canvas.transform);
        var rt = btn.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = pos; rt.sizeDelta = new Vector2(80, 30);
        var txtGo = new GameObject("Lbl");
        txtGo.AddComponent<RectTransform>();
        var txt = txtGo.AddComponent<Text>();
        txt.transform.SetParent(btn.transform);
        txt.rectTransform.anchorMin = txt.rectTransform.anchorMax = Vector2.one * 0.5f;
        txt.rectTransform.sizeDelta = new Vector2(80, 30);
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.fontSize = 14; txt.alignment = TextAnchor.MiddleCenter; txt.color = Color.white; txt.text = label;
        btn.onClick.AddListener(() => onClick());
    }
}
