using UnityEngine;

/// <summary>
/// Provides a reliable font for UI. Tries built-in fonts, falls back to system font.
/// </summary>
public static class UIFont
{
    private static Font _font;

    public static Font Get()
    {
        if (_font != null) return _font;

        // Try multiple built-in font names (varies by Unity version)
        string[] names = { "LegacyRuntime.ttf", "Arial.ttf", "LiberationSans.ttf" };
        foreach (string name in names)
        {
            _font = Resources.GetBuiltinResource<Font>(name);
            if (_font != null) return _font;
        }

        // Fallback: create from system font
        _font = Font.CreateDynamicFontFromOSFont("Arial", 14);
        return _font;
    }
}
