using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages recreation for all colonists.
/// Tracks what each colonist did recently and enforces variety.
/// </summary>
public class RecreationManager : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("How often (seconds) colonist checks for recreation")]
    public float checkInterval = 5f;

    [Tooltip("Recreation threshold — below this, colonist seeks fun")]
    public float recreationThreshold = 30f;

    [Tooltip("How many past activities to track for variety penalty")]
    public int trackedActivities = 5;

    [Header("Available Activities")]
    public RecreationActivity[] allActivities;

    // Per-colonist history of recent recreation
    private Dictionary<Colonist, List<RecreationType>> _recentRecreation
        = new Dictionary<Colonist, List<RecreationType>>();

    private ColonistSpawner _spawner;
    private float _timer;

    private void Awake()
    {
        _spawner = GetComponent<ColonistSpawner>();

        // Default activities if none assigned
        if (allActivities == null || allActivities.Length == 0)
        {
            allActivities = new RecreationActivity[]
            {
                new RecreationActivity { activityName = "Chatting", type = RecreationType.Social, recreationValue = 25f, moodBonus = 5f, duration = 15f },
                new RecreationActivity { activityName = "Meditation", type = RecreationType.Solitary, recreationValue = 30f, moodBonus = 3f, duration = 20f },
                new RecreationActivity { activityName = "Horseshoes", type = RecreationType.Physical, recreationValue = 20f, moodBonus = 8f, duration = 12f, requiresObject = true, requiredObjectName = "HorseshoePost" },
                new RecreationActivity { activityName = "Chess", type = RecreationType.Intellectual, recreationValue = 25f, moodBonus = 5f, duration = 25f, requiresObject = true, requiredObjectName = "ChessTable" },
                new RecreationActivity { activityName = "Reading", type = RecreationType.Intellectual, recreationValue = 20f, moodBonus = 2f, duration = 20f },
                new RecreationActivity { activityName = "Music", type = RecreationType.Music, recreationValue = 30f, moodBonus = 10f, duration = 15f, requiresObject = true, requiredObjectName = "Instrument" },
                new RecreationActivity { activityName = "Stargazing", type = RecreationType.Solitary, recreationValue = 15f, moodBonus = 4f, duration = 10f },
                new RecreationActivity { activityName = "Dancing", type = RecreationType.Physical, recreationValue = 25f, moodBonus = 12f, duration = 10f },
                new RecreationActivity { activityName = "Card Games", type = RecreationType.Social, recreationValue = 20f, moodBonus = 6f, duration = 15f },
                new RecreationActivity { activityName = "Hot Bath", type = RecreationType.Relaxation, recreationValue = 35f, moodBonus = 10f, duration = 20f, requiresObject = true, requiredObjectName = "Bathtub" },
            };
        }
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < checkInterval) return;
        _timer = 0f;

        if (_spawner == null) return;
        foreach (Colonist colonist in _spawner.Colonists)
        {
            if (colonist == null || colonist.currentState == ColonistState.Dead) continue;
            CheckRecreationNeed(colonist);
        }
    }

    /// <summary>
    /// Checks if a colonist needs recreation and finds a suitable activity.
    /// </summary>
    private void CheckRecreationNeed(Colonist colonist)
    {
        if (colonist.recreation > recreationThreshold) return;
        if (colonist.currentState == ColonistState.Sleeping ||
            colonist.currentState == ColonistState.Fighting) return;

        RecreationActivity activity = PickActivity(colonist);
        if (activity.activityName == null) return;

        // Apply recreation
        NeedsSystem needs = colonist.GetComponent<NeedsSystem>();
        if (needs != null)
        {
            float value = activity.recreationValue;
            // Variety penalty: if colonist did same type recently
            if (HasRecentType(colonist, activity.type))
                value *= 0.5f;

            needs.Recreate(value);
            if (activity.moodBonus > 0f)
                colonist.ModifyMood(activity.moodBonus);
        }

        // Track in history
        TrackActivity(colonist, activity.type);

        // Set colonist state
        ColonistAI ai = colonist.GetComponent<ColonistAI>();
        if (ai != null)
        {
            ai.currentTask = ColonistTask.None;
            colonist.currentState = ColonistState.Recreation;
        }
    }

    /// <summary>
    /// Picks the best recreation activity for a colonist based on variety and availability.
    /// </summary>
    private RecreationActivity PickActivity(Colonist colonist)
    {
        // Prefer activities colonist hasn't done recently
        RecreationActivity bestActivity = default;
        float bestScore = -1f;

        foreach (RecreationActivity act in allActivities)
        {
            if (act.requiresObject && !IsObjectAvailable(act.requiredObjectName))
                continue;

            float score = act.recreationValue + act.moodBonus;
            if (HasRecentType(colonist, act.type))
                score *= 0.3f; // strongly penalize repeated types

            if (score > bestScore)
            {
                bestScore = score;
                bestActivity = act;
            }
        }

        return bestActivity;
    }

    /// <summary>
    /// Checks if a recreation object exists in the world.
    /// </summary>
    private bool IsObjectAvailable(string objectName)
    {
        // Check if colony has any Room zone (indicates built infrastructure)
        ZoneMarker[] zones = FindObjectsByType<ZoneMarker>(FindObjectsSortMode.None);
        foreach (ZoneMarker z in zones)
        {
            if (z != null && z.zoneType == ZoneMarker.ZoneType.Room)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if the colonist did this recreation type recently.
    /// </summary>
    private bool HasRecentType(Colonist colonist, RecreationType type)
    {
        if (!_recentRecreation.TryGetValue(colonist, out var history))
            return false;
        return history.Contains(type);
    }

    /// <summary>
    /// Records a recreation activity in the colonist's history.
    /// </summary>
    private void TrackActivity(Colonist colonist, RecreationType type)
    {
        if (!_recentRecreation.ContainsKey(colonist))
            _recentRecreation[colonist] = new List<RecreationType>();

        var history = _recentRecreation[colonist];
        history.Add(type);
        while (history.Count > trackedActivities)
            history.RemoveAt(0);
    }
}
