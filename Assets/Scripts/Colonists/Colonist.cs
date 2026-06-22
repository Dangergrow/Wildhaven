using UnityEngine;

/// <summary>
/// Core colonist class. Holds all data for a single colonist: skills, needs, perks, flaws, inventory, etc.
/// </summary>
public class Colonist : MonoBehaviour
{
    #region Identity

    [Header("Identity")]
    [Tooltip("Colonist display name")]
    public string colonistName = "Colonist";

    [Tooltip("Age in years (16-70)")]
    [Range(16, 70)]
    public int age = 25;

    [Tooltip("Male or female")]
    public bool isMale = true;

    [Tooltip("Faction this colonist belongs to")]
    public int factionId;

    [Tooltip("Background story")]
    public string backstory;

    #endregion

    #region Health and Needs

    [Header("Health")]
    [Tooltip("Current health points")]
    [Range(0f, 100f)]
    public float health = 100f;

    [Tooltip("Maximum health points")]
    public float maxHealth = 100f;

    [Header("Needs")]
    [Tooltip("Hunger level. 100 = full, 0 = starving")]
    [Range(0f, 100f)]
    public float hunger = 80f;

    [Tooltip("Thirst level. 100 = hydrated, 0 = dehydrated")]
    [Range(0f, 100f)]
    public float thirst = 80f;

    [Tooltip("Fatigue level. 0 = rested, 100 = exhausted")]
    [Range(0f, 100f)]
    public float fatigue;

    [Tooltip("Mood level. 0 = miserable, 100 = ecstatic")]
    [Range(0f, 100f)]
    public float mood = 70f;

    [Tooltip("Comfort level. Affects rest quality and mood")]
    [Range(0f, 100f)]
    public float comfort = 50f;

    [Tooltip("Social need. 0 = lonely, 100 = socially fulfilled")]
    [Range(0f, 100f)]
    public float social = 50f;

    [Tooltip("Recreation need. 0 = bored, 100 = entertained")]
    [Range(0f, 100f)]
    public float recreation = 50f;

    [Tooltip("Faith need. 0 = faithless, 100 = devout")]
    [Range(0f, 100f)]
    public float faith = 50f;

    #endregion

    #region Skills

    [Header("Skills (0-20)")]
    [Range(0, 20)] public int constructionSkill;
    [Range(0, 20)] public int miningSkill;
    [Range(0, 20)] public int cookingSkill;
    [Range(0, 20)] public int intellectualSkill;
    [Range(0, 20)] public int medicineSkill;
    [Range(0, 20)] public int meleeSkill;
    [Range(0, 20)] public int rangedSkill;
    [Range(0, 20)] public int craftingSkill;
    [Range(0, 20)] public int farmingSkill;
    [Range(0, 20)] public int socialSkill;
    [Range(0, 20)] public int animalHandlingSkill;
    [Range(0, 20)] public int huntingSkill;
    [Range(0, 20)] public int tradingSkill;
    [Range(0, 20)] public int artisticSkill;

    #endregion

    #region Traits

    [Header("Traits")]
    [Tooltip("Positive trait")]
    public Perk perk = Perk.None;

    [Tooltip("Negative trait")]
    public Flaw flaw = Flaw.None;

    #endregion

    #region State

    [Header("Current State")]
    [Tooltip("Current activity")]
    public ColonistState currentState = ColonistState.Idle;

    [Tooltip("Is in combat mode (drafted)")]
    public bool isDrafted;

    [Tooltip("Is currently doing a priority task")]
    public bool isPriorityTask;

    [Tooltip("Current equipment")]
    public string equippedItem;

    [Tooltip("Movement speed multiplier (0-2)")]
    public float moveSpeedMultiplier = 1f;

    #endregion

    /// <summary>
    /// Gets the skill value for a given skill type.
    /// </summary>
    public int GetSkill(SkillType type)
    {
        return type switch
        {
            SkillType.Construction => constructionSkill,
            SkillType.Mining => miningSkill,
            SkillType.Cooking => cookingSkill,
            SkillType.Intellectual => intellectualSkill,
            SkillType.Medicine => medicineSkill,
            SkillType.Melee => meleeSkill,
            SkillType.Ranged => rangedSkill,
            SkillType.Crafting => craftingSkill,
            SkillType.Farming => farmingSkill,
            SkillType.Social => socialSkill,
            SkillType.AnimalHandling => animalHandlingSkill,
            SkillType.Hunting => huntingSkill,
            SkillType.Trading => tradingSkill,
            SkillType.Artistic => artisticSkill,
            _ => 0,
        };
    }

    /// <summary>
    /// Applies damage to the colonist. Returns true if killed.
    /// </summary>
    public bool TakeDamage(float amount)
    {
        health -= amount;
        FloatingText.Spawn(transform.position, $"-{Mathf.RoundToInt(amount)}", Color.yellow);
        if (health <= 0f)
        {
            health = 0f;
            Die();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Heals the colonist by a given amount.
    /// </summary>
    public void Heal(float amount)
    {
        health = Mathf.Min(health + amount, maxHealth);
        FloatingText.Spawn(transform.position, $"+{Mathf.RoundToInt(amount)}", Color.green, 1f);
    }

    /// <summary>
    /// Colonist dies.
    /// </summary>
    private void Die()
    {
        currentState = ColonistState.Dead;
        // Leave a corpse on the ground
        GameObject corpse = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        corpse.transform.position = transform.position + Vector3.up * 0.3f;
        corpse.transform.localScale = new Vector3(0.7f, 0.3f, 0.7f);
        corpse.GetComponent<Renderer>().material.color = new Color(0.15f, 0.05f, 0.05f);
        corpse.name = $"Corpse_{colonistName}";
        Destroy(corpse.GetComponent<Collider>());
        // Corpse decays after 60 seconds (can be buried sooner)
        Destroy(corpse, 60f);
        Debug.Log($"[Colonist] {colonistName} has died. Corpse at {transform.position}.");
    }

    /// <summary>
    /// Adds mood modifier. Positive = buff, negative = debuff.
    /// </summary>
    public void ModifyMood(float amount)
    {
        mood = Mathf.Clamp(mood + amount, 0f, 100f);
    }
}

/// <summary>
/// Current activity state of a colonist.
/// </summary>
public enum ColonistState
{
    Idle,
    Moving,
    Working,
    Eating,
    Sleeping,
    Fighting,
    Fleeing,
    Incapacitated,
    Dead,
    Socializing,
    Praying,
    Recreation,
}
