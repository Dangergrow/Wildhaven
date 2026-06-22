using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>Going Medieval-style HUD: resource bar, colonist portraits, architect categories, notifications.</summary>
public class CanvasHUD : MonoBehaviour
{
    private Canvas _canvas;
    private DayCycle _day; private ColonistSpawner _spawner; private BuildManager _build; private SelectionManager _select;
    private Text _timeText, _resourceText, _modeText;
    private GameObject _buildPanel;
    private Text[] _blockBtns;
    private int _archCategory; // 0=Walls, 1=Floors, 2=Furniture, 3=Production, 4=Defense, 5=Misc
    private string[] _catNames = { "Walls", "Floors", "Furniture", "Production", "Defense", "Misc" };
    private BlockType[][] _catBlocks = {
        new[]{BlockType.Wood, BlockType.WoodPlanks, BlockType.Stone, BlockType.StoneBrick, BlockType.Dirt, BlockType.Sand, BlockType.Gravel, BlockType.Clay, BlockType.Marble, BlockType.Obsidian},
        new[]{BlockType.Wood, BlockType.Stone, BlockType.StoneBrick, BlockType.WoodPlanks, BlockType.Gravel, BlockType.Marble},
        new[]{BlockType.Wood, BlockType.WoodPlanks, BlockType.StoneBrick, BlockType.Marble, BlockType.Glass},
        new[]{BlockType.IronOre, BlockType.CopperOre, BlockType.Coal, BlockType.GoldOre, BlockType.StoneBrick},
        new[]{BlockType.Wood, BlockType.Stone, BlockType.StoneBrick, BlockType.IronOre},
        new[]{BlockType.Snow, BlockType.Ice, BlockType.Glass, BlockType.Obsidian, BlockType.Sand},
    };
    private List<GameObject> _portraitIcons = new();
    private List<string> _notifications = new();
    private Text _notifText;

    void Start()
    {
        _day = FindFirstObjectByType<DayCycle>();
        _spawner = FindFirstObjectByType<ColonistSpawner>();
        _build = FindFirstObjectByType<BuildManager>();
        _select = FindFirstObjectByType<SelectionManager>();

        _canvas = new GameObject("HUDCanvas").AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.gameObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        _canvas.gameObject.AddComponent<GraphicRaycaster>();

        _timeText = MakeText("Time", _canvas.transform, new Vector2(0.5f, 0.97f), 20, TextAnchor.UpperCenter);
        _resourceText = MakeText("Resources", _canvas.transform, new Vector2(0.01f, 0.94f), 12, TextAnchor.UpperLeft);
        _modeText = MakeText("Mode", _canvas.transform, new Vector2(0.01f, 0.02f), 12, TextAnchor.LowerLeft);

        // Colonist portrait bar (top-left, under resource bar)
        // Created dynamically in Update based on colonist count

        // Notification window (top-right)
        _notifText = MakeText("Notifs", _canvas.transform, new Vector2(0.99f, 0.92f), 11, TextAnchor.UpperRight);
        _notifText.rectTransform.sizeDelta = new Vector2(250, 100);

        // Build panel — right side with categories
        _buildPanel = new GameObject("BuildPanel");
        _buildPanel.transform.SetParent(_canvas.transform);
        var bpRT = _buildPanel.AddComponent<RectTransform>();
        bpRT.anchorMin = bpRT.anchorMax = new Vector2(0.99f, 0.4f);
        bpRT.pivot = new Vector2(1, 0.5f);
        bpRT.sizeDelta = new Vector2(120, 260);

        var bg = _buildPanel.AddComponent<Image>();
        bg.color = new Color(0.1f, 0.1f, 0.12f, 0.85f);

        // Category tabs
        for (int i = 0; i < 6; i++) { int idx = i; AddCatTab(_canvas.transform, idx); }

        ShowArchBlocks();

        _canvas.gameObject.SetActive(false);
        #if UNITY_EDITOR
        _canvas.gameObject.SetActive(true); // show HUD directly in editor
        #endif
    }

    void AddCatTab(Transform parent, int idx)
    {
        var btnGo = new GameObject($"Cat_{_catNames[idx]}");
        btnGo.AddComponent<RectTransform>();
        var btn = btnGo.AddComponent<Button>();
        btn.transform.SetParent(parent);
        var rt = btn.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.99f, 0.85f - idx * 0.055f);
        rt.pivot = new Vector2(1, 0.5f);
        rt.anchoredPosition = Vector2.zero; rt.sizeDelta = new Vector2(110, 20);

            var txtGo = new GameObject("Lbl");
            txtGo.AddComponent<RectTransform>();
            var txt = txtGo.AddComponent<Text>();
        txt.transform.SetParent(btn.transform);
        txt.rectTransform.anchorMin = txt.rectTransform.anchorMax = Vector2.one * 0.5f;
        txt.rectTransform.sizeDelta = new Vector2(110, 20);
        txt.font = UIFont.Get();
        txt.fontSize = 11; txt.alignment = TextAnchor.MiddleCenter; txt.color = Color.white;
        txt.text = _catNames[idx];

        var c = btn.colors;
        c.normalColor = new Color(0.2f, 0.2f, 0.25f, 0.9f);
        c.highlightedColor = new Color(0.4f, 0.5f, 0.4f, 1f);
        btn.colors = c;
        btn.onClick.AddListener(() => { _archCategory = idx; ShowArchBlocks(); });
    }

    void ShowArchBlocks()
    {
        // Clear old block buttons
        if (_blockBtns != null) foreach (var b in _blockBtns) if (b != null) Destroy(b.transform.parent.gameObject);
        var blocks = _catBlocks[_archCategory];
        _blockBtns = new Text[blocks.Length];
        for (int i = 0; i < blocks.Length; i++)
        {
            int idx = i; BlockType bt = blocks[i];
            var go = new GameObject($"Blk_{bt}");
            go.transform.SetParent(_buildPanel.transform);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0.1f, 0.75f - i * 0.075f);
            rt.sizeDelta = new Vector2(100, 22);

            var btn = go.AddComponent<Button>();
        var txtGo = new GameObject("Lbl");
        txtGo.AddComponent<RectTransform>();
        var txt = txtGo.AddComponent<Text>();
            txt.transform.SetParent(go.transform);
            txt.rectTransform.anchorMin = txt.rectTransform.anchorMax = Vector2.one * 0.5f;
            txt.rectTransform.sizeDelta = new Vector2(100, 22);
            txt.font = UIFont.Get();
            txt.fontSize = 10; txt.alignment = TextAnchor.MiddleCenter; txt.color = Color.white;
            txt.text = $"{bt}";
            _blockBtns[i] = txt;

            var c2 = btn.colors;
            c2.normalColor = new Color(0.25f, 0.25f, 0.3f, 0.8f);
            c2.highlightedColor = new Color(0.5f, 0.5f, 0.2f, 1f);
            btn.colors = c2;
            btn.onClick.AddListener(() => { if (_build != null) _build.SetSelectedType(bt); });
        }
    }

    void Update()
    {
        if (_day != null)
        {
            string spd = _day.IsPaused ? "PAUSED" : $"{_day.gameSpeed}x";
            _timeText.text = $"Day {_day.day}  {_day.hour:D2}:{_day.minute:D2}  {spd}";
        }

        if (_spawner != null)
        {
            int c = 0, a = 0, wood = 0, stone = 0, food = 0, metal = 0;
            foreach (var col in _spawner.Colonists)
            {
                c++; if (col.currentState != ColonistState.Dead) a++;
                var inv = col.GetComponent<Inventory>();
                if (inv != null)
                    foreach (var s in inv.Slots)
                    {
                        if (s.itemType == ItemType.WoodLog) wood += s.amount;
                        else if (s.itemType == ItemType.StoneBlock) stone += s.amount;
                        else if ((int)s.itemType >= (int)ItemType.IronIngot && (int)s.itemType <= (int)ItemType.GoldIngot) metal += s.amount;
                        else if (s.itemType == ItemType.RawMeat || s.itemType == ItemType.CookedMeat || s.itemType == ItemType.Bread || s.itemType == ItemType.Berries || s.itemType == ItemType.RationPack || s.itemType == ItemType.Fish) food += s.amount;
                    }
            }
            _resourceText.text = $"W:{wood} S:{stone} F:{food} M:{metal}  Colonists: {a}/{c}";
        }

        if (_select != null)
            _modeText.text = "F1-Architect F2-Work F3-Zone F4-Orders | R-Draft | B-Select | F5 Save";

        // Notifications
        if (_notifications.Count > 0)
        {
            _notifText.text = string.Join("\n", _notifications);
            if (_notifications.Count > 5) _notifications.RemoveAt(0); // keep last 5
        }
    }

    public void AddNotification(string msg)
    {
        _notifications.Add(msg);
        if (_notifications.Count > 10) _notifications.RemoveAt(0);
    }

    public void Show() { if (_canvas != null) _canvas.gameObject.SetActive(true); }

    Text MakeText(string name, Transform parent, Vector2 anchor, int size, TextAnchor align)
    {
        var tGo = new GameObject(name);
        tGo.AddComponent<RectTransform>();
        var t = tGo.AddComponent<Text>();
        t.transform.SetParent(parent);
        t.rectTransform.anchorMin = t.rectTransform.anchorMax = anchor;
        t.rectTransform.sizeDelta = new Vector2(500, 30);
        t.rectTransform.anchoredPosition = Vector2.zero;
        t.font = UIFont.Get();
        t.fontSize = size; t.alignment = align; t.color = Color.white; return t;
    }
}
