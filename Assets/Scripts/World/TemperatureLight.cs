using UnityEngine;

/// <summary>Manages temperature and light levels for colonists. Affects mood and comfort.</summary>
public class TemperatureLight : MonoBehaviour
{
    private GridManager _grid;
    public float baseTemp = 20f;
    public float outdoorLight = 1f;
    public float indoorLight = 0.3f;

    void Awake() { _grid = GetComponent<GridManager>(); if (_grid == null) _grid = FindObjectOfType<GridManager>(); }

    /// <summary>Get temperature at a position. Enclosed rooms = warmer, exposed = colder.</summary>
    public float GetTemperature(Vector3 pos)
    {
        if (_grid == null) return baseTemp;
        Vector3Int p = _grid.WorldToGrid(pos);
        // Count solid walls around — more walls = more insulation
        int walls = 0;
        bool hasCeiling = _grid.GetBlock(p.x, p.y + 1, p.z) != BlockType.Air;
        Vector3Int[] dirs = { new(1,0,0), new(-1,0,0), new(0,0,1), new(0,0,-1) };
        foreach (var d in dirs)
            if (_grid.GetBlock(p.x + d.x, p.y + d.y, p.z + d.z) != BlockType.Air) walls++;
        // Underground bonus
        bool underground = p.y < _grid.Height / 3;

        float temp = baseTemp;
        if (underground) temp += 5f;
        if (hasCeiling) temp += 3f;
        temp += walls * 2f; // each wall adds 2 degrees
        // Height-based cooling
        temp -= p.y * 0.3f;
        return Mathf.Clamp(temp, -10f, 50f);
    }

    /// <summary>Get light level at a position. 0=dark, 1=bright.</summary>
    public float GetLightLevel(Vector3 pos)
    {
        if (_grid == null) return outdoorLight;
        Vector3Int p = _grid.WorldToGrid(pos);

        // Check powered lights nearby
        EnergyNetwork en = FindObjectOfType<EnergyNetwork>();
        if (en == null && _grid != null) en = _grid.gameObject.AddComponent<EnergyNetwork>();
        if (en != null)
        {
            // Search radius 5 for powered lamps
            for (int dx = -5; dx <= 5; dx++)
            for (int dy = -5; dy <= 5; dy++)
            for (int dz = -5; dz <= 5; dz++)
            {
                Vector3Int lp = new(p.x + dx, p.y + dy, p.z + dz);
                int radius = en.GetLightRadius(lp);
                if (radius > 0 && Mathf.Abs(dx) + Mathf.Abs(dy) + Mathf.Abs(dz) <= radius)
                    return 1f; // powered light
            }
        }
        // Count how many sides are open to sky
        int openSides = 0;
        if (_grid.GetBlock(p.x + 1, p.y, p.z) == BlockType.Air) openSides++;
        if (_grid.GetBlock(p.x - 1, p.y, p.z) == BlockType.Air) openSides++;
        if (_grid.GetBlock(p.x, p.y, p.z + 1) == BlockType.Air) openSides++;
        if (_grid.GetBlock(p.x, p.y, p.z - 1) == BlockType.Air) openSides++;
        bool hasCeiling = _grid.GetBlock(p.x, p.y + 1, p.z) != BlockType.Air;

        // Underground = very dark without torches
        if (p.y < _grid.Height / 3 && hasCeiling) return 0.1f;
        if (hasCeiling) return indoorLight;
        return Mathf.Lerp(indoorLight, outdoorLight, openSides / 4f);
    }

    /// <summary>Get penalty label for colonists.</summary>
    public static string GetTempLabel(float t) => t switch
    {
        < 0 => "Freezing!", < 10 => "Cold", > 40 => "Scorching!", > 30 => "Hot", _ => "Comfortable"
    };

    public static string GetLightLabel(float l) => l switch
    {
        < 0.2f => "Pitch black", < 0.4f => "Dark", < 0.6f => "Dim", _ => "Bright"
    };
}
