using UnityEngine;

/// <summary>
/// Processes all colonist needs over time.
/// Manages hunger, thirst, fatigue, mood, comfort, social, recreation, and faith.
/// </summary>
public class NeedsSystem : MonoBehaviour
{
    [Header("Need Decay Rates (per second)")]
    [Tooltip("Hunger decay rate per second")]
    public float hungerDecay = 0.05f;

    [Tooltip("Thirst decay rate per second")]
    public float thirstDecay = 0.08f;

    [Tooltip("Fatigue gain per second (working)")]
    public float fatigueGainWorking = 0.12f;

    [Tooltip("Fatigue gain per second (idle)")]
    public float fatigueGainIdle = 0.03f;

    [Tooltip("Fatigue loss per second (sleeping)")]
    public float fatigueLossSleeping = 0.5f;

    [Tooltip("Mood decay per second (neutral)")]
    public float moodNeutralDecay = 0.01f;

    [Tooltip("Social decay per second")]
    public float socialDecay = 0.02f;

    [Tooltip("Recreation decay per second")]
    public float recreationDecay = 0.03f;

    [Tooltip("Faith decay per second")]
    public float faithDecay = 0.01f;

    [Header("Thresholds")]
    [Tooltip("Hunger level at which colonist seeks food")]
    public float hungryThreshold = 30f;

    [Tooltip("Hunger level at which colonist is starving (mood penalty)")]
    public float starvingThreshold = 10f;

    [Tooltip("Fatigue level at which colonist seeks sleep")]
    public float tiredThreshold = 70f;

    [Tooltip("Fatigue level at which colonist is exhausted (mood penalty)")]
    public float exhaustedThreshold = 90f;

    [Tooltip("Mood level below which mental breaks can occur")]
    public float mentalBreakThreshold = 20f;

    [Tooltip("Mood level above which inspiration occurs")]
    public float inspirationThreshold = 85f;

    private Colonist _colonist;

    private void Awake()
    {
        _colonist = GetComponent<Colonist>();
    }

    private void Update()
    {
        if (_colonist == null || _colonist.currentState == ColonistState.Dead) return;

        float dt = Time.deltaTime;
        ProcessNeeds(dt);
    }

    /// <summary>
    /// Updates all needs based on current state and elapsed time.
    /// </summary>
    private void ProcessNeeds(float dt)
    {
        // Hunger — always decays
        _colonist.hunger = Mathf.Max(0f, _colonist.hunger - hungerDecay * dt);
        if (IsFlawActive(Flaw.Glutton))
            _colonist.hunger = Mathf.Max(0f, _colonist.hunger - hungerDecay * dt); // double

        // Thirst — always decays
        _colonist.thirst = Mathf.Max(0f, _colonist.thirst - thirstDecay * dt);

        // Fatigue — depends on state
        if (_colonist.currentState == ColonistState.Sleeping)
        {
            _colonist.fatigue = Mathf.Max(0f, _colonist.fatigue - fatigueLossSleeping * dt);
        }
        else if (_colonist.currentState == ColonistState.Working || _colonist.currentState == ColonistState.Fighting)
        {
            _colonist.fatigue = Mathf.Min(100f, _colonist.fatigue + fatigueGainWorking * dt);
        }
        else
        {
            _colonist.fatigue = Mathf.Min(100f, _colonist.fatigue + fatigueGainIdle * dt);
        }

        // Comfort — depends on surroundings
        _colonist.comfort = Mathf.Clamp(_colonist.comfort - 0.01f * dt, 0f, 100f);

        // Social — decays over time if alone
        _colonist.social = Mathf.Max(0f, _colonist.social - socialDecay * dt);

        // Recreation — decays
        _colonist.recreation = Mathf.Max(0f, _colonist.recreation - recreationDecay * dt);

        // Faith — decays
        _colonist.faith = Mathf.Max(0f, _colonist.faith - faithDecay * dt);

        // Mood calculation
        UpdateMood(dt);
    }

    /// <summary>
    /// Calculates mood based on all need levels and traits.
    /// </summary>
    private void UpdateMood(float dt)
    {
        float moodDelta = 0f;

        // Hunger effect
        if (_colonist.hunger < starvingThreshold)
            moodDelta -= 0.15f * dt; // starving
        else if (_colonist.hunger < hungryThreshold)
            moodDelta -= 0.05f * dt; // hungry

        // Thirst effect
        if (_colonist.thirst < 20f)
            moodDelta -= 0.1f * dt;

        // Fatigue effect
        if (_colonist.fatigue > exhaustedThreshold)
            moodDelta -= 0.15f * dt;
        else if (_colonist.fatigue > tiredThreshold)
            moodDelta -= 0.05f * dt;

        // Comfort effect
        if (_colonist.comfort < 20f)
            moodDelta -= 0.05f * dt;

        // Social effect
        if (_colonist.social < 20f)
            moodDelta -= 0.03f * dt;

        // Recreation effect
        if (_colonist.recreation < 20f)
            moodDelta -= 0.03f * dt;

        // Faith effect
        if (_colonist.faith < 10f)
            moodDelta -= 0.02f * dt;

        // Trait modifiers
        if (IsFlawActive(Flaw.Depressive))
            moodDelta -= 0.02f * dt;

        // Neutral decay
        moodDelta -= moodNeutralDecay * dt;

        _colonist.mood = Mathf.Clamp(_colonist.mood + moodDelta, 0f, 100f);
    }

    /// <summary>
    /// Feeds the colonist, restoring hunger.
    /// </summary>
    public void Eat(float nutritionValue, float moodBonus = 0f)
    {
        _colonist.hunger = Mathf.Min(100f, _colonist.hunger + nutritionValue);
        if (moodBonus > 0f)
            _colonist.ModifyMood(moodBonus);
    }

    /// <summary>
    /// Colonist drinks, restoring thirst.
    /// </summary>
    public void Drink(float hydrationValue)
    {
        _colonist.thirst = Mathf.Min(100f, _colonist.thirst + hydrationValue);
    }

    /// <summary>
    /// Provides recreation to colonist.
    /// </summary>
    public void Recreate(float amount)
    {
        _colonist.recreation = Mathf.Min(100f, _colonist.recreation + amount);
        _colonist.ModifyMood(5f);
    }

    /// <summary>
    /// Social interaction between two colonists.
    /// </summary>
    public void Socialize(float amount)
    {
        _colonist.social = Mathf.Min(100f, _colonist.social + amount);
    }

    /// <summary>
    /// Faith activity (prayer, ritual).
    /// </summary>
    public void Pray(float amount)
    {
        _colonist.faith = Mathf.Min(100f, _colonist.faith + amount);
        _colonist.ModifyMood(3f);
    }

    private bool IsPerkActive(Perk perk) => _colonist.perk == perk;
    private bool IsFlawActive(Flaw flaw) => _colonist.flaw == flaw;
}
