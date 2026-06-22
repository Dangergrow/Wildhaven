using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Full main menu: New Game, Continue, Multiplayer, Settings, About, Quit.
/// In editor: auto-skips to game.
/// </summary>
public class MainMenu : MonoBehaviour
{
    private Canvas _canvas;
    private ColonistSpawner _spawner;
    private static bool _firstStart = true;

    void Start()
    {
#if UNITY_EDITOR
        if (_firstStart)
        {
            _firstStart = false;
            StartGame();
            return;
        }
#endif

        _spawner = FindFirstObjectByType<ColonistSpawner>();

        var go = new GameObject("__MenuCanvas__");
        go.AddComponent<RectTransform>();
        _canvas = go.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 999;
        go.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
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
            atxt.text = "WILDHAVEN v0.1\n\nColony Simulator\nUnity 6 URP | C#\n\nBuilt with OpenCode AI Agents\nWork PC + Home PC collaboration\n\nClick anywhere to close";
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
        ws.OnComplete = (seed, size, difficulty) =>
        {
            // Apply world settings and regenerate
            var grid = FindFirstObjectByType<GridManager>();
            if (grid != null)
            {
                grid.seed = seed;
                grid.worldWidth = size;
                grid.worldDepth = size;
                grid.Regenerate();
            }

            // Step 2: Character Creator
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
