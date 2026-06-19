using UnityEngine;
using UnityEngine.UI;

/// <summary>Creates Canvas HUD at runtime — time, colonists, block type, mode.</summary>
public class CanvasHUD : MonoBehaviour
{
    private DayCycle _day;
    private ColonistSpawner _spawner;
    private BuildManager _build;
    private SelectionManager _select;
    private Text _timeText, _colonistText, _blockText, _modeText;

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

        var panel = new GameObject("Panel").AddComponent<Image>();
        panel.transform.SetParent(canvas.transform);
        panel.rectTransform.anchorMin = Vector2.zero;
        panel.rectTransform.anchorMax = Vector2.one;
        panel.color = new Color(0, 0, 0, 0);

        _timeText = CreateText("TimeText", canvas.transform, new Vector2(0.5f, 0.97f), 18, TextAnchor.UpperCenter);
        _colonistText = CreateText("ColonistText", canvas.transform, new Vector2(0.01f, 0.96f), 14, TextAnchor.UpperLeft);
        _blockText = CreateText("BlockText", canvas.transform, new Vector2(0.99f, 0.96f), 14, TextAnchor.UpperRight);
        _modeText = CreateText("ModeText", canvas.transform, new Vector2(0.01f, 0.02f), 14, TextAnchor.LowerLeft);
    }

    Text CreateText(string name, Transform parent, Vector2 anchor, int size, TextAnchor align)
    {
        var go = new GameObject(name).AddComponent<Text>();
        go.transform.SetParent(parent);
        go.rectTransform.anchorMin = anchor; go.rectTransform.anchorMax = anchor;
        go.rectTransform.pivot = anchor; go.rectTransform.sizeDelta = new Vector2(400, 30);
        go.rectTransform.anchoredPosition = Vector2.zero;
        go.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        go.fontSize = size; go.alignment = align;
        go.color = Color.white;
        return go;
    }

    void Update()
    {
        if (_day != null)
            _timeText.text = _day.IsPaused ? "PAUSED" : $"Day {_day.day}  {_day.hour:D2}:{_day.minute:D2}  {_day.gameSpeed}x";

        if (_spawner != null)
        {
            int count = 0, alive = 0;
            foreach (var c in _spawner.colonists)
            {
                count++;
                if (c.currentState != ColonistState.Dead) alive++;
            }
            _colonistText.text = $"Colonists: {alive}/{count}";
        }

        if (_build != null)
            _blockText.text = $"Block: {_build.SelectedType}  [1-9]";

        if (_select != null)
            _modeText.text = $"B = Build/Select  |  F5 Save  F9 Load  |  Space Pause  1/2/3 Speed";
    }
}
