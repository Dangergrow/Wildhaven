using UnityEngine;
using UnityEngine.UI;

/// <summary>Full colonist info panel with tabs — replaces OnGUI info box.</summary>
public class ColonistPanel : MonoBehaviour
{
    private Canvas _canvas;
    private GameObject _panel;
    private Text _title, _content;
    private Colonist _viewedColonist;
    private int _tab; // 0=Skills, 1=Equipment, 2=Inventory, 3=Health, 4=Schedule
    private string[] _tabNames = { "Skills", "Gear", "Items", "Health", "Sched" };
    private SelectionManager _select;

    void Start()
    {
        _select = FindObjectOfType<SelectionManager>();
        var go = new GameObject("ColonistPanelCanvas");
        _canvas = go.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        go.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        go.AddComponent<GraphicRaycaster>();

        // Panel at bottom-left
        _panel = new GameObject("Panel");
        _panel.transform.SetParent(_canvas.transform);
        var rt = _panel.AddComponent<RectTransform>();
        var img = _panel.AddComponent<Image>();
        img.color = new Color(0.1f, 0.1f, 0.12f, 0.9f);
        rt.anchorMin = new Vector2(0, 0.1f); rt.anchorMax = new Vector2(0.3f, 0.5f);
        rt.offsetMin = rt.offsetMax = Vector2.zero;

        _title = MakeText("Title", _panel.transform, new Vector2(0.5f, 0.95f), 16, TextAnchor.UpperCenter);

        // Tabs
        for (int i = 0; i < 5; i++) { int idx = i; AddTabBtn(idx); }

        _content = MakeText("Content", _panel.transform, new Vector2(0.5f, 0.07f), 12, TextAnchor.LowerCenter);
        _content.rectTransform.sizeDelta = new Vector2(240, 180);

        _panel.SetActive(false);

        // Close button
        AddBtn("X", new Vector2(0.95f, 0.95f), _panel.transform, () => _panel.SetActive(false));
    }

    void Update()
    {
        if (_select == null) return;
        var c = _select.selectedColonist;
        if (c == null) { _panel.SetActive(false); return; }
        if (c != _viewedColonist) { _viewedColonist = c; _tab = 0; }
        _panel.SetActive(true);
        RefreshContent();
    }

    void RefreshContent()
    {
        if (_viewedColonist == null) return;
        var c = _viewedColonist;
        _title.text = $"{c.colonistName}  Age:{c.age}  {(c.isMale ? "M" : "F")}";

        string txt = "";
        switch (_tab)
        {
            case 0: // Skills
                txt = $"Build:{c.constructionSkill} Mine:{c.miningSkill} Cook:{c.cookingSkill}\n" +
                      $"Intel:{c.intellectualSkill} Med:{c.medicineSkill} Melee:{c.meleeSkill}\n" +
                      $"Range:{c.rangedSkill} Craft:{c.craftingSkill} Farm:{c.farmingSkill}\n" +
                      $"Social:{c.socialSkill} Animal:{c.animalHandlingSkill} Hunt:{c.huntingSkill}\n" +
                      $"Trade:{c.tradingSkill} Art:{c.artisticSkill}\n" +
                      $"Perk: {c.perk}   Flaw: {c.flaw}";
                break;
            case 1: // Equipment
                var eq = c.GetComponent<Equipment>();
                txt = eq != null ? $"Weapon: {(eq.weapon.itemType == 0 ? "None" : eq.weapon.itemType.ToString())}\nHead: {(eq.armorHead.itemType == 0 ? "None" : eq.armorHead.itemType.ToString())}\nBody: {(eq.armorBody.itemType == 0 ? "None" : eq.armorBody.itemType.ToString())}\nLegs: {(eq.armorLegs.itemType == 0 ? "None" : eq.armorLegs.itemType.ToString())}\nTool: {(eq.tool.itemType == 0 ? "None" : eq.tool.itemType.ToString())}" : "No equipment";
                break;
            case 2: // Inventory
                var inv = c.GetComponent<Inventory>();
                if (inv != null)
                {
                    int count = 0;
                    foreach (var s in inv.Slots) if (s.amount > 0) { txt += $"{s.itemType} x{s.amount}\n"; count++; if (count > 15) { txt += "..."; break; } }
                    if (count == 0) txt = "Empty";
                }
                else txt = "No inventory";
                break;
            case 3: // Health
                txt = $"HP: {c.health:F0}/{c.maxHealth:F0}\nMood: {c.mood:F0}\nHunger: {c.hunger:F0}\nThirst: {c.thirst:F0}\n" +
                      $"Fatigue: {c.fatigue:F0}\nComfort: {c.comfort:F0}\nSocial: {c.social:F0}\nRecreation: {c.recreation:F0}\nFaith: {c.faith:F0}\nState: {c.currentState}";
                break;
            case 4: // Schedule
                var sched = c.GetComponent<ColonistSchedule>();
                if (sched != null)
                {
                    txt = "Schedule (24h):\n";
                    string[] colors = { "■", "■", "■", "■" }; // Sleep=blue, Work=yellow, Rec=green, Any=gray
                    for (int h = 0; h < 24; h++)
                    {
                        var block = sched.schedule[h];
                        txt += block switch
                        {
                            ColonistSchedule.Block.Sleep => "<color=#4488ff>█</color>",
                            ColonistSchedule.Block.Work => "<color=#ffff44>█</color>",
                            ColonistSchedule.Block.Recreation => "<color=#44ff44>█</color>",
                            _ => "<color=#666666>█</color>",
                        };
                        if (h == 11) txt += "\n";
                    }
                    txt += "\n<color=#4488ff>Sleep</color> <color=#ffff44>Work</color> <color=#44ff44>Rec</color> <color=#666666>Any</color>";
                }
                else txt = "No schedule component";
                break;
        }
        _content.text = txt;
    }

    void AddTabBtn(int idx)
    {
        AddBtn(_tabNames[idx], new Vector2(0.25f + idx * 0.15f, 0.88f), _panel.transform, () => { _tab = idx; RefreshContent(); });
    }

    Text MakeText(string name, Transform parent, Vector2 anchor, int size, TextAnchor align)
    {
        var tGo = new GameObject(name);
        tGo.AddComponent<RectTransform>();
        var t = tGo.AddComponent<Text>();
        t.transform.SetParent(parent);
        t.rectTransform.anchorMin = t.rectTransform.anchorMax = anchor;
        t.rectTransform.sizeDelta = new Vector2(200, 25);
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = size; t.alignment = align; t.color = Color.white;
        return t;
    }

    void AddBtn(string label, Vector2 anchor, Transform parent, System.Action onClick)
    {
        var btnGo = new GameObject($"Btn_{label}");
        btnGo.AddComponent<RectTransform>();
        var btn = btnGo.AddComponent<Button>();
        btn.transform.SetParent(parent);
        var rt = btn.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = anchor; rt.sizeDelta = new Vector2(50, 22);
        var txtGo = new GameObject("Lbl");
        txtGo.AddComponent<RectTransform>();
        var txt = txtGo.AddComponent<Text>();
        txt.transform.SetParent(btn.transform);
        txt.rectTransform.anchorMin = txt.rectTransform.anchorMax = Vector2.one * 0.5f;
        txt.rectTransform.sizeDelta = new Vector2(50, 22);
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.fontSize = 10; txt.alignment = TextAnchor.MiddleCenter; txt.color = Color.white; txt.text = label;
        btn.onClick.AddListener(() => onClick());
    }
}
