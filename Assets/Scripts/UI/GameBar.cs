using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;

/// <summary>Going Medieval-style bottom bar with mode tabs + block categories.</summary>
public class GameBar : MonoBehaviour
{
    public enum Mode { Architect, Work, Zone, Orders }
    public Mode currentMode = Mode.Architect;

    private Canvas _canvas;
    private Text _modeText, _categoryText, _infoText;
    private List<Button> _tabButtons = new();
    private List<Button> _blockButtons = new();
    private List<Button> _orderButtons = new();
    private BuildManager _build;
    private OrderMarkerSystem _orders;
    private SelectionManager _select;

    // Architect subcategories
    private int _architectPage;
    private string[] _archCategories = { "Walls", "Floors", "Furniture", "Production", "Defense", "Misc" };
    private int _archCategory;
    private BlockType[][] _architectBlocks = new[] {
        new[]{BlockType.Wood, BlockType.WoodPlanks, BlockType.Stone, BlockType.StoneBrick, BlockType.Dirt, BlockType.Grass, BlockType.Sand, BlockType.Gravel, BlockType.Clay},
        new[]{BlockType.Marble, BlockType.Obsidian, BlockType.Glass, BlockType.IronOre, BlockType.CopperOre, BlockType.GoldOre, BlockType.Coal, BlockType.Ice, BlockType.Snow},
    };

    void Start()
    {
        _build = FindObjectOfType<BuildManager>();
        _select = FindObjectOfType<SelectionManager>();
        _orders = FindObjectOfType<OrderMarkerSystem>();
        if (_orders == null) { var ordersGo = new GameObject("OrderMarkerSystem"); _orders = ordersGo.AddComponent<OrderMarkerSystem>(); }

        // Canvas anchored to bottom
        var go = new GameObject("GameBarCanvas");
        _canvas = go.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        go.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        go.AddComponent<GraphicRaycaster>();

        // Bottom bar background
        var bgGo = new GameObject("BarBg");
        bgGo.AddComponent<RectTransform>();
        var bg = bgGo.AddComponent<Image>();
        bg.transform.SetParent(_canvas.transform);
        bg.rectTransform.anchorMin = new Vector2(0, 0); bg.rectTransform.anchorMax = new Vector2(1, 0);
        bg.rectTransform.pivot = new Vector2(0.5f, 0); bg.rectTransform.sizeDelta = new Vector2(0, 80);
        bg.color = new Color(0.1f, 0.1f, 0.12f, 0.9f);

        // Mode tabs
        CreateTab("Architect [F1]", 40, 0, () => SetMode(Mode.Architect));
        CreateTab("Work [F2]", 140, 1, () => SetMode(Mode.Work));
        CreateTab("Zone [F3]", 240, 2, () => SetMode(Mode.Zone));
        CreateTab("Orders [F4]", 340, 3, () => SetMode(Mode.Orders));

        // Info text
        _modeText = MakeText("Mode", _canvas.transform, new Vector2(0.01f, 0.06f), 13, TextAnchor.LowerLeft);
        _categoryText = MakeText("Category", _canvas.transform, new Vector2(0.01f, 0.035f), 11, TextAnchor.LowerLeft);
        _infoText = MakeText("Info", _canvas.transform, new Vector2(0.99f, 0.06f), 11, TextAnchor.LowerRight);

        SetMode(Mode.Architect);
        // Always visible — MainMenu sits on top with higher sort order
    }

    void Update()
    {
        if (Keyboard.current == null) return;
        if (Keyboard.current.f1Key.wasPressedThisFrame) SetMode(Mode.Architect);
        if (Keyboard.current.f2Key.wasPressedThisFrame) SetMode(Mode.Work);
        if (Keyboard.current.f3Key.wasPressedThisFrame) SetMode(Mode.Zone);
        if (Keyboard.current.f4Key.wasPressedThisFrame) SetMode(Mode.Orders);
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            int m = ((int)currentMode + 1) % 4;
            SetMode((Mode)m);
        }
        // Page switching for architect blocks
        if (currentMode == Mode.Architect)
        {
            if (Keyboard.current.leftBracketKey.wasPressedThisFrame) { _architectPage = (_architectPage + 1) % _architectBlocks.Length; ShowArchitectBlocks(); }
            if (Keyboard.current.rightBracketKey.wasPressedThisFrame) { _architectPage = (_architectPage + _architectBlocks.Length - 1) % _architectBlocks.Length; ShowArchitectBlocks(); }
        }

        if (_infoText != null) _infoText.text = "F5 Save  F9 Load  Space Pause  1/2/3 Speed";
    }

    void SetMode(Mode m)
    {
        currentMode = m;
        _modeText.text = $"[{m}]  < > change page";
        ClearBlockButtons();
        ClearOrderButtons();
        if (_orders != null) _orders.isPlacing = false;

        // Highlight active tab
        for (int i = 0; i < _tabButtons.Count; i++)
        {
            var c = _tabButtons[i].colors;
            c.normalColor = i == (int)m ? new Color(0.3f, 0.5f, 0.3f, 0.9f) : new Color(0.2f, 0.2f, 0.2f, 0.8f);
            _tabButtons[i].colors = c;
        }

        if (_select != null) _select.gameObject.SetActive(m != Mode.Architect);
        if (_build != null) _build.enabled = (m == Mode.Architect);

        switch (m)
        {
            case Mode.Architect: ShowArchitectBlocks(); _categoryText.text = "[1-9] blocks  [ ] page  Shift+LMB blueprint"; break;
            case Mode.Work: _categoryText.text = "F = prioritize  R = combat  C = cancel"; break;
            case Mode.Zone: _categoryText.text = "Stockpile  Dump  Farm  Room  Hospital  Temple"; break;
            case Mode.Orders: ShowOrderButtons(); _categoryText.text = "LMB place  RMB cancel  [1-6] type"; break;
        }
    }

    void ShowArchitectBlocks()
    {
        ClearBlockButtons();
        var blocks = _architectBlocks[_architectPage];
        for (int i = 0; i < blocks.Length; i++)
        {
            int idx = i; BlockType bt = blocks[i];
            var btnGo = new GameObject($"Block_{bt}");
            btnGo.AddComponent<RectTransform>();
            var btn = btnGo.AddComponent<Button>();
            btn.transform.SetParent(_canvas.transform);
            var rt = btn.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0);
            rt.anchoredPosition = new Vector2(-200 + i * 48, 52);
            rt.sizeDelta = new Vector2(44, 24);

            var txtGo = new GameObject("Label");
            txtGo.AddComponent<RectTransform>();
            var txt = txtGo.AddComponent<Text>();
            txt.transform.SetParent(btn.transform);
            txt.rectTransform.anchorMin = txt.rectTransform.anchorMax = Vector2.one * 0.5f;
            txt.rectTransform.sizeDelta = new Vector2(44, 24);
            txt.font = UIFont.Get();
            txt.fontSize = 9; txt.alignment = TextAnchor.MiddleCenter; txt.color = Color.white;
            txt.text = $"{i+1}\n{bt}";

            btn.onClick.AddListener(() => { if (_build != null) _build.SetSelectedType(bt); });
            _blockButtons.Add(btn);
        }
    }

    void ClearBlockButtons() { foreach (var b in _blockButtons) if (b != null) Destroy(b.gameObject); _blockButtons.Clear(); }
    void ClearOrderButtons() { foreach (var b in _orderButtons) if (b != null) Destroy(b.gameObject); _orderButtons.Clear(); }

    void ShowOrderButtons()
    {
        ClearBlockButtons();
        ClearOrderButtons();
        _orders.isPlacing = true;
        string[] labels = { "Mine", "Chop", "Harvest", "Hunt", "Haul", "Deconstruct" };
        OrderMarkerSystem.OrderKind[] kinds = { OrderMarkerSystem.OrderKind.Mine, OrderMarkerSystem.OrderKind.Chop, OrderMarkerSystem.OrderKind.Harvest, OrderMarkerSystem.OrderKind.Hunt, OrderMarkerSystem.OrderKind.Haul, OrderMarkerSystem.OrderKind.Deconstruct };
        for (int i = 0; i < labels.Length; i++)
        {
            int idx = i;
            var btnGo = new GameObject($"Order_{labels[i]}");
            btnGo.AddComponent<RectTransform>();
            var btn = btnGo.AddComponent<Button>();
            btn.transform.SetParent(_canvas.transform);
            var rt = btn.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0);
            rt.anchoredPosition = new Vector2(-150 + i * 63, 52);
            rt.sizeDelta = new Vector2(58, 24);

            var txtGo = new GameObject("Label");
            txtGo.AddComponent<RectTransform>();
            var txt = txtGo.AddComponent<Text>();
            txt.transform.SetParent(btn.transform);
            txt.rectTransform.anchorMin = txt.rectTransform.anchorMax = Vector2.one * 0.5f;
            txt.rectTransform.sizeDelta = new Vector2(58, 24);
            txt.font = UIFont.Get();
            txt.fontSize = 10; txt.alignment = TextAnchor.MiddleCenter; txt.color = Color.white;
            txt.text = $"{i+1}\n{labels[i]}";

            btn.onClick.AddListener(() => { _orders.selectedKind = kinds[idx]; });
            _orderButtons.Add(btn);
        }
        _orders.selectedKind = OrderMarkerSystem.OrderKind.Mine;
    }

    void CreateTab(string label, float x, int idx, System.Action onClick)
    {
        var btnGo = new GameObject($"Tab_{label}");
        btnGo.AddComponent<RectTransform>();
        var btn = btnGo.AddComponent<Button>();
        btn.transform.SetParent(_canvas.transform);
        var rt = btn.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0, 0);
        rt.anchoredPosition = new Vector2(x + 20, 8);
        rt.sizeDelta = new Vector2(130, 28);

        var txtGo = new GameObject("Label");
        txtGo.AddComponent<RectTransform>();
        var txt = txtGo.AddComponent<Text>();
        txt.transform.SetParent(btn.transform);
        txt.rectTransform.anchorMin = txt.rectTransform.anchorMax = Vector2.one * 0.5f;
        txt.rectTransform.sizeDelta = new Vector2(130, 28);
        txt.font = UIFont.Get();
        txt.fontSize = 12; txt.alignment = TextAnchor.MiddleCenter; txt.color = Color.white; txt.text = label;

        btn.onClick.AddListener(() => onClick());
        _tabButtons.Add(btn);
    }

    Text MakeText(string name, Transform parent, Vector2 anchor, int size, TextAnchor align)
    {
        var tGo = new GameObject(name);
        tGo.AddComponent<RectTransform>();
        var t = tGo.AddComponent<Text>();
        t.transform.SetParent(parent);
        t.rectTransform.anchorMin = t.rectTransform.anchorMax = anchor;
        t.rectTransform.pivot = anchor; t.rectTransform.sizeDelta = new Vector2(500, 20);
        t.rectTransform.anchoredPosition = Vector2.zero;
        t.font = UIFont.Get();
        t.fontSize = size; t.alignment = align; t.color = Color.white; return t;
    }

    public void Show() { if (_canvas != null) _canvas.gameObject.SetActive(true); }
}
