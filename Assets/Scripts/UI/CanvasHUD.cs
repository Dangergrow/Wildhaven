using UnityEngine;
using UnityEngine.UI;

/// <summary>Full Canvas HUD: time, colonists, block selection panel, mode.</summary>
public class CanvasHUD : MonoBehaviour
{
    private DayCycle _day;
    private ColonistSpawner _spawner;
    private BuildManager _build;
    private SelectionManager _select;
    private Text _timeText, _colonistText, _modeText, _resourceText;
    private GameObject _buildPanel;
    private Text[] _blockBtns;
    private int _btnCount = 18;
    private BlockType[] _btnTypes = {
        BlockType.Dirt, BlockType.Grass, BlockType.Stone, BlockType.Wood, BlockType.Glass,
        BlockType.StoneBrick, BlockType.WoodPlanks, BlockType.Sand, BlockType.Snow,
        BlockType.Marble, BlockType.Obsidian, BlockType.CopperOre, BlockType.GoldOre,
        BlockType.IronOre, BlockType.Coal, BlockType.Ice, BlockType.Clay, BlockType.Gravel,
    };

    void Start()
    {
        _day = FindObjectOfType<DayCycle>();
        _spawner = FindObjectOfType<ColonistSpawner>();
        _build = FindObjectOfType<BuildManager>();
        _select = FindObjectOfType<SelectionManager>();

        var canvas = new GameObject("CanvasHUD").AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.gameObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvas.gameObject.AddComponent<GraphicRaycaster>();

        _timeText = CreateText("Time", canvas.transform, new Vector2(0.5f, 0.97f), 20, TextAnchor.UpperCenter);
        _colonistText = CreateText("Colonists", canvas.transform, new Vector2(0.01f, 0.96f), 14, TextAnchor.UpperLeft);
        _resourceText = CreateText("Resources", canvas.transform, new Vector2(0.01f, 0.92f), 12, TextAnchor.UpperLeft);
        _modeText = CreateText("Mode", canvas.transform, new Vector2(0.01f, 0.02f), 13, TextAnchor.LowerLeft);

        // Build panel — right side, vertical buttons
        _buildPanel = new GameObject("BuildPanel");
        _buildPanel.transform.SetParent(canvas.transform);
        var bpRT = _buildPanel.AddComponent<RectTransform>();
        bpRT.anchorMin = bpRT.anchorMax = new Vector2(0.99f, 0.5f);
        bpRT.pivot = new Vector2(1, 0.5f);
        bpRT.sizeDelta = new Vector2(90, _btnCount * 30 + 30);
        bpRT.anchoredPosition = Vector2.zero;

        var bg = _buildPanel.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.5f);

        CreateLabel("Build", _buildPanel.transform, 0);
        _blockBtns = new Text[_btnCount];
        for (int i = 0; i < _btnCount; i++)
        {
            int idx = i;
            var btn = new GameObject($"Btn_{_btnTypes[i]}").AddComponent<Button>();
            btn.transform.SetParent(_buildPanel.transform);
            var rt = btn.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0);
            rt.pivot = new Vector2(0.5f, 0);
            rt.anchoredPosition = new Vector2(0, 30 + i * 30);
            rt.sizeDelta = new Vector2(70, 26);

            var txt = new GameObject("Label").AddComponent<Text>();
            txt.transform.SetParent(btn.transform);
            txt.rectTransform.anchorMin = txt.rectTransform.anchorMax = Vector2.one * 0.5f;
            txt.rectTransform.sizeDelta = new Vector2(70, 26);
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            txt.fontSize = 12;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = Color.white;
            txt.text = i < 9 ? $"{i+1}:{_btnTypes[i]}" : $"⇧{i-8}:{_btnTypes[i]}";
            _blockBtns[i] = txt;

            var c = btn.colors;
            c.normalColor = new Color(0.3f, 0.3f, 0.3f, 0.8f);
            c.highlightedColor = new Color(0.5f, 0.5f, 0.5f, 0.9f);
            btn.colors = c;

            btn.onClick.AddListener(() => {
                if (_build != null) _build.SetSelectedType(_btnTypes[idx]);
            });
        }
    }

    void CreateLabel(string text, Transform parent, float y)
    {
        var go = new GameObject("Label").AddComponent<Text>();
        go.transform.SetParent(parent);
        var rt = go.rectTransform;
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0);
        rt.pivot = new Vector2(0.5f, 0);
        rt.anchoredPosition = new Vector2(0, y + 5);
        rt.sizeDelta = new Vector2(70, 20);
        go.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        go.fontSize = 11;
        go.alignment = TextAnchor.MiddleCenter;
        go.color = Color.yellow;
        go.text = text;
    }

    Text CreateText(string name, Transform parent, Vector2 anchor, int size, TextAnchor align)
    {
        var go = new GameObject(name).AddComponent<Text>();
        go.transform.SetParent(parent);
        go.rectTransform.anchorMin = anchor; go.rectTransform.anchorMax = anchor;
        go.rectTransform.pivot = anchor; go.rectTransform.sizeDelta = new Vector2(500, 30);
        go.rectTransform.anchoredPosition = Vector2.zero;
        go.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        go.fontSize = size; go.alignment = align;
        go.color = Color.white;
        return go;
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
            int c = 0, a = 0; int wood = 0, stone = 0, food = 0, metal = 0;
            foreach (var col in _spawner.Colonists)
            {
                c++; if (col.currentState != ColonistState.Dead) a++;
                var inv = col.GetComponent<Inventory>();
                if (inv != null)
                {
                    foreach (var s in inv.Slots)
                    {
                        if (s.itemType == ItemType.WoodLog) wood += s.amount;
                        if (s.itemType == ItemType.StoneBlock) stone += s.amount;
                        if (s.itemType == ItemType.IronIngot || s.itemType == ItemType.CopperIngot || s.itemType == ItemType.SteelIngot) metal += s.amount;
                        if (s.itemType == ItemType.RawMeat || s.itemType == ItemType.CookedMeat || s.itemType == ItemType.Bread || s.itemType == ItemType.Berries || s.itemType == ItemType.RationPack) food += s.amount;
                    }
                }
            }
            _colonistText.text = $"Colonists: {a}/{c}";
            _resourceText.text = $"Wood:{wood}  Stone:{stone}  Food:{food}  Metal:{metal}";
        }

        if (_select != null)
            _modeText.text = $"B=Build/Select  F5 Save  F9 Load  Space Pause  1/2/3 Speed";

        // Highlight active block button
        if (_build != null && _blockBtns != null)
        {
            for (int i = 0; i < _btnCount; i++)
            {
                bool active = _btnTypes[i] == _build.SelectedType;
                _blockBtns[i].color = active ? Color.yellow : Color.white;
            }
        }
    }
}
