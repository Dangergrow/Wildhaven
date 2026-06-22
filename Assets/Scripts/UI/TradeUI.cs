using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;

/// <summary>Trading panel — appears when caravan arrives. Buy/sell items.</summary>
public class TradeUI : MonoBehaviour
{
    private Canvas _canvas;
    private bool _visible;
    private Text _fundsText, _repText, _infoText;
    private EconomyManager _econ;
    private FactionManager _faction;
    private Inventory _playerInv;
    private List<(ItemType, int)> _traderStock;
    private float _rep;

    void Start()
    {
        _econ = FindFirstObjectByType<EconomyManager>();
        if (_econ == null) _econ = gameObject.AddComponent<EconomyManager>();
        _faction = FindFirstObjectByType<FactionManager>();
        // Find ANY colonist inventory (not this GameObject)
        var spawner = FindFirstObjectByType<ColonistSpawner>();
        if (spawner != null && spawner.Colonists.Count > 0)
            _playerInv = spawner.Colonists[0].GetComponent<Inventory>();

        var go = new GameObject("TradeCanvas");
        _canvas = go.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 60;
        go.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        go.AddComponent<GraphicRaycaster>();

        var bgGo = new GameObject("Bg");
        bgGo.AddComponent<RectTransform>();
        var bg = bgGo.AddComponent<Image>();
        bg.transform.SetParent(_canvas.transform);
        bg.rectTransform.anchorMin = Vector2.zero; bg.rectTransform.anchorMax = Vector2.one;
        bg.color = new Color(0.06f, 0.06f, 0.08f, 0.93f);

        _fundsText = MakeText("Funds", new Vector2(0.5f, 0.92f), 20, TextAnchor.UpperCenter);
        _repText = MakeText("Rep", new Vector2(0.7f, 0.92f), 14, TextAnchor.UpperLeft);
        _infoText = MakeText("Info", new Vector2(0.5f, 0.08f), 13, TextAnchor.LowerCenter);

        // Buy button
        AddBtn("Buy Selected", new Vector2(0.35f, 0.12f), new Vector2(140, 30), BuySelected);
        AddBtn("Sell All", new Vector2(0.65f, 0.12f), new Vector2(140, 30), SellAll);
        AddBtn("Close [Esc]", new Vector2(0.5f, 0.05f), new Vector2(140, 30), () => Hide());

        _canvas.gameObject.SetActive(false);
    }

    public void Show()
    {
        _visible = true;
        _canvas.gameObject.SetActive(true);
        _rep = _faction != null ? _faction.GetAverageReputation() : 0;
        _traderStock = _econ.GenerateTraderStock();
        Refresh();
    }

    public bool IsVisible() => _visible;

    public void Hide() { _visible = false; _canvas.gameObject.SetActive(false); }

    void Update()
    {
        if (!_visible) return;
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) Hide();
        Refresh();
    }

    void Refresh()
    {
        _fundsText.text = $"Trade — Funds: {_econ.FundsDisplay}";
        _repText.text = $"Rep: {_rep:F0}";
        string list = "TRADER SELLS:\n";
        for (int i = 0; i < _traderStock.Count; i++)
        {
            var item = _traderStock[i];
            int price = _econ.GetBuyPrice(item.Item1, _rep);
            list += $"[{i+1}] {item.Item1} x{item.Item2} — {price}c each\n";
        }
        _infoText.text = list + "\nClick Buy to purchase first item. Sell All sells non-food items.";
    }

    void BuySelected()
    {
        if (_traderStock.Count == 0) return;
        var item = _traderStock[0];
        int totalCost = _econ.GetBuyPrice(item.Item1, _rep) * item.Item2;
        if (!_econ.ModifyMoney(-totalCost)) { Debug.Log("Not enough money!"); return; }
        if (_playerInv != null) _playerInv.AddItem(item.Item1, item.Item2);
        _traderStock.RemoveAt(0);
        Refresh();
    }

    void SellAll()
    {
        if (_playerInv == null) return;
        foreach (var slot in _playerInv.Slots)
        {
            if (slot.amount > 0 && slot.itemType != ItemType.Coin)
            {
                int price = _econ.GetSellPrice(slot.itemType, _rep);
                int earned = price * slot.amount;
                _econ.ModifyMoney(earned);
                slot.Clear();
            }
        }
        Refresh();
    }

    Text MakeText(string name, Vector2 anchor, int size, TextAnchor align)
    {
        var tGo = new GameObject(name);
        tGo.AddComponent<RectTransform>();
        var t = tGo.AddComponent<Text>();
        t.transform.SetParent(_canvas.transform);
        t.rectTransform.anchorMin = t.rectTransform.anchorMax = anchor;
        t.rectTransform.sizeDelta = new Vector2(400, 30);
        t.font = UIFont.Get();
        t.fontSize = size; t.alignment = align; t.color = Color.white; return t;
    }

    void AddBtn(string label, Vector2 pos, Vector2 size, System.Action onClick)
    {
        var btnGo = new GameObject($"Btn_{label}");
        btnGo.AddComponent<RectTransform>();
        var btn = btnGo.AddComponent<Button>();
        btn.transform.SetParent(_canvas.transform);
        var rt = btn.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = pos; rt.sizeDelta = size;
        var txtGo = new GameObject("Lbl");
        txtGo.AddComponent<RectTransform>();
        var txt = txtGo.AddComponent<Text>();
        txt.transform.SetParent(btn.transform);
        txt.rectTransform.anchorMin = txt.rectTransform.anchorMax = Vector2.one * 0.5f;
        txt.rectTransform.sizeDelta = size;
        txt.font = UIFont.Get();
        txt.fontSize = 13; txt.alignment = TextAnchor.MiddleCenter; txt.color = Color.white; txt.text = label;
        btn.onClick.AddListener(() => onClick());
    }
}
