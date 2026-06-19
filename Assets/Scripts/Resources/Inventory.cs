using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Inventory manager for a colonist or storage container.
/// </summary>
public class Inventory : MonoBehaviour
{
    [Header("Capacity")]
    public int maxSlots = 20;
    public float maxWeight = 100f;

    public List<InventorySlot> Slots { get; private set; } = new List<InventorySlot>();
    public float TotalWeight { get; private set; }

    private void Awake()
    {
        for (int i = 0; i < maxSlots; i++)
            Slots.Add(new InventorySlot());
    }

    /// <summary>
    /// Tries to add an item. Returns amount actually added.
    /// </summary>
    public int AddItem(ItemType type, int amount)
    {
        int remaining = amount;

        // First try stacking into existing slots
        foreach (InventorySlot slot in Slots)
        {
            if (slot.itemType == type && slot.amount < slot.maxStack)
            {
                int space = slot.maxStack - slot.amount;
                int add = Mathf.Min(space, remaining);
                slot.amount += add;
                remaining -= add;
                if (remaining <= 0) return amount;
            }
        }

        // Then try empty slots
        foreach (InventorySlot slot in Slots)
        {
            if (slot.IsEmpty)
            {
                slot.itemType = type;
                slot.amount = Mathf.Min(slot.maxStack, remaining);
                remaining -= slot.amount;
                if (remaining <= 0) return amount;
            }
        }

        return amount - remaining; // how many were added
    }

    /// <summary>
    /// Removes items. Returns amount actually removed.
    /// </summary>
    public int RemoveItem(ItemType type, int amount)
    {
        int remaining = amount;
        for (int i = Slots.Count - 1; i >= 0; i--)
        {
            if (Slots[i].itemType == type)
            {
                int remove = Mathf.Min(remaining, Slots[i].amount);
                Slots[i].amount -= remove;
                remaining -= remove;
                if (Slots[i].amount <= 0) Slots[i].Clear();
                if (remaining <= 0) return amount;
            }
        }
        return amount - remaining;
    }

    /// <summary>
    /// Returns total count of an item type in inventory.
    /// </summary>
    public int Count(ItemType type)
    {
        int count = 0;
        foreach (InventorySlot slot in Slots)
            if (slot.itemType == type) count += slot.amount;
        return count;
    }

    /// <summary>
    /// Checks if inventory has at least this many of an item.
    /// </summary>
    public bool Has(ItemType type, int amount) => Count(type) >= amount;

    /// <summary>
    /// Returns first free slot index, or -1.
    /// </summary>
    public int FreeSlots()
    {
        int count = 0;
        foreach (InventorySlot slot in Slots)
            if (slot.IsEmpty || slot.amount < slot.maxStack) count++;
        return count;
    }
}

/// <summary>
/// Single inventory slot.
/// </summary>
[System.Serializable]
public class InventorySlot
{
    public ItemType itemType;
    public int amount;
    public int maxStack = 64;

    public bool IsEmpty => itemType == 0 && amount == 0;

    public void Clear()
    {
        itemType = 0;
        amount = 0;
    }
}
