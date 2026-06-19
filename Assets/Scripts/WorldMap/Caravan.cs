using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents a caravan traveling on the global map.
/// Caravans move between hexes, consume supplies, and trigger events.
/// </summary>
public class Caravan : MonoBehaviour
{
    public int caravanId;
    public string caravanName = "Caravan";
    public int ownerFactionId;

    [Header("Position")]
    public HexTile currentHex;
    public HexTile destinationHex;
    public List<HexTile> path = new List<HexTile>();
    public float progress; // 0-1 within current hex

    [Header("Members")]
    public List<int> colonistIds = new List<int>(); // IDs from ColonistSpawner
    public List<int> animalIds = new List<int>();

    [Header("Supplies")]
    public float food;
    public float water;
    public float medicine;

    [Header("Speed")]
    public float baseSpeed = 1f; // hexes per day at 1x
    public float speedMultiplier = 1f;

    [Header("State")]
    public CaravanState state = CaravanState.Idle;
    public float restTimer;

    // Events
    public delegate void CaravanEvent(Caravan caravan, string eventText);
    public event CaravanEvent OnEvent;

    private WorldMapGenerator _worldMap;

    void Awake()
    {
        _worldMap = FindObjectOfType<WorldMapGenerator>();
    }

    void Update()
    {
        if (state != CaravanState.Traveling || destinationHex == null) return;

        float dayProgress = DayCycle.Instance != null ? DayCycle.Instance.DayProgress : 0f;
        float speed = baseSpeed * speedMultiplier * Time.deltaTime * 0.1f; // scale to game time

        progress += speed / currentHex.MovementCost;

        if (progress >= 1f)
        {
            // Arrived at next hex in path
            progress = 0f;
            if (path.Count > 0)
            {
                currentHex = path[0];
                path.RemoveAt(0);

                // Random event chance
                if (Random.value < 0.05f) // 5% per hex
                    TriggerRandomEvent();

                // Consume supplies
                float consumption = 0.1f * colonistIds.Count;
                food -= consumption;
                water -= consumption * 0.5f;

                if (path.Count == 0)
                {
                    state = CaravanState.Arrived;
                    currentHex = destinationHex;
                    OnEvent?.Invoke(this, $"{caravanName} arrived at destination.");
                }
            }
        }
    }

    /// <summary>
    /// Sends caravan to a destination hex. Pathfinding via simple straight-line.
    /// </summary>
    public void TravelTo(HexTile destination)
    {
        if (_worldMap == null) return;
        destinationHex = destination;
        state = CaravanState.Traveling;
        progress = 0f;

        // Simple path: walk hex-by-hex toward destination
        path.Clear();
        int steps = Mathf.Max(Mathf.Abs(destination.q - currentHex.q), Mathf.Abs(destination.r - currentHex.r));
        for (int i = 1; i <= steps; i++)
        {
            int q = currentHex.q + Mathf.RoundToInt((destination.q - currentHex.q) * (i / (float)steps));
            int r = currentHex.r + Mathf.RoundToInt((destination.r - currentHex.r) * (i / (float)steps));
            if (_worldMap.Tiles.TryGetValue((q, r), out HexTile tile) && tile.IsPassable)
                path.Add(tile);
        }
        path.Add(destination);

        OnEvent?.Invoke(this, $"{caravanName} departing. {path.Count} hexes to travel.");
    }

    /// <summary>
    /// Triggers a random event during travel.
    /// </summary>
    void TriggerRandomEvent()
    {
        float roll = Random.value;
        if (roll < 0.3f)
            OnEvent?.Invoke(this, "Ambush! Bandits attack the caravan!");
        else if (roll < 0.5f)
            OnEvent?.Invoke(this, "Found wild berries. +food");
        else if (roll < 0.6f)
            OnEvent?.Invoke(this, "A trader joins the caravan temporarily.");
        else if (roll < 0.7f)
            OnEvent?.Invoke(this, "Heavy rain slows progress.");
        else if (roll < 0.8f)
            OnEvent?.Invoke(this, "A colonist fell sick.");
    }

    /// <summary>
    /// Makes the caravan rest at current hex.
    /// </summary>
    public void Rest(float days)
    {
        state = CaravanState.Resting;
        restTimer = days;
        OnEvent?.Invoke(this, $"{caravanName} resting for {days:F1} days.");
    }

    /// <summary>
    /// Returns estimated travel time to destination in game days.
    /// </summary>
    public float EstimatedTravelTime()
    {
        if (destinationHex == null) return 0f;
        float total = 0f;
        foreach (HexTile hex in path)
            total += hex.MovementCost / (baseSpeed * speedMultiplier);
        return total;
    }
}

public enum CaravanState
{
    Idle,
    Traveling,
    Resting,
    Arrived,
    Ambushed,
}
