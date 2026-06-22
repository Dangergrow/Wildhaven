using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// World generation settings before starting a new game.
/// Simple: choose seed, map size, and difficulty.
/// </summary>
public class WorldSettings : MonoBehaviour
{
    public System.Action<int, int, float> OnComplete; // seed, size, difficulty

    private Canvas _canvas;
    private int _seed = 12345;
    private int _mapSize = 100; // 50/100/200
    private float _difficulty = 1f; // 0.5-2.0

    void Start()
    {
        _seed = Random.Range(1, 1000000);

        var go = new GameObject("WorldSettingsCanvas");
        go.AddComponent<RectTransform>();
        _canvas = go.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 1000;
        go.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        go.AddComponent<GraphicRaycaster>();

        // Background
        var bgGo = new GameObject("Bg"); bgGo.AddComponent<RectTransform>();
        var bg = bgGo.AddComponent<Image>();
        bg.transform.SetParent(_canvas.transform, false);
        bg.rectTransform.anchorMin = Vector2.zero; bg.rectTransform.anchorMax = Vector2.one;
        bg.color = new Color(0.05f, 0.05f, 0.08f, 1f);

        Txt("WORLD SETTINGS", 32, 0.82f, Color.white);

        Txt($"Seed: {_seed}", 18, 0.65f, new Color(0.7f, 0.7f, 0.7f));
        Btn("Random Seed", new Vector2(0.5f, 0.58f), () => { _seed = Random.Range(1, 1000000); Refresh(); });

        Txt($"Map Size: {_mapSize}x{_mapSize}", 18, 0.50f, new Color(0.7f, 0.7f, 0.7f));
        Btn("50", new Vector2(0.35f, 0.43f), () => { _mapSize = 50; Refresh(); });
        Btn("100", new Vector2(0.5f, 0.43f), () => { _mapSize = 100; Refresh(); });
        Btn("200", new Vector2(0.65f, 0.43f), () => { _mapSize = 200; Refresh(); });

        Txt($"Difficulty: {_difficulty:F1}x", 18, 0.35f, new Color(0.7f, 0.7f, 0.7f));
        Btn("Easy", new Vector2(0.35f, 0.28f), () => { _difficulty = 0.5f; Refresh(); });
        Btn("Normal", new Vector2(0.5f, 0.28f), () => { _difficulty = 1f; Refresh(); });
        Btn("Hard", new Vector2(0.65f, 0.28f), () => { _difficulty = 2f; Refresh(); });

        Btn("NEXT →", new Vector2(0.5f, 0.15f), () => { OnComplete?.Invoke(_seed, _mapSize, _difficulty); Destroy(gameObject); });
    }

    void Refresh()
    {
        Destroy(_canvas.gameObject);
        Start();
    }

    void Txt(string msg, int size, float y, Color c)
    {
        var go = new GameObject("__T__"); go.AddComponent<RectTransform>();
        var t = go.AddComponent<Text>();
        t.rectTransform.SetParent(_canvas.transform, false);
        t.rectTransform.anchorMin = t.rectTransform.anchorMax = new Vector2(0.5f, y);
        t.rectTransform.sizeDelta = new Vector2(400, 40);
        t.font = UIFont.Get(); t.fontSize = size; t.alignment = TextAnchor.MiddleCenter; t.color = c; t.text = msg;
    }

    void Btn(string label, Vector2 pos, System.Action onClick)
    {
        var go = new GameObject("__B__"); go.AddComponent<RectTransform>();
        go.AddComponent<Image>().rectTransform.SetParent(_canvas.transform, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = pos;
        rt.sizeDelta = new Vector2(120, 40);
        var btn = go.AddComponent<Button>();
        var cc = btn.colors;
        cc.normalColor = new Color(0.15f, 0.15f, 0.2f, 1f);
        cc.highlightedColor = new Color(0.3f, 0.4f, 0.3f, 1f);
        btn.colors = cc;

        var lg = new GameObject("__L__"); lg.AddComponent<RectTransform>();
        var lt = lg.AddComponent<Text>();
        lt.rectTransform.SetParent(go.transform, false);
        lt.rectTransform.anchorMin = Vector2.zero; lt.rectTransform.anchorMax = Vector2.one;
        lt.font = UIFont.Get(); lt.fontSize = 18; lt.alignment = TextAnchor.MiddleCenter; lt.color = Color.white; lt.text = label;
        btn.onClick.AddListener(() => onClick());
    }
}
