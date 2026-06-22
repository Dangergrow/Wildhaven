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
        EnsureSystem<RepairSystem>("Repair");
        EnsureSystem<AnimalManager>("Animals");
        EnsureSystem<PrisonerSystem>("Prisoners");
        EnsureSystem<BurialSystem>("Burial");
        EnsureSystem<PauseMenu>("PauseMenu");
        EnsureSystem<MapOverlay>("MapOverlay");
        EnsureSystem<FloorController>("FloorController");

        // Add UI panels
        EnsureSystem<GameBar>("GameBar");
        EnsureSystem<CentralIntegration>("Integration");

        // Ensure EventSystem exists for ALL UI
        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // Fix cameras: tag CameraController cam as MainCamera, disable others
        var allCams = FindObjectsByType<Camera>(FindObjectsSortMode.None);
        Camera mainCam = null;
        foreach (var cam in allCams)
        {
            if (cam.GetComponent<CameraController>() != null)
            {
                cam.tag = "MainCamera";
                cam.enabled = true;
                mainCam = cam;
            }
            else if (cam.tag != "MainCamera")
            {
                cam.enabled = false; // disable extra cameras
            }
        }
        if (mainCam == null && allCams.Length > 0)
        {
            allCams[0].tag = "MainCamera";
            allCams[0].enabled = true;
        }
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
