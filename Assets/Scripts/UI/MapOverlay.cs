using UnityEngine;

/// <summary>
/// M key toggles a simple global map overlay using OnGUI.
/// Shows hex grid with biomes as colored rectangles.
/// </summary>
public class MapOverlay : MonoBehaviour
{
    private bool _visible;
    private WorldMapGenerator _worldMap;

    void Start()
    {
        _worldMap = FindFirstObjectByType<WorldMapGenerator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) _visible = !_visible;
    }

    void OnGUI()
    {
        if (!_visible || _worldMap == null || _worldMap.Tiles.Count == 0) return;

        int mapW = 500, mapH = 400;
        Rect mapRect = new Rect(Screen.width / 2 - mapW / 2, 50, mapW, mapH);
        GUI.Box(mapRect, "WORLD MAP [M]");

        float hexSize = 6f;
        float cx = mapRect.x + mapW / 2f;
        float cy = mapRect.y + mapH / 2f;

        // Draw hex tiles
        foreach (var kvp in _worldMap.Tiles)
        {
            HexTile tile = kvp.Value;
            float x = cx + tile.q * hexSize * 1.5f;
            float y = cy - (tile.r * hexSize * 0.87f + tile.q * hexSize * 0.435f);
            x = Mathf.Clamp(x, mapRect.x + 2, mapRect.x + mapW - 10);
            y = Mathf.Clamp(y, mapRect.y + 20, mapRect.y + mapH - 10);

            Color c = tile.biome switch
            {
                MapBiome.Ocean => new Color(0.1f, 0.2f, 0.5f),
                MapBiome.Forest => new Color(0.1f, 0.5f, 0.1f),
                MapBiome.Desert => new Color(0.7f, 0.6f, 0.3f),
                MapBiome.Tundra => Color.white,
                MapBiome.Plains => new Color(0.4f, 0.6f, 0.2f),
                MapBiome.Swamp => new Color(0.2f, 0.3f, 0.1f),
                MapBiome.Jungle => new Color(0.0f, 0.4f, 0.0f),
                _ => Color.gray,
            };

            if (tile.factionId == -999) c = Color.green; // player settlement
            if (tile.hasSettlement) c = Color.yellow;

            Rect tileRect = new Rect(x - 3, y - 3, 7, 7);
            GUI.color = c;
            GUI.DrawTexture(tileRect, Texture2D.whiteTexture);
        }
        GUI.color = Color.white;

        GUI.Label(new Rect(mapRect.x + 5, mapRect.y + mapH - 20, 300, 20), "Green = your settlement | Yellow = NPC settlements | Blue = ocean");
    }
}
