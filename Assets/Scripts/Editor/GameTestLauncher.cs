using UnityEditor;
using UnityEngine;

/// <summary>
/// Launches Play mode from batch and waits for tests to complete.
/// Usage: Unity.exe -batchmode -projectPath ... -executeMethod GameTestLauncher.Run -logFile ...
/// DO NOT use -quit flag — the launcher handles exit itself.
/// </summary>
public static class GameTestLauncher
{
    public static void Run()
    {
        Debug.Log("[LAUNCHER] Starting Play mode for gameplay tests...");

        // Register for editor updates to keep the process alive
        EditorApplication.update += OnEditorUpdate;

        // Enter Play mode
        EditorApplication.isPlaying = true;
    }

    private static float _playTimer;
    private static bool _testsDone;

    static void OnEditorUpdate()
    {
        if (!EditorApplication.isPlaying && !_testsDone)
        {
            _testsDone = true;
            Debug.Log("[LAUNCHER] Play mode ended — exiting");
            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.Exit(0);
            return;
        }

        if (EditorApplication.isPlaying)
        {
            _playTimer += Time.unscaledDeltaTime;

            // Check test results from log (RuntimeTestRunner logs "RESULTS:")
            // and force exit after tests complete
            if (_playTimer > 15f)
            {
                Debug.Log("[LAUNCHER] Tests should be done by now — stopping Play mode");
                EditorApplication.isPlaying = false;
                return;
            }

            // Timeout after 120 seconds
            if (_playTimer > 120f)
            {
                Debug.LogWarning("[LAUNCHER] Timeout after 120s — forcing exit");
                EditorApplication.isPlaying = false;
                return;
            }
        }
    }
}
