using UnityEngine;

/// <summary>
/// Equipment manager for a colonist. Handles equipping weapons, armor, and tools.
/// Each slot holds one item. Items come from the colonist's inventory.
/// </summary>
public class Equipment : MonoBehaviour
{
    [Header("Equipment Slots")]
    public EquippedItem weapon;
    public EquippedItem armorHead;
    public EquippedItem armorBody;
    public EquippedItem armorLegs;
    public EquippedItem tool;

    private Colonist _colonist;
    private Inventory _inventory;

    void Awake()
    {
        _colonist = GetComponent<Colonist>();
        _inventory = GetComponent<Inventory>();
    }

    /// <summary>
    /// Equips an item from inventory to the appropriate slot.
    /// </summary>
    public bool Equip(ItemType itemType)
    {
        if (_inventory == null || !_inventory.Has(itemType, 1)) return false;

        // Find the item's data to determine slot
        // For now: use naming convention
        string name = itemType.ToString().ToLower();

        if (name.Contains("sword") || name.Contains("axe") || name.Contains("bow") ||
            name.Contains("musket") || name.Contains("crossbow") || name.Contains("pick") ||
            name.Contains("shovel") || name.Contains("hammer") || name.Contains("knife") ||
            name.Contains("rod"))
        {
            // Weapon or tool
            if (name.Contains("pick") || name.Contains("shovel") ||
                name.Contains("hammer") || name.Contains("knife") || name.Contains("rod"))
                tool = new EquippedItem { itemType = itemType };
            else
                weapon = new EquippedItem { itemType = itemType };
        }
        else if (name.Contains("helmet") || name.Contains("head"))
        {
            armorHead = new EquippedItem { itemType = itemType };
        }
        else if (name.Contains("chest") || name.Contains("body"))
        {
            armorBody = new EquippedItem { itemType = itemType };
        }
        else if (name.Contains("leg"))
        {
            armorLegs = new EquippedItem { itemType = itemType };
        }
        else return false;

        _inventory.RemoveItem(itemType, 1);
        Debug.Log($"[Equipment] {_colonist.colonistName} equipped {itemType}");

        RecalculateStats();
        return true;
    }

    /// <summary>
    /// Unequips an item from a slot back to inventory.
    /// </summary>
    public bool Unequip(EquipSlot slot)
    {
        EquippedItem item;
        switch (slot)
        {
            case EquipSlot.Weapon: item = weapon; break;
            case EquipSlot.Head: item = armorHead; break;
            case EquipSlot.Body: item = armorBody; break;
            case EquipSlot.Legs: item = armorLegs; break;
            case EquipSlot.Tool: item = tool; break;
            default: return false;
        }

        if (item.itemType == 0) return false;

        _inventory.AddItem(item.itemType, 1);

        switch (slot)
        {
            case EquipSlot.Weapon: weapon.itemType = 0; break;
            case EquipSlot.Head: armorHead.itemType = 0; break;
            case EquipSlot.Body: armorBody.itemType = 0; break;
            case EquipSlot.Legs: armorLegs.itemType = 0; break;
            case EquipSlot.Tool: tool.itemType = 0; break;
        }

        RecalculateStats();
        return true;
    }

    /// <summary>
    /// Gets current combat stats from equipment.
    /// </summary>
    public float GetAttackBonus()
    {
        float bonus = 0f;
        if (weapon.itemType != 0) bonus += 5f;
        if (tool.itemType != 0) bonus += 2f;
        return bonus;
    }

    /// <summary>Check if equipped weapon is ranged.</summary>
    public bool HasRangedWeapon()
    {
        ItemType w = weapon.itemType;
        return w == ItemType.Bow || w == ItemType.Crossbow || w == ItemType.Musket;
    }

    public float GetDefenseBonus()
    {
        float bonus = 0f;
        if (armorHead.itemType != 0) bonus += 2f;
        if (armorBody.itemType != 0) bonus += 5f;
        if (armorLegs.itemType != 0) bonus += 3f;
        return bonus;
    }

    void RecalculateStats()
    {
        _colonist.moveSpeedMultiplier = 1f;
        if (armorBody.itemType != 0) _colonist.moveSpeedMultiplier -= 0.15f;
        if (armorLegs.itemType != 0) _colonist.moveSpeedMultiplier -= 0.1f;
    }
}

[System.Serializable]
public struct EquippedItem
{
    public ItemType itemType;
    public float durability;
}

public enum EquipSlot
{
    Weapon,
    Head,
    Body,
    Legs,
    Tool,
}
