using UnityEngine;

/// <summary>Zone types for room designation. Attach to a zone marker GameObject.</summary>
public class ZoneMarker : MonoBehaviour
{
    public enum ZoneType { Stockpile, Dump, Farm, Room, Hospital, Prison, Temple, Workshop, DiningRoom, Kitchen, Bedroom, Graveyard }

    public ZoneType zoneType;
    public Vector3Int gridPos;
    public int width = 1, height = 1; // zone size in blocks

    void OnDrawGizmos()
    {
        Gizmos.color = GetZoneColor();
        Gizmos.DrawWireCube(transform.position, new Vector3(width, 0.1f, height));
    }

    Color GetZoneColor() => zoneType switch
    {
        ZoneType.Stockpile => Color.yellow,
        ZoneType.Dump => Color.gray,
        ZoneType.Farm => Color.green,
        ZoneType.Room => Color.blue,
        ZoneType.Hospital => Color.red,
        ZoneType.Prison => new Color(1, 0.5f, 0),
        ZoneType.Temple => Color.magenta,
        ZoneType.Workshop => Color.cyan,
        ZoneType.DiningRoom => new Color(1, 0.8f, 0),
        ZoneType.Kitchen => new Color(1, 0.6f, 0.3f),
        ZoneType.Bedroom => new Color(0.5f, 0.5f, 1),
        ZoneType.Graveyard => Color.black,
        _ => Color.white,
    };
}
