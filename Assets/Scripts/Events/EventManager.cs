using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages random events, triggers them periodically, and applies their effects.
/// </summary>
public class EventManager : MonoBehaviour
{
    [Header("Timing")]
    public float checkInterval = 60f; // seconds at 1x
    public float eventChance = 0.3f; // chance per check

    [Header("Event Definitions")]
    public GameEventDef[] eventDefs;

    [Header("State")]
    public int currentDay;
    public List<ActiveEvent> activeEvents = new List<ActiveEvent>();
    public List<EventType> pastEvents = new List<EventType>();

    private float _timer;
    private DayCycle _dayCycle;
    private RaidManager _raidManager;
    private ColonistSpawner _colonistSpawner;

    void Awake()
    {
        _dayCycle = FindObjectOfType<DayCycle>();
        _raidManager = FindObjectOfType<RaidManager>();
        _colonistSpawner = FindObjectOfType<ColonistSpawner>();

        if (eventDefs == null || eventDefs.Length == 0)
        {
            eventDefs = new GameEventDef[]
            {
                new GameEventDef { type = EventType.WandererJoins, title = "Странник", description = "Путник хочет присоединиться к колонии.", minDay = 2, weight = 2f },
                new GameEventDef { type = EventType.TradeCaravan, title = "Торговый караван", description = "Прибыл караван торговцев.", minDay = 3, weight = 2f },
                new GameEventDef { type = EventType.CropBoom, title = "Бум урожая", description = "Внезапно созрел весь урожай!", minDay = 5, weight = 1.5f },
                new GameEventDef { type = EventType.Plague, title = "Чума", description = "Болезнь поразила колонию!", minDay = 4, weight = 1f },
                new GameEventDef { type = EventType.Blight, title = "Гниль", description = "Грибок уничтожает запасы еды.", minDay = 3, weight = 1f },
                new GameEventDef { type = EventType.Drought, title = "Засуха", description = "Растения не растут без воды.", minDay = 5, weight = 1f },
                new GameEventDef { type = EventType.SolarFlare, title = "Солнечная вспышка", description = "Электричество выключено.", minDay = 10, weight = 0.5f },
                new GameEventDef { type = EventType.PsychicWave, title = "Пси-волна", description = "У всех колонистов упало настроение.", minDay = 8, weight = 0.8f },
                new GameEventDef { type = EventType.ManhunterPack, title = "Стая хищников", description = "Агрессивные животные атакуют!", minDay = 6, weight = 1f },
                new GameEventDef { type = EventType.HeatWave, title = "Жара", description = "Температура повышена несколько дней.", minDay = 3, weight = 1.5f },
                new GameEventDef { type = EventType.ColdSnap, title = "Похолодание", description = "Температура понижена несколько дней.", minDay = 3, weight = 1.5f },
            };
        }
    }

    void Update()
    {
        if (_dayCycle == null || _dayCycle.IsPaused) return;

        _timer += Time.deltaTime * _dayCycle.gameSpeed;
        if (_timer >= checkInterval)
        {
            _timer = 0f;
            CheckEvents();
        }

        // Update active events
        for (int i = activeEvents.Count - 1; i >= 0; i--)
        {
            activeEvents[i].remainingTime -= Time.deltaTime * _dayCycle.gameSpeed;
            if (activeEvents[i].remainingTime <= 0f)
                EndEvent(i);
        }
    }

    /// <summary>
    /// Checks if a random event should trigger.
    /// </summary>
    void CheckEvents()
    {
        if (_dayCycle == null) return;
        currentDay = _dayCycle.day;

        if (Random.value > eventChance) return;

        // Pick a random eligible event
        List<GameEventDef> eligible = new List<GameEventDef>();
        foreach (GameEventDef def in eventDefs)
        {
            if (def.minDay <= currentDay && (def.repeats || !pastEvents.Contains(def.type)))
                eligible.Add(def);
        }

        if (eligible.Count == 0) return;

        GameEventDef chosen = eligible[Random.Range(0, eligible.Count)];
        TriggerEvent(chosen);
    }

    /// <summary>
    /// Triggers a specific event and applies its effects.
    /// </summary>
    public void TriggerEvent(GameEventDef def)
    {
        Debug.Log($"[EventManager] Event: {def.title} — {def.description}");

        switch (def.type)
        {
            case EventType.WandererJoins:
                if (_colonistSpawner != null)
                {
                    Vector3 pos = _colonistSpawner.transform.position + Random.insideUnitSphere * 3f;
                    _colonistSpawner.SpawnColonist(pos + Vector3.up * 15f);
                }
                break;

            case EventType.TradeCaravan:
                // Mark upcoming trade
                break;

            case EventType.Plague:
                if (_colonistSpawner != null)
                {
                    foreach (Colonist c in _colonistSpawner.Colonists)
                        if (Random.value < 0.5f) c.TakeDamage(10f);
                }
                break;

            case EventType.Raid:
                if (_raidManager != null) _raidManager.StartRaid();
                break;

            case EventType.ManhunterPack:
                // Spawn animals (delegated to RaidManager as special raid)
                if (_raidManager != null) _raidManager.StartRaid();
                break;

            case EventType.HeatWave:
                foreach (Colonist c in _colonistSpawner.Colonists)
                    c.ModifyMood(-10f);
                break;

            case EventType.ColdSnap:
                foreach (Colonist c in _colonistSpawner.Colonists)
                    c.ModifyMood(-5f);
                break;

            case EventType.PsychicWave:
                foreach (Colonist c in _colonistSpawner.Colonists)
                    c.ModifyMood(-20f);
                break;

            case EventType.CropBoom:
                foreach (Colonist c in _colonistSpawner.Colonists)
                    c.ModifyMood(10f);
                break;
        }

        if (def.duration > 0f)
        {
            activeEvents.Add(new ActiveEvent { type = def.type, remainingTime = def.duration });
        }

        pastEvents.Add(def.type);
    }

    void EndEvent(int index)
    {
        Debug.Log($"[EventManager] Event ended: {activeEvents[index].type}");
        activeEvents.RemoveAt(index);
    }
}

/// <summary>
/// Tracks an active event with remaining duration.
/// </summary>
[System.Serializable]
public class ActiveEvent
{
    public EventType type;
    public float remainingTime;
}
