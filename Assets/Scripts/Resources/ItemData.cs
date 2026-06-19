using UnityEngine;

/// <summary>
/// ScriptableObject defining an item's properties.
/// </summary>
[CreateAssetMenu(fileName = "ItemData", menuName = "Wildhaven/Item Data")]
public class ItemData : ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    public string description;
    public Sprite icon;

    [Header("Stacking")]
    public bool isStackable = true;
    public int maxStackSize = 64;

    [Header("Value")]
    public int baseValue; // in Kopeyki

    [Header("Category")]
    public ItemCategory category;

    [Header("Equipment (if weapon/armor/tool)")]
    public bool isEquippable;
    public float durability = 100f;
    public float damageBonus;
    public float defenseBonus;
    public float workSpeedBonus; // for tools
}

public enum ItemCategory
{
    RawMaterial,
    Food,
    Plant,
    Textile,
    Weapon,
    Armor,
    Tool,
    Medicine,
    Misc,
}
