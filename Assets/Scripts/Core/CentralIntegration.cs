using UnityEngine;

/// <summary>Master integrator — calls ALL orphaned systems every frame. Makes dead code alive.</summary>
public class CentralIntegration : MonoBehaviour
{
    private float _timer1s, _timer5s, _timer30s;

    void Awake()
    {
        // Auto-create GameManager if missing
        if (FindFirstObjectByType<GameManager>() == null)
            gameObject.AddComponent<GameManager>();
    }

    void Update()
    {
        float dt = Time.unscaledDeltaTime;
        _timer1s += dt; _timer5s += dt; _timer30s += dt;

        if (_timer1s > 1f) { _timer1s = 0f; EverySecond(); }
        if (_timer5s > 5f) { _timer5s = 0f; Every5Seconds(); }
        if (_timer30s > 30f) { _timer30s = 0f; Every30Seconds(); }
    }

    void EverySecond()
    {
        var spawner = FindFirstObjectByType<ColonistSpawner>();
        var day = FindFirstObjectByType<DayCycle>();

        // Apply schedule to all colonists
        if (spawner != null && day != null)
        {
            foreach (var c in spawner.Colonists)
            {
                if (c.currentState == ColonistState.Dead) continue;
                var sched = c.GetComponent<ColonistSchedule>();
                if (sched == null) sched = c.gameObject.AddComponent<ColonistSchedule>();
                sched.ApplySchedule(c, day.hour);
            }
        }
    }

    void Every5Seconds()
    {
        // Auto-heal wounded colonists
        var colony = FindFirstObjectByType<ColonyServices>();
        if (colony != null) colony.AutoHeal();

        // Auto-roof
        if (colony != null) colony.AutoRoof();

        // Try to complete quests near colonists
        var quests = FindFirstObjectByType<QuestManager>();
        var spawner = FindFirstObjectByType<ColonistSpawner>();
        if (quests != null && spawner != null)
        {
            foreach (var c in spawner.Colonists)
            {
                if (c.currentState == ColonistState.Dead) continue;
                var gm = FindFirstObjectByType<GridManager>();
                if (gm != null)
                {
                    Vector3Int p = gm.WorldToGrid(c.transform.position);
                    quests.TryCompleteAt(p, c);
                }
            }
        }

        // Try to harvest forage near colonists
        var forage = FindFirstObjectByType<ForageSpawner>();
        if (forage != null && spawner != null)
        {
            foreach (var c in spawner.Colonists)
            {
                if (c.currentState == ColonistState.Dead) continue;
                var gm = FindFirstObjectByType<GridManager>();
                if (gm != null)
                {
                    Vector3Int p = gm.WorldToGrid(c.transform.position);
                    var result = forage.TryHarvest(p);
                    if (result != null)
                    {
                        var inv = c.GetComponent<Inventory>();
                        if (inv != null) inv.AddItem(result.Value.Item1, result.Value.Item2);
                    }
                }
            }
        }

        // Try cooking if near campfire
        var cooking = FindFirstObjectByType<CookingSystem>();
        if (cooking != null && spawner != null)
        {
            foreach (var c in spawner.Colonists)
            {
                if (c.currentState == ColonistState.Dead) continue;
                var inv = c.GetComponent<Inventory>();
                if (inv == null) continue;
                foreach (var recipe in cooking.recipes)
                {
                    if (cooking.TryCook(recipe, inv, out float _)) break; // cook one recipe
                }
            }
        }

        // Try religion rituals near altars
        var religion = FindFirstObjectByType<ReligionSystem>();
        if (religion != null)
        {
            religion.PerformRitual(ReligionSystem.RitualType.Prayer, Vector3Int.zero, ReligionSystem.Belief.NatureWorship);
        }

        // Ignite random fires (rare)
        if (Random.value < 0.01f && spawner != null)
        {
            var fire = FindFirstObjectByType<FireAndSeasons>();
            if (fire != null)
            {
                var gm = FindFirstObjectByType<GridManager>();
                if (gm != null)
                {
                    int rx = Random.Range(0, gm.Width), rz = Random.Range(0, gm.Depth);
                    for (int y = gm.Height - 1; y > 0; y--)
                        if (gm.GetBlock(rx, y, rz) != BlockType.Air) { fire.Ignite(new(rx, y, rz)); break; }
                }
            }
        }
    }

    void Every30Seconds()
    {
        // Scale raid difficulty with wealth
        var econ = FindFirstObjectByType<EconomyManager>();
        var raids = FindFirstObjectByType<RaidManager>();
        if (econ != null && raids != null) raids.UpdateWealth(econ.TotalCopper);

        // Apply seasonal modifiers to farming
        var seasons = FindFirstObjectByType<FireAndSeasons>();
        var plants = FindFirstObjectByType<PlantGrowth>();
        if (seasons != null && plants != null)
            plants.growthMultiplier = seasons.GetSeasonFarmMod();

        // Trade caravan chance
        if (Random.value < 0.3f) // 30% chance every 30s
        {
            var trade = FindFirstObjectByType<TradeUI>();
            if (trade != null && !trade.IsVisible()) trade.Show();
        }
    }
}
