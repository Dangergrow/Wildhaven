using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages social relationships between colonists.
/// Uses colonistName as unique key.
/// </summary>
public class SocialSystem : MonoBehaviour
{
    public float socialCheckInterval = 15f;
    public float relationDecay = 0.1f;
    public float chatBonus = 2f;
    public float fightPenalty = -10f;
    public float marriageThreshold = 80f;
    public float marriageMoodBonus = 15f;

    // Relations: "nameA|nameB" → value (-100 to +100)
    private Dictionary<string, float> _relations = new Dictionary<string, float>();
    private HashSet<string> _married = new HashSet<string>();

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

    void ProcessSocial()
    {
        if (_spawner == null) return;
        List<Colonist> list = new List<Colonist>();
        foreach (Colonist c in _spawner.Colonists)
            if (c != null && c.currentState != ColonistState.Dead) list.Add(c);

        for (int i = 0; i < list.Count; i++)
        {
            for (int j = i + 1; j < list.Count; j++)
            {
                Colonist a = list[i], b = list[j];
                if (a == b) continue;

                string key = Key(a, b);
                float dist = Vector3.Distance(a.transform.position, b.transform.position);

                if (dist < 3f)
                {
                    ChangeRelation(key, chatBonus);
                    a.GetComponent<NeedsSystem>()?.Socialize(2f);
                    b.GetComponent<NeedsSystem>()?.Socialize(2f);

                    if (GetRel(key) < -50f && Random.value < 0.02f)
                    {
                        ChangeRelation(key, fightPenalty);
                        a.ModifyMood(-10f); b.ModifyMood(-10f);
                        Debug.Log($"[Social] Fight: {a.colonistName} vs {b.colonistName}");
                    }
                }
                else ChangeRelation(key, -relationDecay);

                if (!_married.Contains(key) && GetRel(key) >= marriageThreshold && Compatible(a, b))
                {
                    _married.Add(key);
                    a.ModifyMood(marriageMoodBonus); b.ModifyMood(marriageMoodBonus);
                    Debug.Log($"[Social] Married: {a.colonistName} + {b.colonistName}");
                }
            }
        }
    }

    bool Compatible(Colonist a, Colonist b)
    {
        if (a.isMale == b.isMale) return false;
        return Mathf.Abs(a.age - b.age) <= 20;
    }

    static string Key(Colonist a, Colonist b)
    {
        return string.Compare(a.colonistName, b.colonistName) < 0
            ? $"{a.colonistName}|{b.colonistName}"
            : $"{b.colonistName}|{a.colonistName}";
    }

    float GetRel(string key) { _relations.TryGetValue(key, out float v); return v; }
    void ChangeRelation(string key, float d) { if (!_relations.ContainsKey(key)) _relations[key] = 0f; _relations[key] = Mathf.Clamp(_relations[key] + d, -100f, 100f); }

    public float GetRelationBetween(Colonist a, Colonist b) => a == null || b == null ? 0f : GetRel(Key(a, b));
    public bool AreMarried(Colonist a, Colonist b) => a != null && b != null && _married.Contains(Key(a, b));
}
