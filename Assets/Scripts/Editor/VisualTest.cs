using UnityEngine;
using UnityEditor;
using System.Collections;

/// <summary>Automated visual test: enters Play mode, captures screenshot, verifies MainMenu.</summary>
public static class VisualTest
{
    [MenuItem("Wildhaven/Visual Test")]
    public static void Run()
    {
        EditorApplication.isPlaying = true;
        EditorApplication.update += OnUpdate;
    }

    static int _frame;

    static void OnUpdate()
    {
        _frame++;

        if (_frame == 5)
        {
            // Check MainMenu exists
            var menu = Object.FindAnyObjectByType<MainMenu>();
            if (menu != null)
                Debug.Log("[VISUAL] MainMenu found ✓");
            else
                Debug.Log("[VISUAL] FAIL: No MainMenu!");

            // Check EventSystem exists
            var es = Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>();
            if (es != null)
                Debug.Log("[VISUAL] EventSystem found ✓");
            else
                Debug.Log("[VISUAL] FAIL: No EventSystem — buttons won't work!");

            // Check Canvas with sortingOrder 999
            var canvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            bool foundMenu = false;
            foreach (var c in canvases)
            {
                if (c.sortingOrder == 999)
                {
                    Debug.Log($"[VISUAL] Menu Canvas found: sortingOrder={c.sortingOrder}, renderMode={c.renderMode} ✓");
                    foundMenu = true;
                }
            }
            if (!foundMenu) Debug.Log("[VISUAL] FAIL: No menu Canvas with sortingOrder 999!");
        }

        if (_frame == 10)
        {
            // Screenshot
            ScreenCapture.CaptureScreenshot("Assets/../visual_test.png");
            Debug.Log("[VISUAL] Screenshot saved to visual_test.png");
        }

        if (_frame == 15)
        {
            EditorApplication.isPlaying = false;
            EditorApplication.update -= OnUpdate;
            Debug.Log("[VISUAL] Test complete — check visual_test.png");
        }
    }
}
