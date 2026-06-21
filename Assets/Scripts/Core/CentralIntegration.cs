using UnityEngine;

/// <summary>Master integrator — auto-creates on game start. No scene setup needed.</summary>
public class CentralIntegration : MonoBehaviour
{
    private float _t1, _t5, _t30;

    private GameManager _gameManager;
    private ColonistSpawner _colonistSpawner;
    private DayCycle _dayCycle;
    private ColonyServices _colonyServices;
    private GridManager _gridManager;
    private QuestManager _questManager;
    private ForageSpawner _forageSpawner;
    private PlantGrowth _plantGrowth;
    private AnimalManager _animalManager;
    private PrisonerSystem _prisonSystem;
    private CookingSystem _cookingSystem;
    private ReligionSystem _religionSystem;
    private FireAndSeasons _fireAndSeasons;
    private EconomyManager _economyManager;
    private RaidManager _raidManager;
    private TradeUI _tradeUI;

    /// <summary>Auto-create on game start — no manual scene setup required.</summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void AutoCreate()
    {
        var go = new GameObject("__CentralIntegration__");
        go.AddComponent<CentralIntegration>();
        DontDestroyOnLoad(go);
    }

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
        if (_colonistSpawner == null) _colonistSpawner = FindFirstObjectByType<ColonistSpawner>();
        if (_dayCycle == null) _dayCycle = FindFirstObjectByType<DayCycle>();
        if (_colonistSpawner == null || _dayCycle == null) return;
        foreach (var c in _colonistSpawner.Colonists)
        {
            if (c == null || c.currentState == ColonistState.Dead) continue;
            var sched = c.GetComponent<ColonistSchedule>();
            if (sched == null) sched = c.gameObject.AddComponent<ColonistSchedule>();
            sched.ApplySchedule(c, _dayCycle.hour);
        }
    }

    void Tick5s()
    {
        if (_colonyServices == null) _colonyServices = FindFirstObjectByType<ColonyServices>();
        if (_colonistSpawner == null) _colonistSpawner = FindFirstObjectByType<ColonistSpawner>();
        if (_gridManager == null) _gridManager = FindFirstObjectByType<GridManager>();
        if (_colonistSpawner == null) return;

        if (_colonyServices != null) { _colonyServices.AutoHeal(); _colonyServices.AutoRoof(); }

        foreach (var c in _colonistSpawner.Colonists)
        {
            if (c == null || c.currentState == ColonistState.Dead) continue;
            var inv = c.GetComponent<Inventory>();

            // Quest completion
            if (_questManager == null) _questManager = FindFirstObjectByType<QuestManager>();
            if (_questManager != null && _gridManager != null)
                _questManager.TryCompleteAt(_gridManager.WorldToGrid(c.transform.position), c);

            // Forage harvest
            if (_forageSpawner == null) _forageSpawner = FindFirstObjectByType<ForageSpawner>();
            if (_forageSpawner != null && _gridManager != null)
            {
                var r = _forageSpawner.TryHarvest(_gridManager.WorldToGrid(c.transform.position));
                if (r != null && inv != null) inv.AddItem(r.Value.Item1, r.Value.Item2);
            }

            // Plant harvest
            if (_plantGrowth == null) _plantGrowth = FindFirstObjectByType<PlantGrowth>();
            if (_plantGrowth != null && _gridManager != null && c.farmingSkill >= 2)
                _plantGrowth.TryHarvestAt(_gridManager.WorldToGrid(c.transform.position), inv);

            // Hunting
            if (_animalManager == null) _animalManager = FindFirstObjectByType<AnimalManager>();
            if (_animalManager != null && _gridManager != null && c.huntingSkill >= 2)
            {
                var loot = _animalManager.Hunt(_gridManager.WorldToGrid(c.transform.position), c);
                if (loot != null && inv != null) inv.AddItem(loot.Value.Item1, loot.Value.Item2);
            }

            // Taming
            if (_animalManager == null) _animalManager = FindFirstObjectByType<AnimalManager>();
            if (_animalManager != null && _gridManager != null && c.animalHandlingSkill >= 3)
                _animalManager.Tame(_gridManager.WorldToGrid(c.transform.position), c);

            // Prisoner recruitment (warden with social skill)
            if (_prisonSystem == null) _prisonSystem = FindFirstObjectByType<PrisonerSystem>();
            if (_prisonSystem != null && c.socialSkill >= 5 && _prisonSystem.PrisonerCount > 0)
                for (int i = 0; i < _prisonSystem.PrisonerCount; i++)
                    if (_prisonSystem.TryRecruit(c, i)) break;

            // Cooking
            if (_cookingSystem == null) _cookingSystem = FindFirstObjectByType<CookingSystem>();
            if (_cookingSystem != null && inv != null)
                foreach (var rec in _cookingSystem.recipes)
                    if (_cookingSystem.TryCook(rec, inv, out _)) break;
        }

        // Religion
        if (_religionSystem == null) _religionSystem = FindFirstObjectByType<ReligionSystem>();
        if (_religionSystem != null) _religionSystem.PerformRitual(ReligionSystem.RitualType.Prayer, Vector3Int.zero, ReligionSystem.Belief.NatureWorship);

        // Fire
        if (Random.value < 0.01f && _gridManager != null)
        {
            if (_fireAndSeasons == null) _fireAndSeasons = FindFirstObjectByType<FireAndSeasons>();
            if (_fireAndSeasons != null)
            {
                int rx = Random.Range(0, _gridManager.Width), rz = Random.Range(0, _gridManager.Depth);
                for (int y = _gridManager.Height - 1; y > 0; y--)
                    if (_gridManager.GetBlock(rx, y, rz) != BlockType.Air) { _fireAndSeasons.Ignite(new(rx, y, rz)); break; }
            }
        }

        // Dead cleanup
        for (int i = _colonistSpawner.Colonists.Count - 1; i >= 0; i--)
            if (_colonistSpawner.Colonists[i] == null || _colonistSpawner.Colonists[i].currentState == ColonistState.Dead)
                _colonistSpawner.Colonists.RemoveAt(i);
    }

    void Tick30s()
    {
        if (_economyManager == null) _economyManager = FindFirstObjectByType<EconomyManager>();
        if (_raidManager == null) _raidManager = FindFirstObjectByType<RaidManager>();
        if (_economyManager != null && _raidManager != null) _raidManager.UpdateWealth(_economyManager.TotalCopper);

        if (_fireAndSeasons == null) _fireAndSeasons = FindFirstObjectByType<FireAndSeasons>();
        if (_plantGrowth == null) _plantGrowth = FindFirstObjectByType<PlantGrowth>();
        if (_fireAndSeasons != null && _plantGrowth != null) _plantGrowth.growthMultiplier = _fireAndSeasons.GetSeasonFarmMod();

        if (Random.value < 0.3f)
        {
            if (_tradeUI == null) _tradeUI = FindFirstObjectByType<TradeUI>();
            if (_tradeUI != null && !_tradeUI.IsVisible()) _tradeUI.Show();
        }
    }
}
