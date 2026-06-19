using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Spawns colonists using Physics.Raycast against World MeshCollider.
/// </summary>
public class ColonistSpawner : MonoBehaviour
{
    public int startingColonists = 3;
    public GameObject colonistPrefab;
    public Vector3 spawnCenter = new Vector3(50f, 60f, 50f);
    public float spawnRadius = 10f;
    public string[] maleNames = { "Ivan", "Boris", "Dmitri", "Alexei", "Sergei" };
    public string[] femaleNames = { "Anna", "Maria", "Elena", "Olga", "Natasha" };
    public string[] surnames = { "Petrov", "Ivanov", "Sidorov", "Kuznetsov", "Smirnov" };

    public List<Colonist> Colonists { get; private set; } = new List<Colonist>();

    void Start() { Invoke(nameof(DoSpawn), 1.5f); } // wait for world+collider

    void DoSpawn() { for (int i = 0; i < startingColonists; i++) SpawnColonist(FindSpot()); }

    Vector3 FindSpot()
    {
        for (int a = 0; a < 100; a++)
        {
            Vector2 c = Random.insideUnitCircle * spawnRadius;
            Vector3 from = spawnCenter + new Vector3(c.x, 0f, c.y);
            if (Physics.Raycast(from, Vector3.down, out RaycastHit hit, 80f))
                return hit.point + Vector3.up * 0.5f;
        }
        Debug.LogError("[ColonistSpawner] All raycasts failed");
        return spawnCenter + Vector3.up;
    }

    Colonist SpawnColonist(Vector3 pos)
    {
        if (colonistPrefab == null) return null;
        GameObject go = Instantiate(colonistPrefab, pos, Quaternion.identity);
        Colonist c = go.GetComponent<Colonist>();
        if (c == null) { Destroy(go); return null; }

        c.isMale = Random.value > 0.5f;
        c.colonistName = (c.isMale ? maleNames[Random.Range(0, maleNames.Length)] : femaleNames[Random.Range(0, femaleNames.Length)]) + " " + surnames[Random.Range(0, surnames.Length)];
        c.age = Random.Range(18, 50);
        Colonists.Add(c);
        Debug.Log($"[ColonistSpawner] {c.colonistName} at {pos}");
        return c;
    }

    public void RemoveColonist(Colonist c) => Colonists.Remove(c);
}
