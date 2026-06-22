using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>Going Medieval-style character creator: 3 colonists, names, skills, perks.</summary>
public class CharacterCreator : MonoBehaviour
{
    [System.Serializable]
    public class ColonistTemplate
    {
        public string colonistName = "Settler";
        public int age = 25;
        public bool isMale = true;
        public int[] skills = new int[14];
        public Perk perk;
        public Flaw flaw;
        public string backstory = "Farmer";
        public string hairColor = "Brown";
        public string bodyType = "Average";
    }

    public ColonistTemplate[] colonists = new ColonistTemplate[3];
    private int _currentColonist;
    private int _skillPoints = 30; // points to distribute

    private Canvas _canvas;
    private Text _titleText, _pointsText, _infoText;
    private Text[] _skillLabels = new Text[14];
    private string[] _skillNames = { "Construction", "Mining", "Cooking", "Intellectual", "Medicine", "Melee", "Ranged", "Crafting", "Farming", "Social", "Animal", "Hunting", "Trading", "Artistic" };
    private string[] _perkNames = { "None","FastLearner","Workaholic","Tough","IronWill","GreenThumb","EagleEye","SilverTongue","NightOwl","Cannibal","Pyromaniac","Gourmet","Ascetic","Psychopath" };
    private string[] _flawNames = { "None","Lazy","Coward","SlowLearner","Frail","Depressive","Ugly","Glutton","Alcoholic","Bloodlust","Insomniac","Pyrophobic" };
    private string[] _backstories = { "Farmer", "Soldier", "Scholar", "Bandit", "Shaman", "Slave", "Merchant", "Nomad", "Artist", "Hunter" };

    public System.Action<ColonistTemplate[]> OnComplete; // callback when done

    void Start()
    {
        // Init default colonists
        string[] defaultNames = { "Ivan", "Anna", "Boris" };
        for (int i = 0; i < 3; i++)
        {
            colonists[i] = new ColonistTemplate { colonistName = defaultNames[i], age = Random.Range(20, 40), isMale = i != 1 };
            for (int s = 0; s < 14; s++) colonists[i].skills[s] = 3;
        }
        BuildUI();
    }

    void BuildUI()
    {
        var go = new GameObject("CharCreatorCanvas");
        _canvas = go.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        go.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        go.AddComponent<GraphicRaycaster>();

        // Background
        var bgGo = new GameObject("Bg");
        bgGo.AddComponent<RectTransform>();
        var bg = bgGo.AddComponent<Image>();
        bg.transform.SetParent(_canvas.transform);
        bg.rectTransform.anchorMin = Vector2.zero; bg.rectTransform.anchorMax = Vector2.one;
        bg.color = new Color(0.05f, 0.05f, 0.08f, 1f);

        _titleText = MakeText("Title", new Vector2(0.5f, 0.92f), 28, TextAnchor.UpperCenter);
        _pointsText = MakeText("Points", new Vector2(0.85f, 0.88f), 18, TextAnchor.UpperLeft);
        _infoText = MakeText("Info", new Vector2(0.5f, 0.08f), 14, TextAnchor.LowerCenter);

        // Colonist tabs
        for (int i = 0; i < 3; i++) { int idx = i; AddButton($"Colonist {i+1}", new Vector2(0.2f + i * 0.3f, 0.85f), () => SelectColonist(idx)); }

        // Skills list on the left
        for (int i = 0; i < 14; i++)
        {
            int idx = i;
            _skillLabels[i] = MakeText($"Skill{i}", new Vector2(0.15f, 0.78f - i * 0.045f), 13, TextAnchor.MiddleLeft);
            AddButton("-", new Vector2(0.42f, 0.78f - i * 0.045f), () => AdjustSkill(idx, -1));
            AddButton("+", new Vector2(0.5f, 0.78f - i * 0.045f), () => AdjustSkill(idx, 1));
        }

        // Perk/Flaw/Backstory on the right
        AddButton("< Perk", new Vector2(0.65f, 0.75f), () => CyclePerk(-1));
        AddButton("Perk >", new Vector2(0.75f, 0.75f), () => CyclePerk(1));
        AddButton("< Flaw", new Vector2(0.65f, 0.65f), () => CycleFlaw(-1));
        AddButton("Flaw >", new Vector2(0.75f, 0.65f), () => CycleFlaw(1));
        AddButton("< Backstory", new Vector2(0.65f, 0.55f), () => CycleBackstory(-1));
        AddButton("Backstory >", new Vector2(0.75f, 0.55f), () => CycleBackstory(1));

        // Name and age
        AddButton("Name", new Vector2(0.65f, 0.45f), () => { string[] rn = {"Ivan","Anna","Boris","Olga","Dmitri","Elena","Sergei","Natasha","Viktor","Maria"}; colonists[_currentColonist].colonistName = rn[Random.Range(0, rn.Length)]; Refresh(); });
        AddButton("Age -", new Vector2(0.65f, 0.4f), () => { colonists[_currentColonist].age = Mathf.Max(16, colonists[_currentColonist].age - 1); Refresh(); });
        AddButton("Age +", new Vector2(0.75f, 0.4f), () => { colonists[_currentColonist].age = Mathf.Min(70, colonists[_currentColonist].age + 1); Refresh(); });
        AddButton("< Hair", new Vector2(0.65f, 0.33f), () => CycleHairColor(-1));
        AddButton("Hair >", new Vector2(0.75f, 0.33f), () => CycleHairColor(1));
        AddButton("< Body", new Vector2(0.65f, 0.28f), () => CycleBodyType(-1));
        AddButton("Body >", new Vector2(0.75f, 0.28f), () => CycleBodyType(1));

        // Random + Done
        AddButton("Random Colonist", new Vector2(0.5f, 0.25f), Randomize);
        AddButton("START GAME", new Vector2(0.5f, 0.15f), () => { gameObject.SetActive(false); OnComplete?.Invoke(colonists); });

        SelectColonist(0);
    }

    void SelectColonist(int i) { _currentColonist = i; Refresh(); }

    void AdjustSkill(int idx, int delta)
    {
        var c = colonists[_currentColonist];
        if (delta > 0 && _skillPoints <= 0) return;
        if (delta < 0 && c.skills[idx] <= 0) return;
        c.skills[idx] = Mathf.Clamp(c.skills[idx] + delta, 0, 20);
        _skillPoints -= delta;
        Refresh();
    }

    void CyclePerk(int dir)
    {
        var c = colonists[_currentColonist];
        int cur = (int)c.perk;
        cur = (cur + dir + _perkNames.Length) % _perkNames.Length;
        c.perk = (Perk)cur;
        Refresh();
    }

    void CycleFlaw(int dir)
    {
        var c = colonists[_currentColonist];
        int cur = (int)c.flaw;
        cur = (cur + dir + _flawNames.Length) % _flawNames.Length;
        c.flaw = (Flaw)cur;
        Refresh();
    }

    void CycleBackstory(int dir)
    {
        var c = colonists[_currentColonist];
        int cur = System.Array.IndexOf(_backstories, c.backstory);
        if (cur < 0) cur = 0;
        cur = (cur + dir + _backstories.Length) % _backstories.Length;
        c.backstory = _backstories[cur];
        Refresh();
    }

    void Randomize()
    {
        var c = colonists[_currentColonist];
        c.age = Random.Range(18, 55);
        c.isMale = Random.value > 0.5f;
        for (int i = 0; i < 14; i++) c.skills[i] = Random.Range(0, 8);
        c.perk = (Perk)Random.Range(0, _perkNames.Length);
        c.flaw = (Flaw)Random.Range(0, _flawNames.Length);
        c.backstory = _backstories[Random.Range(0, _backstories.Length)];
        _skillPoints = 30;
        Refresh();
    }

    string[] _hairColors = { "Brown", "Black", "Blonde", "Red", "Gray", "White" };
    string[] _bodyTypes = { "Average", "Thin", "Muscular", "Heavy" };

    void CycleHairColor(int dir)
    {
        var c = colonists[_currentColonist];
        int idx = System.Array.IndexOf(_hairColors, c.hairColor);
        if (idx < 0) idx = 0;
        idx = (idx + dir + _hairColors.Length) % _hairColors.Length;
        c.hairColor = _hairColors[idx];
        Refresh();
    }

    void CycleBodyType(int dir)
    {
        var c = colonists[_currentColonist];
        int idx = System.Array.IndexOf(_bodyTypes, c.bodyType);
        if (idx < 0) idx = 0;
        idx = (idx + dir + _bodyTypes.Length) % _bodyTypes.Length;
        c.bodyType = _bodyTypes[idx];
        Refresh();
    }

    void Refresh()
    {
        var c = colonists[_currentColonist];
        _titleText.text = $"Colonist {_currentColonist + 1}: {c.colonistName}";
        _pointsText.text = $"Points: {_skillPoints}";
        for (int i = 0; i < 14; i++) _skillLabels[i].text = $"{_skillNames[i]}: {c.skills[i]}";
        _infoText.text = $"Age:{c.age} | Hair:{c.hairColor} | Body:{c.bodyType} | Perk:{c.perk} | Flaw:{c.flaw} | {c.backstory}";
    }

    Text MakeText(string name, Vector2 anchor, int size, TextAnchor align)
    {
        var tGo = new GameObject(name);
        tGo.AddComponent<RectTransform>();
        var t = tGo.AddComponent<Text>();
        t.transform.SetParent(_canvas.transform);
        t.rectTransform.anchorMin = t.rectTransform.anchorMax = anchor;
        t.rectTransform.sizeDelta = new Vector2(300, 30);
        t.font = UIFont.Get();
        t.fontSize = size; t.alignment = align; t.color = Color.white; return t;
    }

    void AddButton(string label, Vector2 pos, System.Action onClick)
    {
        var btnGo = new GameObject($"Btn_{label}");
        btnGo.AddComponent<RectTransform>();
        var btn = btnGo.AddComponent<Button>();
        btn.transform.SetParent(_canvas.transform);
        var rt = btn.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = pos; rt.sizeDelta = new Vector2(90, 26);
        var txtGo = new GameObject("Lbl");
        txtGo.AddComponent<RectTransform>();
        var txt = txtGo.AddComponent<Text>();
        txt.transform.SetParent(btn.transform);
        txt.rectTransform.anchorMin = txt.rectTransform.anchorMax = Vector2.one * 0.5f;
        txt.rectTransform.sizeDelta = new Vector2(90, 26);
        txt.font = UIFont.Get();
        txt.fontSize = 11; txt.alignment = TextAnchor.MiddleCenter; txt.color = Color.white; txt.text = label;
        btn.onClick.AddListener(() => onClick());
    }
}
