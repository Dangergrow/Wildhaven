using UnityEngine;
using UnityEngine.UI;

/// <summary>Full main menu: New Game, Continue, Multiplayer, Settings, About, Quit.</summary>
public class MainMenu : MonoBehaviour
{
    private Canvas _canvas;
    private ColonistSpawner _spawner;

    void Start()
    {
        _spawner = FindFirstObjectByType<ColonistSpawner>();

        var go = new GameObject("__MenuCanvas__");
        go.AddComponent<RectTransform>();
        _canvas = go.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 999;
        go.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        go.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
        go.AddComponent<GraphicRaycaster>();

        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            var esGo = new GameObject("EventSystem");
            esGo.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esGo.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // Background
        var bgGo = new GameObject("__Bg__"); bgGo.AddComponent<RectTransform>();
        var bg = bgGo.AddComponent<Image>();
        bg.rectTransform.SetParent(_canvas.transform, false);
        bg.rectTransform.anchorMin = Vector2.zero; bg.rectTransform.anchorMax = Vector2.one;
        bg.rectTransform.offsetMin = bg.rectTransform.offsetMax = Vector2.zero;
        bg.color = new Color(0.03f, 0.03f, 0.06f, 0.95f);

        // Title
        Txt("WILDHAVEN", 48, 0.78f, Color.white);
        Txt("Colony Simulator", 18, 0.72f, new Color(0.6f, 0.6f, 0.6f));

        // Buttons
        Btn("New Game", 0.56f, () => StartNewGame());
        Btn("Continue", 0.47f, () => {
            if (!System.IO.File.Exists(System.IO.Path.Combine(Application.persistentDataPath, "game.sav")))
            { Debug.Log("[Menu] No save file found"); return; }
            StartGame();
        });
        Btn("Multiplayer", 0.40f, () => BtnClick("Multiplayer — coming soon"));
        Btn("Settings", 0.33f, () => {
            var gs = FindFirstObjectByType<GameSettings>();
            if (gs != null) gs.Show();
            else Debug.Log("[Menu] GameSettings not found — is GameManager running?");
        });
        Btn("About", 0.26f, () => {
            var aGo = new GameObject("AboutPanel");
            aGo.AddComponent<RectTransform>();
            var aCanvas = aGo.AddComponent<Canvas>();
            aCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            aCanvas.sortingOrder = 2000;
            aGo.AddComponent<CanvasScaler>();
            aGo.AddComponent<GraphicRaycaster>();
            var abg = new GameObject("Bg"); abg.AddComponent<RectTransform>();
            var abgImg = abg.AddComponent<Image>();
            abgImg.rectTransform.SetParent(aCanvas.transform, false);
            abgImg.rectTransform.anchorMin = Vector2.zero; abgImg.rectTransform.anchorMax = Vector2.one;
            abgImg.color = new Color(0.05f, 0.05f, 0.08f, 0.95f);
            var at = new GameObject("T"); at.AddComponent<RectTransform>();
            var atxt = at.AddComponent<Text>();
            atxt.rectTransform.SetParent(aCanvas.transform, false);
            atxt.rectTransform.anchorMin = atxt.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            atxt.rectTransform.sizeDelta = new Vector2(500, 200);
            atxt.font = UIFont.Get(); atxt.fontSize = 20; atxt.alignment = TextAnchor.MiddleCenter;
            atxt.color = Color.white;
            atxt.text = "WILDHAVEN v0.1\n\nColony Simulator | Unity 6 URP | C#\n\nInspired by Going Medieval & RimWorld\nBuilt with OpenCode AI Agents\n\nFeatures:\n• Voxel world 100×32×100\n• 15 biomes, 10+ factions\n• 60+ research nodes, 5 eras\n• 31 cooking recipes, 20 animal types\n• Stability system, electricity, religion\n• Global hex map with caravans\n\nClick anywhere to close";
            var aBtn = aGo.AddComponent<Button>();
            aBtn.onClick.AddListener(() => Destroy(aGo));
            var ac = aBtn.colors; ac.normalColor = Color.clear; ac.highlightedColor = Color.clear; aBtn.colors = ac;
        });
        Btn("Quit", 0.19f, () => Application.Quit());
    }

    void StartNewGame()
    {
        DeleteSave();
        Destroy(_canvas.gameObject);

        // Step 1: World Settings
        var wsGo = new GameObject("__WorldSettings__");
        var ws = wsGo.AddComponent<WorldSettings>();
        ws.OnComplete = (cfg) =>
        {
            var grid = FindFirstObjectByType<GridManager>();
            if (grid != null)
            {
                grid.seed = cfg.seed;
                grid.worldWidth = cfg.mapSize;
                grid.worldDepth = cfg.mapSize;
                grid.Regenerate();
            }

            // Reset spawner so colonists appear on new terrain
            var spawner = FindFirstObjectByType<ColonistSpawner>();
            if (spawner != null) spawner.ResetSpawn();

            ApplyPlanetConfig(cfg);

            var ccGo = new GameObject("__CharacterCreator__");
            var cc = ccGo.AddComponent<CharacterCreator>();
            cc.OnComplete = (templates) =>
            {
                var spawner = FindFirstObjectByType<ColonistSpawner>();
                if (spawner != null)
                {
                    spawner.templates = templates;
                    spawner.useTemplates = true;
                    spawner.gameStarted = true;
                }
                StartGame();
            };
        };
    }

    void StartGame()
    {
        if (_canvas != null) Destroy(_canvas.gameObject);

        var saveMgr = FindFirstObjectByType<GameSaveManager>();
        if (saveMgr != null && System.IO.File.Exists(System.IO.Path.Combine(Application.persistentDataPath, "game.sav")))
            saveMgr.LoadGame();

        Destroy(this);
    }

    void ApplyPlanetConfig(WorldSettings.WorldConfig cfg)
    {
        // Apply apocalyptic mode
        if (cfg.apocalyptic)
        {
            var grid = FindFirstObjectByType<GridManager>();
            if (grid != null)
            {
                // Convert some surface blocks to ruins/wasteland
                var rng = new System.Random(cfg.seed);
                int w = grid.Width, d = grid.Depth;
                for (int x = 0; x < w; x++)
                for (int z = 0; z < d; z++)
                {
                    for (int y = grid.Height - 1; y > 0; y--)
                    {
                        BlockType b = grid.GetBlock(x, y, z);
                        if (b == BlockType.Grass && rng.Next(100) < 30)
                            grid.SetBlock(x, y, z, BlockType.Gravel); // dead ground
                        else if (b == BlockType.Dirt && rng.Next(100) < 15)
                            grid.SetBlock(x, y, z, BlockType.Coal);   // scorched earth
                        else if (b != BlockType.Air && b != BlockType.Water && rng.Next(100) < 2)
                            grid.SetBlock(x, y, z, BlockType.Obsidian); // ruins
                    }
                }
            }
        }

        // Apply difficulty to RaidManager
        var raid = FindFirstObjectByType<RaidManager>();
        if (raid != null)
        {
            raid.raidInterval = 120f / cfg.raidFrequency; // base 120s, scaled by frequency
        }

        // Store planet type for animal/resource generation
        PlayerPrefs.SetString("PlanetType", cfg.planet.ToString());
        PlayerPrefs.SetFloat("Difficulty", cfg.difficulty);
        PlayerPrefs.Save();
    }

    void BtnClick(string msg) { Debug.Log($"[Menu] {msg}"); }

    void DeleteSave()
    {
        var p = System.IO.Path.Combine(Application.persistentDataPath, "game.sav");
        if (System.IO.File.Exists(p)) System.IO.File.Delete(p);
        p = System.IO.Path.Combine(Application.persistentDataPath, "world.sav");
        if (System.IO.File.Exists(p)) System.IO.File.Delete(p);
    }

    void Txt(string msg, int size, float y, Color c)
    {
        var go = new GameObject("__T__"); go.AddComponent<RectTransform>();
        var t = go.AddComponent<Text>();
        t.rectTransform.SetParent(_canvas.transform, false);
        t.rectTransform.anchorMin = t.rectTransform.anchorMax = new Vector2(0.5f, y);
        t.rectTransform.sizeDelta = new Vector2(400, 50);
        t.font = UIFont.Get();
        t.fontSize = size; t.alignment = TextAnchor.MiddleCenter; t.color = c; t.text = msg;
    }

    void Btn(string label, float y, System.Action onClick)
    {
        var go = new GameObject("__B__"); go.AddComponent<RectTransform>();
        go.AddComponent<Image>().rectTransform.SetParent(_canvas.transform, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, y);
        rt.sizeDelta = new Vector2(250, 45);
        var btn = go.AddComponent<Button>();
        var c = btn.colors;
        c.normalColor = new Color(0.15f, 0.15f, 0.2f, 1f);
        c.highlightedColor = new Color(0.3f, 0.4f, 0.3f, 1f);
        btn.colors = c;

        var lg = new GameObject("__L__"); lg.AddComponent<RectTransform>();
        var lt = lg.AddComponent<Text>();
        lt.rectTransform.SetParent(go.transform, false);
        lt.rectTransform.anchorMin = Vector2.zero; lt.rectTransform.anchorMax = Vector2.one;
        lt.font = UIFont.Get();
        lt.fontSize = 22; lt.alignment = TextAnchor.MiddleCenter; lt.color = Color.white; lt.text = label;
        btn.onClick.AddListener(() => onClick());
    }
}
