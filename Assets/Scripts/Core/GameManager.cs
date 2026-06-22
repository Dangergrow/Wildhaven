using UnityEngine;
using System.Collections;

/// <summary>Central game initializer — connects all systems, adds missing components.</summary>
public class GameManager : MonoBehaviour
{
    void Awake()
    {
        // CRITICAL: Create GridManager FIRST, before anything else depends on it.
        var existingGrid = FindFirstObjectByType<GridManager>();
        if (existingGrid == null)
        {
            var worldGo = new GameObject("World");
            var gm = worldGo.AddComponent<GridManager>();
            gm.worldWidth = 100; gm.worldHeight = 32; gm.worldDepth = 100;
            gm.seed = System.DateTime.Now.Millisecond;
            // Wait for Awake to initialize, then force build
            StartCoroutine(InitWorldDelayed(gm));
        }
        else if (existingGrid.GetComponent<MeshFilter>() == null)
        {
            // GridManager exists but not initialized — force init
            Debug.LogWarning("[GameManager] GridManager found but not initialized — reinitializing");
            existingGrid.InitGrid();
            existingGrid.GenerateTerrain();
            existingGrid.BuildAllChunks();
        }

        // Ensure MainCamera exists and is tagged
        if (Camera.main == null)
        {
            var camGo = new GameObject("MainCamera");
            camGo.tag = "MainCamera";
            var c = camGo.AddComponent<Camera>();
            c.transform.position = new Vector3(50, 55, 50);
            c.transform.rotation = Quaternion.Euler(75, 45, 0);
            camGo.AddComponent<AudioListener>();
            camGo.AddComponent<CameraController>();
        }

        EnsureSystem<DayCycle>("DayCycle");
        EnsureSystem<ColonistSpawner>("Spawner");
        EnsureSystem<BuildManager>("BuildManager");
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
        // StabilitySystem: algorithm needs tuning — BFS marks too many blocks unsupported.
        // Disabled until proper fix (only Wood/WoodPlanks/StoneBrick above natural terrain).
        // EnsureSystem<StabilitySystem>("Stability");
        EnsureSystem<FireAndSeasons>("FireSeasons");
        EnsureSystem<RepairSystem>("Repair");
        EnsureSystem<AnimalManager>("Animals");
        EnsureSystem<PrisonerSystem>("Prisoners");
        EnsureSystem<BurialSystem>("Burial");
        EnsureSystem<PauseMenu>("PauseMenu");
        EnsureSystem<MapOverlay>("MapOverlay");
        EnsureSystem<FloorController>("FloorController");
        EnsureSystem<ZoneDesignator>("ZoneDesignator");

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
        Camera foundCam = null;
        foreach (var cam in allCams)
        {
            if (cam.GetComponent<CameraController>() != null)
            {
                cam.tag = "MainCamera";
                cam.enabled = true;
                foundCam = cam;
            }
            else if (cam.tag != "MainCamera")
            {
                cam.enabled = false; // disable extra cameras
            }
        }
        if (foundCam == null && allCams.Length > 0)
        {
            allCams[0].tag = "MainCamera";
            allCams[0].enabled = true;
        }
        EnsureSystem<WorkPanel>("WorkPanel");
        EnsureSystem<ColonistPanel>("ColPanel");
        EnsureSystem<TradeUI>("TradeUI");
        EnsureSystem<RuntimeTestRunner>("TestRunner");

        Debug.Log("[GameManager] All systems initialized");
    }

    void EnsureSystem<T>(string label) where T : Component
    {
        if (FindFirstObjectByType<T>() == null)
        {
            var go = new GameObject(label);
            var comp = go.AddComponent<T>();
        }
    }

    IEnumerator InitWorldDelayed(GridManager gm)
    {
        yield return null; // wait one frame for Awake to complete
        if (gm != null && gm.transform.childCount == 0)
        {
            Debug.Log("[GameManager] Force-initializing GridManager...");
            gm.InitGrid();
            gm.GenerateTerrain();
            gm.BuildAllChunks();
        }
    }
}
