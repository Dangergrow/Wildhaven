using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// World generation settings: planet, biome, map size, difficulty, apocalyptic mode.
/// </summary>
public class WorldSettings : MonoBehaviour
{
    public System.Action<WorldConfig> OnComplete;

    public class WorldConfig
    {
        public int seed;
        public int mapSize;
        public float difficulty;
        public PlanetType planet;
        public MapBiome forcedBiome;
        public bool apocalyptic;
        public bool pvpEnabled;
        public float raidFrequency = 1f;
    }

    public enum PlanetType
    {
        Earthlike,   // standard
        DesertWorld, // hot, rare water
        IceWorld,    // cold, frozen
        JungleWorld, // lush, dangerous
        DeadWorld,   // post-apocalyptic
    }

    private Canvas _canvas;
    private WorldConfig _cfg = new();
    private int _page; // 0=planet, 1=biome/size, 2=difficulty/mods

    void Start()
    {
        _cfg.seed = Random.Range(1, 1000000);
        _cfg.mapSize = 100;
        _cfg.difficulty = 1f;
        _cfg.planet = PlanetType.Earthlike;
        _cfg.apocalyptic = false;
        _cfg.pvpEnabled = false;

        var go = new GameObject("WorldSettingsCanvas");
        _canvas = go.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 1000;
        go.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        go.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
        go.AddComponent<GraphicRaycaster>();

        Bg();
        ShowPage();
    }

    void Bg()
    {
        var bgGo = new GameObject("Bg"); bgGo.AddComponent<RectTransform>();
        var bg = bgGo.AddComponent<Image>();
        bg.transform.SetParent(_canvas.transform, false);
        bg.rectTransform.anchorMin = Vector2.zero; bg.rectTransform.anchorMax = Vector2.one;
        bg.color = new Color(0.04f, 0.04f, 0.07f, 1f);
    }

    void Clear() { foreach (Transform c in _canvas.transform) if (c.name != "Bg") Destroy(c.gameObject); }

    void ShowPage()
    {
        Clear();
        switch (_page)
        {
            case 0: ShowPlanetPage(); break;
            case 1: ShowSizePage(); break;
            case 2: ShowModsPage(); break;
        }
    }

    // ═══ PAGE 0: PLANET ═══
    void ShowPlanetPage()
    {
        Txt("CHOOSE PLANET", 32, 0.88f, Color.white);

        PlanetBtn("Earthlike", 0.72f, PlanetType.Earthlike, "Standard world with balanced biomes.\nForests, plains, oceans.");
        PlanetBtn("Desert World", 0.58f, PlanetType.DesertWorld, "Hot and dry. Scarce water.\nSandstorms, rare oases.");
        PlanetBtn("Ice World", 0.44f, PlanetType.IceWorld, "Frozen wasteland. Extreme cold.\nLimited farming, unique crystals.");
        PlanetBtn("Jungle World", 0.30f, PlanetType.JungleWorld, "Dense vegetation. Dangerous\nfauna. High rainfall.");
        PlanetBtn("Dead World", 0.16f, PlanetType.DeadWorld, "Post-apocalyptic ruins.\nToxic zones, mutants, rare tech.");

        Btn("NEXT →", new Vector2(0.8f, 0.05f), () => { _page = 1; ShowPage(); });
    }

    void PlanetBtn(string label, float y, PlanetType p, string desc)
    {
        float h = 0.10f;
        Btn(label, new Vector2(0.25f, y), () => { _cfg.planet = p; });
        Txt(desc, 10, y, new Color(0.5f, 0.5f, 0.6f));
    }

    // ═══ PAGE 1: BIOME + SIZE ═══
    void ShowSizePage()
    {
        Txt($"PLANET: {_cfg.planet}", 24, 0.88f, Color.white);

        Txt("MAP SIZE", 20, 0.75f, new Color(0.7f, 0.7f, 0.7f));
        BtnRow("50×50", "100×100", "200×200", 0.68f,
            () => _cfg.mapSize = 50, () => _cfg.mapSize = 100, () => _cfg.mapSize = 200);

        Txt($"Selected: {_cfg.mapSize}×{_cfg.mapSize}", 14, 0.62f, new Color(0.5f, 0.5f, 0.5f));

        // Show biome preview based on planet
        Txt("CLIMATE & BIOMES", 20, 0.52f, new Color(0.7f, 0.7f, 0.7f));
        string biomeDesc = _cfg.planet switch
        {
            PlanetType.Earthlike => "Temperate forests, plains, tundra at poles.\nBalanced rainfall and temperatures.",
            PlanetType.DesertWorld => "Vast deserts, rocky badlands.\nRare savannah oases. Extreme heat.",
            PlanetType.IceWorld => "Ice wastes, frozen tundra.\nUnderground hot springs. Eternal winter.",
            PlanetType.JungleWorld => "Dense jungles, swamps, tropical forests.\nDangerous wildlife, year-round heat.",
            PlanetType.DeadWorld => "Wastelands, toxic swamps, ruins.\nDead forests, crystal caves. Mutants roam.",
            _ => "Unknown"
        };
        Txt(biomeDesc, 10, 0.38f, new Color(0.5f, 0.6f, 0.5f));

        Btn("← BACK", new Vector2(0.2f, 0.05f), () => { _page = 0; ShowPage(); });
        Btn("NEXT →", new Vector2(0.8f, 0.05f), () => { _page = 2; ShowPage(); });
    }

    // ═══ PAGE 2: DIFFICULTY + MODS ═══
    void ShowModsPage()
    {
        Txt("DIFFICULTY & MODIFIERS", 28, 0.88f, Color.white);

        Txt("Difficulty", 20, 0.76f, new Color(0.7f, 0.7f, 0.7f));
        BtnRow("Peaceful", "Normal", "Brutal", 0.69f,
            () => _cfg.difficulty = 0.3f, () => _cfg.difficulty = 1f, () => _cfg.difficulty = 2.5f);

        Txt($"Raid Frequency: {_cfg.raidFrequency:F1}x", 18, 0.58f, new Color(0.7f, 0.7f, 0.7f));
        BtnRow("Rare", "Normal", "Often", 0.51f,
            () => _cfg.raidFrequency = 0.3f, () => _cfg.raidFrequency = 1f, () => _cfg.raidFrequency = 3f);

        // Apocalyptic toggle
        string apoLabel = _cfg.apocalyptic ? "APOCALYPTIC: ON ☢" : "Apocalyptic: OFF";
        Btn(apoLabel, new Vector2(0.5f, 0.38f), () => _cfg.apocalyptic = !_cfg.apocalyptic);
        Txt("Ruins, toxic zones, mutants, rare ancient tech", 11, 0.33f, new Color(0.5f, 0.4f, 0.3f));

        // PvP toggle (for multiplayer)
        string pvpLabel = _cfg.pvpEnabled ? "PvP: ENABLED ⚔" : "PvP: Disabled";
        Btn(pvpLabel, new Vector2(0.5f, 0.24f), () => _cfg.pvpEnabled = !_cfg.pvpEnabled);

        Txt($"Seed: {_cfg.seed}", 14, 0.15f, new Color(0.4f, 0.4f, 0.4f));

        Btn("← BACK", new Vector2(0.2f, 0.05f), () => { _page = 1; ShowPage(); });
        Btn("START GAME", new Vector2(0.8f, 0.05f), () => { _canvas.gameObject.SetActive(false); OnComplete?.Invoke(_cfg); });
    }

    // ═══ HELPERS ═══
    void Txt(string msg, int size, float y, Color c)
    {
        var go = new GameObject("T"); go.AddComponent<RectTransform>();
        var t = go.AddComponent<Text>();
        t.transform.SetParent(_canvas.transform, false);
        t.rectTransform.anchorMin = t.rectTransform.anchorMax = new Vector2(0.5f, y);
        t.rectTransform.sizeDelta = new Vector2(500, Mathf.Max(size + 10, 30));
        t.font = UIFont.Get(); t.fontSize = size; t.alignment = TextAnchor.MiddleCenter; t.color = c; t.text = msg;
    }

    void Btn(string label, Vector2 pos, System.Action onClick)
    {
        var go = new GameObject("B"); go.AddComponent<RectTransform>();
        go.AddComponent<Image>().rectTransform.SetParent(_canvas.transform, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = pos;
        rt.sizeDelta = new Vector2(180, 38);
        var btn = go.AddComponent<Button>();
        var cc = btn.colors;
        cc.normalColor = new Color(0.15f, 0.15f, 0.22f, 1f);
        cc.highlightedColor = new Color(0.3f, 0.45f, 0.3f, 1f);
        if (label.Contains("START")) cc.highlightedColor = new Color(0.5f, 0.7f, 0.3f, 1f);
        btn.colors = cc;
        var lg = new GameObject("L"); lg.AddComponent<RectTransform>();
        var lt = lg.AddComponent<Text>();
        lt.transform.SetParent(go.transform, false);
        lt.rectTransform.anchorMin = Vector2.zero; lt.rectTransform.anchorMax = Vector2.one;
        lt.font = UIFont.Get(); lt.fontSize = label.Length > 15 ? 13 : 16;
        lt.alignment = TextAnchor.MiddleCenter; lt.color = Color.white; lt.text = label;
        btn.onClick.AddListener(() => onClick());
    }

    void BtnRow(string a, string b, string c, float y, System.Action actA, System.Action actB, System.Action actC)
    {
        Btn(a, new Vector2(0.28f, y), actA);
        Btn(b, new Vector2(0.5f, y), actB);
        Btn(c, new Vector2(0.72f, y), actC);
    }
}
