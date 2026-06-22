using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// M key toggles a global map overlay using OnGUI.
/// Shows hex grid with biomes. Hover hex = shows biome/climate info.
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
        if (Keyboard.current != null && Keyboard.current.mKey.wasPressedThisFrame)
            _visible = !_visible;
    }

    HexTile _hoveredTile;
    string _hoverInfo;

    void OnGUI()
    {
        if (!_visible || _worldMap == null || _worldMap.Tiles == null || _worldMap.Tiles.Count == 0) return;

        int mapW = 560, mapH = 440;
        Rect mapRect = new Rect(Screen.width / 2 - mapW / 2, 40, mapW, mapH);
        GUI.Box(mapRect, "WORLD MAP [M] — hover hex for details");

        float hexSize = 6f;
        float cx = mapRect.x + mapW / 2f;
        float cy = mapRect.y + mapH / 2f;

        Vector2 mouse = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
        _hoveredTile = null;
        _hoverInfo = "";

        foreach (var kvp in _worldMap.Tiles)
        {
            HexTile tile = kvp.Value;
            float x = cx + tile.q * hexSize * 1.5f;
            float y = cy - (tile.r * hexSize * 0.87f + tile.q * hexSize * 0.435f);
            x = Mathf.Clamp(x, mapRect.x + 2, mapRect.x + mapW - 10);
            y = Mathf.Clamp(y, mapRect.y + 20, mapRect.y + mapH - 10);

            Color c = tile.biome switch
            {
                MapBiome.Ocean => new Color(0.1f, 0.2f, 0.6f),
                MapBiome.Forest => new Color(0.1f, 0.5f, 0.1f),
                MapBiome.Desert => new Color(0.7f, 0.55f, 0.2f),
                MapBiome.Tundra => new Color(0.7f, 0.75f, 0.8f),
                MapBiome.Plains => new Color(0.4f, 0.6f, 0.2f),
                MapBiome.Swamp => new Color(0.2f, 0.3f, 0.1f),
                MapBiome.Jungle => new Color(0.0f, 0.4f, 0.0f),
                MapBiome.Taiga => new Color(0.1f, 0.35f, 0.2f),
                MapBiome.Savannah => new Color(0.6f, 0.5f, 0.2f),
                MapBiome.Tropics => new Color(0.1f, 0.6f, 0.3f),
                MapBiome.IceWastes => new Color(0.85f, 0.9f, 1f),
                MapBiome.MushroomForest => new Color(0.4f, 0.2f, 0.5f),
                MapBiome.CrystalCaves => new Color(0.6f, 0.4f, 0.9f),
                MapBiome.Deadlands => new Color(0.25f, 0.2f, 0.2f),
                MapBiome.Volcanic => new Color(0.6f, 0.2f, 0.1f),
                _ => Color.gray,
            };

            if (tile.factionId == -999) c = Color.green;
            if (tile.hasSettlement) c = Color.yellow;
            if (tile.factionId >= 0) c = Color.Lerp(c, Color.red, 0.15f); // faction territory tint

            Rect tileRect = new Rect(x - 3, y - 3, 7, 7);

            // Hover detection
            if (mouse.x >= tileRect.x - 2 && mouse.x <= tileRect.x + tileRect.width + 2 &&
                mouse.y >= tileRect.y - 2 && mouse.y <= tileRect.y + tileRect.height + 2)
            {
                _hoveredTile = tile;
                c = Color.white; // highlight hovered hex
                _hoverInfo = $"Biome: {tile.biome}\n" +
                             $"Terrain: {tile.terrain}\n" +
                             $"Temp: {tile.temperature * 100:F0}%  Rain: {tile.rainfall * 100:F0}%\n" +
                             $"Elevation: {tile.elevation * 100:F0}%\n" +
                             (tile.factionId >= 0 ? $"Faction: {tile.factionId}" :
                              tile.factionId == -999 ? "YOUR SETTLEMENT" :
                              tile.hasSettlement ? "NPC Settlement" : "Wilderness") +
                             (tile.hasRoad ? "\nRoad: Yes" : "");
            }

            GUI.color = c;
            GUI.DrawTexture(tileRect, Texture2D.whiteTexture);
        }
        GUI.color = Color.white;

        // Legend
        GUI.Label(new Rect(mapRect.x + 5, mapRect.y + mapH - 40, 540, 20),
            "Green=Your settlement  Yellow=NPC  Blue=Ocean  Red tint=Faction territory");
        GUI.Label(new Rect(mapRect.x + 5, mapRect.y + mapH - 22, 540, 20),
            "White=Forest  Brown=Desert  Cyan=Tundra  Lime=Plains  DarkGreen=Jungle  Grey=Taiga");

        // Hover tooltip
        if (_hoveredTile != null)
        {
            float tipX = mouse.x + 15;
            float tipY = mouse.y - 60;
            if (tipX > Screen.width - 200) tipX = mouse.x - 200;
            GUI.Box(new Rect(tipX, tipY, 190, 100), _hoverInfo);
        }
    }
}
