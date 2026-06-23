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

        string[] names = { "LegacyRuntime.ttf", "Arial.ttf", "LiberationSans.ttf" };
        foreach (string name in names)
        {
            _font = Resources.GetBuiltinResource<Font>(name);
            if (_font != null) return _font;
        }

        // Fallback: try multiple system fonts at readable size
        string[] osFonts = { "Calibri", "Segoe UI", "Arial", "Tahoma" };
        foreach (string f in osFonts)
        {
            _font = Font.CreateDynamicFontFromOSFont(f, 16);
            if (_font != null) return _font;
        }

        _font = Font.CreateDynamicFontFromOSFont("Arial", 16);
        return _font;
    }
}
