using UnityEngine;
using UnityEditor;

public static class SimpleTest
{
    [MenuItem("Wildhaven/Simple Test")]
    public static void Run()
    {
        Debug.Log("[TEST] Simple test PASS");
        EditorApplication.Exit(0);
    }
}
