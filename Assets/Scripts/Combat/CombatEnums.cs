/// <summary>
/// Enemy types that can raid the colony.
/// </summary>
public enum EnemyType
{
    BanditMelee,
    BanditRanged,
    BanditBoss,
    Wolf,
    Bear,
    Spider,
    Pirate,
    Cultist,
    Mutant,
    Mechanoid,
}

/// <summary>
/// Combat state of a unit.
/// </summary>
public enum CombatState
{
    Idle,
    Moving,
    Attacking,
    Fleeing,
    Dead,
}

/// <summary>
/// Damage types for weapons.
/// </summary>
public enum DamageType
{
    Slash,
    Pierce,
    Blunt,
    Fire,
    Poison,
    Explosive,
}
