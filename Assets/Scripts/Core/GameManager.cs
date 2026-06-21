using UnityEngine;

/// <summary>Central game initializer — connects all systems, adds missing components.</summary>
public class GameManager : MonoBehaviour
{
    void Awake()
    {
        EnsureSystem<DayCycle>("DayCycle");
        EnsureSystem<ColonistSpawner>("Spawner");
        EnsureSystem<SelectionManager>("Selection");
        EnsureSystem<CanvasHUD>("HUD"); // Canvas UI
        EnsureSystem<MainMenu>("Menu");
        EnsureSystem<GameSaveManager>("SaveMgr");
        EnsureSystem<EconomyManager>("Economy");
        EnsureSystem<FoodSpoilage>("Spoilage");
        EnsureSystem<FamilySystem>("Family");
        EnsureSystem<QuestManager>("Quests");
        EnsureSystem<ReligionSystem>("Religion");
        EnsureSystem<MedicineSystem>("Medicine");
        EnsureSystem<ColonyServices>("Services");
        EnsureSystem<StabilitySystem>("Stability");
        EnsureSystem<FireAndSeasons>("FireSeasons");

        // Add UI panels
        EnsureSystem<GameBar>("GameBar");
        EnsureSystem<WorkPanel>("WorkPanel");
        EnsureSystem<ColonistPanel>("ColPanel");
        EnsureSystem<TradeUI>("TradeUI");

        Debug.Log("[GameManager] All systems initialized");
    }

    void EnsureSystem<T>(string label) where T : Component
    {
        if (FindFirstObjectByType<T>() == null)
        {
            var go = new GameObject(label);
            go.AddComponent<T>();
        }
    }
}
