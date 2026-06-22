using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/// <summary>Work priority panel — set 1-4 priority per colonist job. F2 toggles.</summary>
public class WorkPanel : MonoBehaviour
{
    private Canvas _canvas;
    private bool _visible;
    private ColonistSpawner _spawner;
    private int _colIdx;
    private ColonistAI _currentAI;
    private Button[,] _priButtons = new Button[14, 4]; // [job, priority-1]
    private string[] _jobNames = { "Build", "Mine", "Cook", "Research", "Medicine", "Melee", "Ranged", "Craft", "Farm", "Social", "Animals", "Hunt", "Trade", "Art" };

    void Start()
    {
        _spawner = FindObjectOfType<ColonistSpawner>();
        var go = new GameObject("WorkPanelCanvas");
        _canvas = go.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 50;
        go.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        go.AddComponent<GraphicRaycaster>();
        BuildUI();
        _canvas.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current == null) return;
        if (Keyboard.current.f2Key.wasPressedThisFrame) Toggle();
        if (Keyboard.current.escapeKey.wasPressedThisFrame && _visible) Toggle();
        if (_visible && _spawner != null && _spawner.Colonists.Count > 0)
        {
            if (Keyboard.current.numpadPlusKey.wasPressedThisFrame || Keyboard.current.equalsKey.wasPressedThisFrame)
            { _colIdx = (_colIdx + 1) % _spawner.Colonists.Count; SelectCol(); }
            if (Keyboard.current.numpadMinusKey.wasPressedThisFrame || Keyboard.current.minusKey.wasPressedThisFrame)
            { _colIdx = (_colIdx + _spawner.Colonists.Count - 1) % _spawner.Colonists.Count; SelectCol(); }
        }
    }

    void Toggle() { _visible = !_visible; _canvas.gameObject.SetActive(_visible); if (_visible) SelectCol(); }

    void SelectCol()
    {
        if (_spawner == null || _spawner.Colonists.Count == 0) return;
        int idx = Mathf.Clamp(_colIdx, 0, _spawner.Colonists.Count - 1);
        Colonist c = _spawner.Colonists[idx];
        if (c == null) return;
        _currentAI = c.GetComponent<ColonistAI>();
        Refresh();
    }

    void BuildUI()
    {
        // Background
        var bgGo = new GameObject("Bg"); bgGo.AddComponent<RectTransform>();
        var bg = bgGo.AddComponent<Image>();
        bg.transform.SetParent(_canvas.transform);
        bg.rectTransform.anchorMin = Vector2.zero; bg.rectTransform.anchorMax = Vector2.one;
        bg.color = new Color(0.08f, 0.08f, 0.1f, 0.92f);

        // Title + colonist name
        var titleGo = new GameObject("Title"); titleGo.AddComponent<RectTransform>();
        var title = titleGo.AddComponent<Text>();
        title.transform.SetParent(_canvas.transform);
        title.rectTransform.anchorMin = title.rectTransform.anchorMax = new Vector2(0.5f, 0.95f);
        title.rectTransform.sizeDelta = new Vector2(500, 30);
        title.font = UIFont.Get(); title.fontSize = 22; title.alignment = TextAnchor.MiddleCenter; title.color = Color.white;
        title.text = "Work Priorities (F2 close, +/- change colonist)";

        // Job rows with 1-4 buttons that HIGHLIGHT selected priority
        for (int job = 0; job < 14; job++)
        {
            float y = 0.85f - job * 0.053f;

            // Job label
            var lblGo = new GameObject($"Job{job}"); lblGo.AddComponent<RectTransform>();
            var lbl = lblGo.AddComponent<Text>();
            lbl.transform.SetParent(_canvas.transform);
            lbl.rectTransform.anchorMin = lbl.rectTransform.anchorMax = new Vector2(0.08f, y);
            lbl.rectTransform.sizeDelta = new Vector2(120, 22);
            lbl.font = UIFont.Get(); lbl.fontSize = 13; lbl.alignment = TextAnchor.MiddleLeft; lbl.color = Color.white;
            lbl.text = _jobNames[job];

            // Priority buttons 1-4
            for (int p = 0; p < 4; p++)
            {
                int jobIdx = job, pri = p + 1;
                var btnGo = new GameObject($"Pri_{job}_{pri}"); btnGo.AddComponent<RectTransform>();
                var btn = btnGo.AddComponent<Button>();
                btn.transform.SetParent(_canvas.transform);
                var rt = btn.GetComponent<RectTransform>();
                rt.anchorMin = rt.anchorMax = new Vector2(0.42f + p * 0.08f, y);
                rt.sizeDelta = new Vector2(36, 22);

                var tGo = new GameObject("T"); tGo.AddComponent<RectTransform>();
                var t = tGo.AddComponent<Text>();
                t.transform.SetParent(btn.transform);
                t.rectTransform.anchorMin = t.rectTransform.anchorMax = Vector2.one * 0.5f;
                t.rectTransform.sizeDelta = new Vector2(36, 22);
                t.font = UIFont.Get(); t.fontSize = 12; t.alignment = TextAnchor.MiddleCenter; t.color = Color.white;
                t.text = pri.ToString();

                btn.onClick.AddListener(() => {
                    if (_currentAI != null) { _currentAI.jobPriorities[jobIdx] = pri; Refresh(); }
                });

                _priButtons[job, p] = btn;
            }
        }
    }

    void Refresh()
    {
        if (_currentAI == null) return;
        // Highlight selected priority button for each job
        for (int job = 0; job < 14; job++)
        {
            int currentPri = _currentAI.jobPriorities[job];
            for (int p = 0; p < 4; p++)
            {
                Button btn = _priButtons[job, p];
                if (btn == null) continue;
                var c = btn.colors;
                c.normalColor = (p + 1 == currentPri)
                    ? new Color(0.3f, 0.5f, 0.3f, 1f)  // green = selected
                    : new Color(0.2f, 0.2f, 0.2f, 0.8f); // dark = not selected
                btn.colors = c;
            }
        }
    }
}
