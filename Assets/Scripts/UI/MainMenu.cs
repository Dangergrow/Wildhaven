using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/// <summary>Main menu overlay — appears on start. Hides when game starts.</summary>
public class MainMenu : MonoBehaviour
{
    private Canvas _canvas;
    private bool _started;

    void Start()
    {
        var go = new GameObject("MenuCanvas");
        _canvas = go.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 100; // on top of everything
        go.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        go.AddComponent<GraphicRaycaster>();

        // Background
        var bg = new GameObject("Bg").AddComponent<Image>();
        bg.transform.SetParent(_canvas.transform);
        bg.rectTransform.anchorMin = Vector2.zero; bg.rectTransform.anchorMax = Vector2.one;
        bg.color = new Color(0.05f, 0.05f, 0.1f, 1f);

        // Title
        CreateText("WILDHAVEN", 0.8f, 48, Color.white);
        CreateText("Colony Simulator", 0.73f, 18, new Color(0.7f, 0.7f, 0.7f));

        // Buttons
        AddBtn("New Game", 0.55f, OnNewGame);
        AddBtn("Continue", 0.45f, OnContinue);
        AddBtn("Quit", 0.35f, () => Application.Quit());

        // ESC to show/hide
        // Handled in Update
    }

    void Update()
    {
        if (_started) return;
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            Application.Quit();
    }

    void CreateText(string msg, float y, int size, Color color)
    {
        var t = new GameObject("Text").AddComponent<Text>();
        t.transform.SetParent(_canvas.transform);
        t.rectTransform.anchorMin = t.rectTransform.anchorMax = new Vector2(0.5f, y);
        t.rectTransform.sizeDelta = new Vector2(500, 60);
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = size; t.alignment = TextAnchor.MiddleCenter;
        t.color = color; t.text = msg;
    }

    void AddBtn(string label, float y, System.Action onClick)
    {
        var btn = new GameObject($"Btn_{label}").AddComponent<Button>();
        btn.transform.SetParent(_canvas.transform);
        var rt = btn.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, y);
        rt.sizeDelta = new Vector2(220, 44);

        var txt = new GameObject("Label").AddComponent<Text>();
        txt.transform.SetParent(btn.transform);
        txt.rectTransform.anchorMin = txt.rectTransform.anchorMax = Vector2.one * 0.5f;
        txt.rectTransform.sizeDelta = new Vector2(220, 44);
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.fontSize = 20; txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.white; txt.text = label;

        var c = btn.colors;
        c.normalColor = new Color(0.25f, 0.25f, 0.3f, 0.9f);
        c.highlightedColor = new Color(0.4f, 0.4f, 0.5f, 1f);
        btn.colors = c;

        btn.onClick.AddListener(() => onClick());
    }

    void OnNewGame()
    {
        var savePath = System.IO.Path.Combine(Application.persistentDataPath, "world.sav");
        if (System.IO.File.Exists(savePath)) System.IO.File.Delete(savePath);

        // Show character creator, then start game
        var cc = gameObject.AddComponent<CharacterCreator>();
        cc.OnComplete = (templates) =>
        {
            var spawner = FindObjectOfType<ColonistSpawner>();
            if (spawner != null)
            {
                // Clear existing colonists
                foreach (var c in spawner.Colonists.ToArray())
                    Destroy(c.gameObject);
                spawner.Colonists.Clear();

                // Spawn from templates
                foreach (var t in templates)
                {
                    Vector3 pos = FindSpawnPos();
                    var go = Instantiate(spawner.colonistPrefab, pos, Quaternion.identity);
                    var col = go.GetComponent<Colonist>();
                    if (col == null) continue;
                    col.colonistName = t.colonistName;
                    col.age = t.age; col.isMale = t.isMale;
                    for (int i = 0; i < 14; i++) SetSkill(col, i, t.skills[i]);
                    col.perk = t.perk; col.flaw = t.flaw;
                    spawner.Colonists.Add(col);
                }
            }
            StartGame();
        };
        _canvas.gameObject.SetActive(false);
    }

    Vector3 FindSpawnPos()
    {
        var gm = FindObjectOfType<GridManager>();
        if (gm == null) return new Vector3(50, 15, 50);
        for (int x = 45; x < 55; x++)
        for (int z = 45; z < 55; z++)
        for (int y = gm.Height - 1; y > 5; y--)
        {
            if (gm.GetBlock(x, y, z) == BlockType.Air && gm.GetBlock(x, y - 1, z) == BlockType.Grass)
                return gm.GridToWorld(x, y, z);
        }
        return new Vector3(50, 15, 50);
    }

    void OnContinue()
    {
        StartGame();
    }

    void SetSkill(Colonist c, int idx, int val)
    {
        switch (idx)
        {
            case 0: c.constructionSkill = val; break; case 1: c.miningSkill = val; break;
            case 2: c.cookingSkill = val; break; case 3: c.intellectualSkill = val; break;
            case 4: c.medicineSkill = val; break; case 5: c.meleeSkill = val; break;
            case 6: c.rangedSkill = val; break; case 7: c.craftingSkill = val; break;
            case 8: c.farmingSkill = val; break; case 9: c.socialSkill = val; break;
            case 10: c.animalHandlingSkill = val; break; case 11: c.huntingSkill = val; break;
            case 12: c.tradingSkill = val; break; case 13: c.artisticSkill = val; break;
        }
    }

    void StartGame()
    {
        _started = true;
        _canvas.gameObject.SetActive(false);
        // Unpause if needed
        var day = FindObjectOfType<DayCycle>();
        if (day != null) day.gameSpeed = 1f;
    }
}

