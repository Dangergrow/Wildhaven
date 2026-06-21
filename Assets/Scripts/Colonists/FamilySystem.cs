using UnityEngine;
using System.Collections.Generic;

/// <summary>Family system: relationships, marriage, pregnancy, children.</summary>
public class FamilySystem : MonoBehaviour
{
    private ColonistSpawner _spawner;
    private DayCycle _day;
    private float _checkTimer;

    public struct FamilyRecord
    {
        public Colonist parent1;
        public Colonist parent2;
        public Colonist child;
        public float birthDay;
    }
    private List<FamilyRecord> _families = new();

    void Awake()
    {
        _spawner = FindObjectOfType<ColonistSpawner>();
        _day = FindObjectOfType<DayCycle>();
    }

    void Update()
    {
        _checkTimer += Time.unscaledDeltaTime;
        if (_checkTimer < 10f) return; // check every 10 seconds
        _checkTimer = 0f;

        if (_spawner == null || _day == null) return;

        CheckPregnancies();
        CheckChildAging();
    }

    /// <summary>Try to make a baby between two married colonists.</summary>
    public bool TryMakeBaby(Colonist parent1, Colonist parent2)
    {
        // Requirements: hetero, married, living, ages 18-45 (female)
        if (parent1.isMale == parent2.isMale) return false;
        Colonist mother = parent1.isMale ? parent2 : parent1;
        Colonist father = parent1.isMale ? parent1 : parent2;
        if (mother.age < 18 || mother.age > 45 || father.age < 18) return false;
        if (mother.currentState == ColonistState.Dead || father.currentState == ColonistState.Dead) return false;

        // 10% chance per check
        if (Random.value > 0.1f) return false;

        // Create baby
        var baby = SpawnChild(mother, father);
        if (baby == null) return false;
        _families.Add(new FamilyRecord { parent1 = mother, parent2 = father, child = baby, birthDay = _day.day });
        Debug.Log($"[Family] {mother.colonistName} gave birth to {baby.colonistName}!");
        return true;
    }

    Colonist SpawnChild(Colonist mother, Colonist father)
    {
        if (_spawner == null || _spawner.colonistPrefab == null) return null;
        Vector3 pos = mother.transform.position + Vector3.right;
        var go = Instantiate(_spawner.colonistPrefab, pos, Quaternion.identity);
        var child = go.GetComponent<Colonist>();
        child.colonistName = $"{father.colonistName.Split(' ')[0]} Jr.";
        child.age = 0;
        child.isMale = Random.value > 0.5f;
        child.health = 50; child.maxHealth = 50; // baby has less HP
        for (int i = 0; i < 14; i++) SetSkill(child, i, Random.Range(0, 2)); // baby has low skills
        child.currentState = ColonistState.Idle;
        _spawner.Colonists.Add(child);
        return child;
    }

    void CheckPregnancies()
    {
        var socialSys = FindObjectOfType<SocialSystem>();
        if (socialSys == null) return;

        // Check each married pair for pregnancy chance
        foreach (var c1 in _spawner.Colonists)
        {
            if (c1.currentState == ColonistState.Dead) continue;
            foreach (var c2 in _spawner.Colonists)
            {
                if (c2.currentState == ColonistState.Dead || c1 == c2) continue;
                if (socialSys.AreMarried(c1, c2))
                    TryMakeBaby(c1, c2);
            }
        }
    }

    void CheckChildAging()
    {
        foreach (var c in _spawner.Colonists)
        {
            if (c.age < 18 && c.currentState != ColonistState.Dead)
            {
                // Children age 3x faster
                if (_day != null && _day.day % 20 == 0) c.age++;
                // Children can't do heavy work
                if (c.currentState == ColonistState.Working && c.age < 14)
                    c.currentState = ColonistState.Idle;
            }
        }
    }

    void SetSkill(Colonist c, int idx, int val)
    {
        switch (idx)
        {
            case 0: c.constructionSkill = val; break; case 1: c.miningSkill = val; break;
            case 2: c.cookingSkill = val; break; case 3: c.intellectualSkill = val; break;
            case 4: c.medicineSkill = val; break; case 5: c.meleeSkill = val; break;
            case 6: c.rangedSkill = val; break; case 7: c.craftingSkill = val; break;
            case 8: c.farmingSkill = val; break; case 9: c.socialSkill = val; break;
            case 10: c.animalHandlingSkill = val; break; case 11: c.huntingSkill = val; break;
            case 12: c.tradingSkill = val; break; case 13: c.artisticSkill = val; break;
        }
    }
}
