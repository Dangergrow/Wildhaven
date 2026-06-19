/// <summary>
/// Enum for all colonist skills. Each ranges from 0 to 20.
/// </summary>
public enum SkillType
{
    Construction,   // Building speed and quality
    Mining,         // Digging speed
    Cooking,        // Food quality and speed
    Intellectual,   // Research speed
    Medicine,       // Healing effectiveness
    Melee,          // Melee combat accuracy and damage
    Ranged,         // Ranged combat accuracy and damage
    Crafting,       // Crafting speed and quality
    Farming,        // Planting/harvesting speed and yield
    Social,         // Trading, recruiting, conversation
    AnimalHandling, // Taming, training, milking/shearing
    Hunting,        // Hunting accuracy and yield
    Trading,        // Better prices when trading
    Artistic,       // Art quality and speed
}

/// <summary>
/// Available colonist perks (positive traits).
/// </summary>
public enum Perk
{
    None,
    FastLearner,    // +50% XP gain
    Workaholic,     // Works longer without rest
    Tough,          // +30% HP, reduced injury chance
    IronWill,       // Resist mental breaks
    GreenThumb,     // +30% farming yield
    EagleEye,       // +20% ranged accuracy
    SilverTongue,   // +30% trade prices, better social
    NightOwl,       // No darkness penalty, works better at night
    Cannibal,       // Eats human meat without mood penalty, gains mood buff
    Pyromaniac,     // Gains mood from fire, may start fires
    Gourmet,        // Requires better food, but gets bigger mood buffs from good food
    Ascetic,        // No comfort/mood penalty from poor room, poor food, poor clothes
    Psychopath,     // No mood penalty from death, harvesting organs, killing
}

/// <summary>
/// Available colonist flaws (negative traits).
/// </summary>
public enum Flaw
{
    None,
    Lazy,           // Works slower, needs more recreation
    Coward,         // Runs from combat, breaks under fire
    SlowLearner,    // -50% XP gain
    Frail,          // -30% HP, higher injury chance
    Depressive,     // Mood decays faster, harder to keep happy
    Ugly,           // Social penalty with other colonists
    Glutton,        // Eats twice as much
    Alcoholic,      // Mood debuff without alcohol, addiction
    Bloodlust,      // Gets mood buff from violence/killing (others get debuff seeing this)
    Insomniac,      // Sleeps poorly, needs more sleep for same rest
    Pyrophobic,     // Strong fear of fire
}
