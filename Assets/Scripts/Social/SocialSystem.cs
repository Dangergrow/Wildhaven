using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages social relationships between colonists.
/// Tracks friendship/rivalry, allows marriage, and affects mood.
/// </summary>
public class SocialSystem : MonoBehaviour
{
    [Header("Settings")]
    public float socialCheckInterval = 15f; // seconds at 1x
    public float relationDecay = 0.1f; // per check, when not interacting
    public float chatBonus = 2f; // relation gain per chat
    public float fightPenalty = -10f;

    [Header("Marriage")]
    public float marriageThreshold = 80f; // relation needed to propose
    public float marriageMoodBonus = 15f; // permanent mood bonus

    // Relations: (colonistA, colonistB) → value (-100 to +100)
    private Dictionary<(int, int), float> _relations = new Dictionary<(int, int), float>();
    // Married pairs
    private HashSet<(int, int)> _married = new HashSet<(int, int)>();

    private ColonistSpawner _spawner;
    private DayCycle _day;
    private float _timer;

    void Awake()
    {
        _spawner = FindObjectOfType<ColonistSpawner>();
        _day = FindObjectOfType<DayCycle>();
    }

    void Update()
    {
        if (_day != null && _day.IsPaused) return;

        _timer += Time.deltaTime * (_day != null ? _day.gameSpeed : 1f);
        if (_timer < socialCheckInterval) return;
        _timer = 0f;

        ProcessSocial();
    }

    /// <summary>
    /// Processes social interactions between all colonist pairs.
    /// </summary>
    void ProcessSocial()
    {
        if (_spawner == null) return;
        List<Colonist> colonists = new List<Colonist>();
        foreach (Colonist c in _spawner.Colonists)
            if (c != null && c.currentState != ColonistState.Dead) colonists.Add(c);

        for (int i = 0; i < colonists.Count; i++)
        {
            for (int j = i + 1; j < colonists.Count; j++)
            {
                Colonist a = colonists[i];
                Colonist b = colonists[j];
                if (a == b) continue;

                int idA = a.GetInstanceID();
                int idB = b.GetInstanceID();
                var key = idA < idB ? (idA, idB) : (idB, idA);

                float dist = Vector3.Distance(a.transform.position, b.transform.position);

                // Close colonists interact
                if (dist < 3f)
                {
                    // Chat
                    ChangeRelation(key, chatBonus);
                    a.GetComponent<NeedsSystem>()?.Socialize(2f);
                    b.GetComponent<NeedsSystem>()?.Socialize(2f);

                    // Possible fight (if relation is very negative)
                    if (GetRelation(key) < -50f && Random.value < 0.02f)
                    {
                        ChangeRelation(key, fightPenalty);
                        a.ModifyMood(-10f);
                        b.ModifyMood(-10f);
                        Debug.Log($"[Social] {a.colonistName} and {b.colonistName} had a fight!");
                    }
                }
                else
                {
                    // Decay when apart
                    ChangeRelation(key, -relationDecay);
                }

                // Marriage proposal check
                if (!_married.Contains(key) && GetRelation(key) >= marriageThreshold)
                {
                    if (IsCompatible(a, b))
                    {
                        _married.Add(key);
                        a.ModifyMood(marriageMoodBonus);
                        b.ModifyMood(marriageMoodBonus);
                        Debug.Log($"[Social] {a.colonistName} and {b.colonistName} got married!");
                    }
                }
            }
        }
    }

    /// <summary>
    /// Checks if two colonists are compatible for marriage (hetero only, age diff < 20).
    /// </summary>
    bool IsCompatible(Colonist a, Colonist b)
    {
        if (a.isMale == b.isMale) return false;
        if (Mathf.Abs(a.age - b.age) > 20) return false;
        return true;
    }

    float GetRelation((int, int) key)
    {
        _relations.TryGetValue(key, out float val);
        return val;
    }

    void ChangeRelation((int, int) key, float delta)
    {
        if (!_relations.ContainsKey(key)) _relations[key] = 0f;
        _relations[key] = Mathf.Clamp(_relations[key] + delta, -100f, 100f);
    }

    /// <summary>
    /// Returns the relation value between two colonists.
    /// </summary>
    public float GetRelationBetween(Colonist a, Colonist b)
    {
        if (a == null || b == null) return 0f;
        int idA = a.GetInstanceID();
        int idB = b.GetInstanceID();
        return GetRelation(idA < idB ? (idA, idB) : (idB, idA));
    }

    /// <summary>
    /// Checks if two colonists are married.
    /// </summary>
    public bool AreMarried(Colonist a, Colonist b)
    {
        int idA = a.GetInstanceID();
        int idB = b.GetInstanceID();
        return _married.Contains(idA < idB ? (idA, idB) : (idB, idA));
    }
}
