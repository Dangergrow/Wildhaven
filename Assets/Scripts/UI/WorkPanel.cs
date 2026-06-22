using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;

/// <summary>Work priority panel — set 1-4 priority for each colonist's job types.</summary>
public class WorkPanel : MonoBehaviour
{
    private Canvas _canvas;
    private bool _visible;
    private ColonistSpawner _spawner;
    private string[] _jobNames = { "Build", "Mine", "Cook", "Research", "Medicine", "Melee", "Ranged", "Craft", "Farm", "Social", "Animals", "Hunt", "Trade", "Art" };
    private Colonist _currentColonist;
    private int _colIdx;

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
        // Numpad +/- to switch colonist
        if (_visible)
        {
            if (Keyboard.current.numpadPlusKey.wasPressedThisFrame || Keyboard.current.equalsKey.wasPressedThisFrame)
            { if (_spawner != null && _spawner.Colonists.Count > 0) { _colIdx = (_colIdx + 1) % _spawner.Colonists.Count; SelectCol(); } }
            if (Keyboard.current.numpadMinusKey.wasPressedThisFrame || Keyboard.current.minusKey.wasPressedThisFrame)
            { if (_spawner != null && _spawner.Colonists.Count > 0) { _colIdx = (_colIdx + _spawner.Colonists.Count - 1) % _spawner.Colonists.Count; SelectCol(); } }
        }
    }

    void Toggle() { _visible = !_visible; _canvas.gameObject.SetActive(_visible); if (_visible) SelectCol(); }

    void SelectCol()
    {
        if (_spawner == null || _spawner.Colonists.Count == 0) return;
        _currentColonist = _spawner.Colonists[Mathf.Clamp(_colIdx, 0, _spawner.Colonists.Count - 1)];
        Refresh();
    }

    void BuildUI()
    {
        var bgGo = new GameObject("Bg");
        bgGo.AddComponent<RectTransform>();
        var bg = bgGo.AddComponent<Image>();
        bg.transform.SetParent(_canvas.transform);
        bg.rectTransform.anchorMin = Vector2.zero; bg.rectTransform.anchorMax = Vector2.one;
        bg.color = new Color(0.08f, 0.08f, 0.1f, 0.92f);

        // Title
        var titleGo = new GameObject("Title");
        titleGo.AddComponent<RectTransform>();
        var title = titleGo.AddComponent<Text>();
        title.transform.SetParent(_canvas.transform);
        title.rectTransform.anchorMin = title.rectTransform.anchorMax = new Vector2(0.5f, 0.95f);
        title.rectTransform.sizeDelta = new Vector2(400, 30);
        title.font = UIFont.Get();
        title.fontSize = 22; title.alignment = TextAnchor.MiddleCenter; title.color = Color.white;
        title.text = "Work Priorities (F2 to close)";

        // Job rows with 1-4 buttons
        for (int i = 0; i < 14; i++)
        {
            int idx = i;
            float y = 0.85f - i * 0.055f;
            var labelGo = new GameObject($"Job{i}");
            labelGo.AddComponent<RectTransform>();
            var label = labelGo.AddComponent<Text>();
            label.transform.SetParent(_canvas.transform);
            label.rectTransform.anchorMin = label.rectTransform.anchorMax = new Vector2(0.1f, y);
            label.rectTransform.sizeDelta = new Vector2(120, 20);
            label.font = UIFont.Get();
            label.fontSize = 13; label.alignment = TextAnchor.MiddleLeft; label.color = Color.white;
            label.text = _jobNames[i];

            for (int p = 1; p <= 4; p++)
            {
                int pri = p;
                AddBtn($"{p}", new Vector2(0.45f + p * 0.08f, y), new Vector2(30, 20), () =>
                {
                    if (_currentColonist != null)
                    {
                        var ai = _currentColonist.GetComponent<ColonistAI>();
                        if (ai != null && idx < ai.jobPriorities.Length) ai.jobPriorities[idx] = pri;
                        Refresh();
                    }
                });
            }
        }
    }

    void Refresh()
    {
        // Update button highlights based on current priorities
        var ai = _currentColonist != null ? _currentColonist.GetComponent<ColonistAI>() : null;
        // Simple text update — just redraw
        if (_canvas != null) _canvas.gameObject.SetActive(_canvas.gameObject.activeSelf); // force refresh
    }

    void AddBtn(string label, Vector2 pos, Vector2 size, System.Action onClick)
    {
        var btnGo = new GameObject($"Btn_{label}_{pos.x}_{pos.y}");
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
        txt.fontSize = 12; txt.alignment = TextAnchor.MiddleCenter; txt.color = Color.white; txt.text = label;
        btn.onClick.AddListener(() => onClick());
    }
}
