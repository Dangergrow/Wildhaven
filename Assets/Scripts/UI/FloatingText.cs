using UnityEngine;

/// <summary>World-space floating damage/heal text that fades out.</summary>
public class FloatingText : MonoBehaviour
{
    public static void Spawn(Vector3 worldPos, string text, Color color, float duration = 1.5f)
    {
        GameObject go = new GameObject("FloatingText");
        go.transform.position = worldPos + Vector3.up * 2f + Random.insideUnitSphere * 0.3f;
        var tm = go.AddComponent<TextMesh>();
        tm.text = text;
        tm.fontSize = 42;
        tm.color = color;
        tm.characterSize = 0.15f;
        tm.fontStyle = FontStyle.Bold;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        go.AddComponent<FloatingText>()._duration = duration;
    }

    private float _duration = 1.5f;
    private float _elapsed;

    void Update()
    {
        _elapsed += Time.unscaledDeltaTime;
        transform.position += Vector3.up * Time.unscaledDeltaTime * 1.2f;
        TextMesh tm = GetComponent<TextMesh>();
        if (tm != null)
        {
            Color c = tm.color;
            c.a = Mathf.Clamp01(1f - _elapsed / _duration);
            tm.color = c;
        }
        if (_elapsed >= _duration) Destroy(gameObject);
    }
}
