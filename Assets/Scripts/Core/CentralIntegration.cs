using UnityEngine;

/// <summary>Master integrator — calls ALL systems on timers. Fixes dead code.</summary>
public class CentralIntegration : MonoBehaviour
{
    private float _t1, _t5, _t30;

    void Awake()
    {
        if (FindFirstObjectByType<GameManager>() == null)
            gameObject.AddComponent<GameManager>();
    }

    void Update()
    {
        _t1 += Time.unscaledDeltaTime; _t5 += Time.unscaledDeltaTime; _t30 += Time.unscaledDeltaTime;
        if (_t1 > 1f) { _t1 = 0f; Tick1s(); }
        if (_t5 > 5f) { _t5 = 0f; Tick5s(); }
        if (_t30 > 30f) { _t30 = 0f; Tick30s(); }
    }

    void Tick1s()
    {
        var spawner = FindFirstObjectByType<ColonistSpawner>();
        var day = FindFirstObjectByType<DayCycle>();
        if (spawner == null || day == null) return;
        foreach (var c in spawner.Colonists)
        {
            if (c == null || c.currentState == ColonistState.Dead) continue;
            var sched = c.GetComponent<ColonistSchedule>();
            if (sched == null) sched = c.gameObject.AddComponent<ColonistSchedule>();
            sched.ApplySchedule(c, day.hour);
        }
    }

    void Tick5s()
    {
        var colony = FindFirstObjectByType<ColonyServices>();
        var spawner = FindFirstObjectByType<ColonistSpawner>();
        var gm = FindFirstObjectByType<GridManager>();
        if (spawner == null) return;

        if (colony != null) { colony.AutoHeal(); colony.AutoRoof(); }

        foreach (var c in spawner.Colonists)
        {
            if (c == null || c.currentState == ColonistState.Dead) continue;
            var inv = c.GetComponent<Inventory>();

            // Quest completion
            var quests = FindFirstObjectByType<QuestManager>();
            if (quests != null && gm != null)
                quests.TryCompleteAt(gm.WorldToGrid(c.transform.position), c);

            // Forage harvest
            var forage = FindFirstObjectByType<ForageSpawner>();
            if (forage != null && gm != null)
            {
                var r = forage.TryHarvest(gm.WorldToGrid(c.transform.position));
                if (r != null && inv != null) inv.AddItem(r.Value.Item1, r.Value.Item2);
            }

            // Plant harvest
            var plants = FindFirstObjectByType<PlantGrowth>();
            if (plants != null && gm != null && c.farmingSkill >= 2)
                plants.TryHarvestAt(gm.WorldToGrid(c.transform.position), inv);

            // Hunting
            var animals = FindFirstObjectByType<AnimalManager>();
            if (animals != null && gm != null && c.huntingSkill >= 2)
            {
                var loot = animals.Hunt(gm.WorldToGrid(c.transform.position), c);
                if (loot != null && inv != null) inv.AddItem(loot.Value.Item1, loot.Value.Item2);
            }

            // Taming
            if (animals != null && gm != null && c.animalHandlingSkill >= 3)
                animals.Tame(gm.WorldToGrid(c.transform.position), c);

            // Prisoner recruitment (warden with social skill)
            var prison = FindFirstObjectByType<PrisonerSystem>();
            if (prison != null && c.socialSkill >= 5 && prison.PrisonerCount > 0)
                for (int i = 0; i < prison.PrisonerCount; i++)
                    if (prison.TryRecruit(c, i)) break;

            // Cooking
            var cooking = FindFirstObjectByType<CookingSystem>();
            if (cooking != null && inv != null)
                foreach (var rec in cooking.recipes)
                    if (cooking.TryCook(rec, inv, out _)) break;
        }

        // Religion
        var rel = FindFirstObjectByType<ReligionSystem>();
        if (rel != null) rel.PerformRitual(ReligionSystem.RitualType.Prayer, Vector3Int.zero, ReligionSystem.Belief.NatureWorship);

        // Fire
        if (Random.value < 0.01f && gm != null)
        {
            var fire = FindFirstObjectByType<FireAndSeasons>();
            if (fire != null)
            {
                int rx = Random.Range(0, gm.Width), rz = Random.Range(0, gm.Depth);
                for (int y = gm.Height - 1; y > 0; y--)
                    if (gm.GetBlock(rx, y, rz) != BlockType.Air) { fire.Ignite(new(rx, y, rz)); break; }
            }
        }

        // Dead cleanup
        for (int i = spawner.Colonists.Count - 1; i >= 0; i--)
            if (spawner.Colonists[i] == null || spawner.Colonists[i].currentState == ColonistState.Dead)
                spawner.Colonists.RemoveAt(i);
    }

    void Tick30s()
    {
        var econ = FindFirstObjectByType<EconomyManager>();
        var raids = FindFirstObjectByType<RaidManager>();
        if (econ != null && raids != null) raids.UpdateWealth(econ.TotalCopper);

        var seasons = FindFirstObjectByType<FireAndSeasons>();
        var plants = FindFirstObjectByType<PlantGrowth>();
        if (seasons != null && plants != null) plants.growthMultiplier = seasons.GetSeasonFarmMod();

        if (Random.value < 0.3f)
        {
            var trade = FindFirstObjectByType<TradeUI>();
            if (trade != null && !trade.IsVisible()) trade.Show();
        }
    }
}
