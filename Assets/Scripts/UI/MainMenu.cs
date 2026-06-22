using UnityEngine;
using UnityEngine.UI;

/// <summary>Simple menu overlay. Does NOT pause or disable anything.</summary>
public class MainMenu : MonoBehaviour
{
    private Canvas _canvas;

    void Start()
    {
        // Auto-skip menu in editor for testing
        #if UNITY_EDITOR
        StartGame();
        return;
        #endif
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
        bg.color = new Color(0.03f, 0.03f, 0.06f, 0.9f);

        Txt("WILDHAVEN", 46, 0.75f, Color.white);
        Txt("Colony Simulator", 18, 0.69f, new Color(0.6f, 0.6f, 0.6f));

        Btn("New Game", 0.55f, () => { DeleteSave(); StartGame(); });
        Btn("Continue", 0.45f, () => StartGame());
        Btn("Quit", 0.35f, () => Application.Quit());
    }

    void DeleteSave()
    {
        var p = System.IO.Path.Combine(Application.persistentDataPath, "game.sav");
        if (System.IO.File.Exists(p)) System.IO.File.Delete(p);
    }

    void StartGame()
    {
        Destroy(_canvas.gameObject);
        Destroy(this);
    }

    void Txt(string msg, int size, float y, Color c)
    {
        var go = new GameObject("__T__"); go.AddComponent<RectTransform>();
        var t = go.AddComponent<Text>();
        t.rectTransform.SetParent(_canvas.transform, false);
        t.rectTransform.anchorMin = t.rectTransform.anchorMax = new Vector2(0.5f, y);
        t.rectTransform.sizeDelta = new Vector2(400, 50);
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = size; t.alignment = TextAnchor.MiddleCenter; t.color = c; t.text = msg;
    }

    void Btn(string label, float y, System.Action onClick)
    {
        var go = new GameObject("__B__"); go.AddComponent<RectTransform>();
        go.AddComponent<Image>().rectTransform.SetParent(_canvas.transform, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, y);
        rt.sizeDelta = new Vector2(220, 50);
        var btn = go.AddComponent<Button>();
        var c = btn.colors;
        c.normalColor = new Color(0.15f, 0.15f, 0.2f, 1f);
        c.highlightedColor = new Color(0.3f, 0.4f, 0.3f, 1f);
        btn.colors = c;

        var lg = new GameObject("__L__"); lg.AddComponent<RectTransform>();
        var lt = lg.AddComponent<Text>();
        lt.rectTransform.SetParent(go.transform, false);
        lt.rectTransform.anchorMin = Vector2.zero; lt.rectTransform.anchorMax = Vector2.one;
        lt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        lt.fontSize = 22; lt.alignment = TextAnchor.MiddleCenter; lt.color = Color.white; lt.text = label;

        btn.onClick.AddListener(() => onClick());
    }
}
