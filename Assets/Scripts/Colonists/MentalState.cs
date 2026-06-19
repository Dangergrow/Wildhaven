using UnityEngine;

/// <summary>
/// Handles mental breaks and inspiration for colonists based on mood.
/// Low mood = mental break, high mood = inspiration.
/// </summary>
public class MentalState : MonoBehaviour
{
    [Header("Thresholds")]
    public float extremeBreakThreshold = 2f;
    public float majorBreakThreshold = 10f;
    public float minorBreakThreshold = 20f;
    public float inspirationThreshold = 85f;

    [Header("Cooldowns")]
    public float breakCooldown = 120f; // seconds at 1x between breaks
    public float inspirationCooldown = 300f;

    [Header("Break Types")]
    public MentalBreakType[] breakTypes;

    private Colonist _colonist;
    private float _lastBreakTime = -999f;
    private float _lastInspirationTime = -999f;
    private bool _inBreak;
    private bool _inspired;

    void Awake()
    {
        _colonist = GetComponent<Colonist>();
        if (breakTypes == null || breakTypes.Length == 0)
        {
            breakTypes = new MentalBreakType[]
            {
                new MentalBreakType { name = "Binge Eating", description = "Eats all food nearby.", minMood = 0, maxMood = 15, duration = 10f, effect = BreakEffect.ConsumeAllFood },
                new MentalBreakType { name = "Berserk", description = "Attacks the nearest colonist or object.", minMood = 0, maxMood = 8, duration = 15f, effect = BreakEffect.Berserk },
                new MentalBreakType { name = "Sad Wander", description = "Wanders aimlessly, ignoring work.", minMood = 5, maxMood = 20, duration = 30f, effect = BreakEffect.SadWander },
                new MentalBreakType { name = "Hide in Room", description = "Hides and refuses to work.", minMood = 8, maxMood = 25, duration = 20f, effect = BreakEffect.Hide },
                new MentalBreakType { name = "Tantrum", description = "Destroys random objects.", minMood = 3, maxMood = 12, duration = 12f, effect = BreakEffect.Tantrum },
            };
        }
    }

    void Update()
    {
        if (_colonist == null || _colonist.currentState == ColonistState.Dead) return;

        DayCycle day = FindObjectOfType<DayCycle>();
        if (day != null && day.IsPaused) return;

        float gameTime = Time.time * (day != null ? day.gameSpeed : 1f);

        // Check for mental break
        if (!_inBreak && _colonist.mood <= minorBreakThreshold && gameTime - _lastBreakTime > breakCooldown)
        {
            TryMentalBreak();
        }

        // Check for inspiration
        if (!_inspired && _colonist.mood >= inspirationThreshold && gameTime - _lastInspirationTime > inspirationCooldown)
        {
            TryInspiration();
        }
    }

    /// <summary>
    /// Attempts a mental break based on mood severity.
    /// </summary>
    void TryMentalBreak()
    {
        float chance = 0f;
        if (_colonist.mood <= extremeBreakThreshold) chance = 1f;
        else if (_colonist.mood <= majorBreakThreshold) chance = 0.5f;
        else if (_colonist.mood <= minorBreakThreshold) chance = 0.15f;

        // Traits affect chance
        if (_colonist.perk == Perk.IronWill) chance *= 0.3f;
        if (_colonist.flaw == Flaw.Depressive) chance *= 2f;

        if (Random.value > chance) return;

        // Pick break type matching mood range
        MentalBreakType type = null;
        foreach (MentalBreakType bt in breakTypes)
        {
            if (_colonist.mood >= bt.minMood && _colonist.mood <= bt.maxMood)
            {
                type = bt; break;
            }
        }
        if (type == null) return;

        _inBreak = true;
        _lastBreakTime = Time.time;

        Debug.Log($"[Mental] {_colonist.colonistName} — {type.name}: {type.description}");

        switch (type.effect)
        {
            case BreakEffect.ConsumeAllFood:
                // Eat all available food
                Inventory inv = GetComponent<Inventory>();
                if (inv != null)
                {
                    foreach (var slot in inv.Slots)
                    {
                        if (slot.itemType == ItemType.Berries || slot.itemType == ItemType.Wheat ||
                            slot.itemType == ItemType.Bread || slot.itemType == ItemType.RawMeat)
                            slot.amount = 0;
                    }
                }
                _colonist.ModifyMood(5f); // temporary relief
                break;

            case BreakEffect.Berserk:
                // Damage nearest colonist
                ColonistSpawner sp = FindObjectOfType<ColonistSpawner>();
                if (sp != null)
                {
                    foreach (Colonist c in sp.Colonists)
                    {
                        if (c != _colonist && c != null && Vector3.Distance(transform.position, c.transform.position) < 3f)
                        {
                            c.TakeDamage(5f);
                            break;
                        }
                    }
                }
                break;

            case BreakEffect.Tantrum:
                // Destroy items
                break;
        }

        Invoke(nameof(EndBreak), type.duration);
    }

    void EndBreak()
    {
        _inBreak = false;
        _colonist.ModifyMood(-5f); // hangover
        Debug.Log($"[Mental] {_colonist.colonistName} recovered from mental break");
    }

    /// <summary>
    /// Attempts inspiration — temporary buff to work speed and quality.
    /// </summary>
    void TryInspiration()
    {
        if (Random.value > 0.2f) return; // 20% chance per tick

        _inspired = true;
        _lastInspirationTime = Time.time;
        _colonist.ModifyMood(10f);

        // Buff skills temporarily
        _colonist.constructionSkill += 2;
        _colonist.craftingSkill += 2;
        _colonist.farmingSkill += 2;

        Debug.Log($"[Mental] {_colonist.colonistName} inspired! Buff active.");
        Invoke(nameof(EndInspiration), 60f);
    }

    void EndInspiration()
    {
        _inspired = false;
        _colonist.constructionSkill -= 2;
        _colonist.craftingSkill -= 2;
        _colonist.farmingSkill -= 2;
    }

    public bool IsInBreak => _inBreak;
    public bool IsInspired => _inspired;
}

[System.Serializable]
public class MentalBreakType
{
    public string name;
    public string description;
    public float minMood;
    public float maxMood;
    public float duration;
    public BreakEffect effect;
}

public enum BreakEffect
{
    ConsumeAllFood,
    Berserk,
    SadWander,
    Hide,
    Tantrum,
    RunWild,
}
