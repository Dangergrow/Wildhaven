using UnityEngine;
using System.Collections.Generic;

/// <summary>Manages player economy: money, trading, dynamic prices.</summary>
public class EconomyManager : MonoBehaviour
{
    public int copperCoins; // 100 copper = 1 silver, 100 silver = 1 gold
    public int silverCoins;
    public int goldCoins;

    /// <summary>Total value in copper equivalent.</summary>
    public int TotalCopper => goldCoins * 10000 + silverCoins * 100 + copperCoins;

    /// <summary>Get display string for current funds.</summary>
    public string FundsDisplay
    {
        get
        {
            if (goldCoins > 0) return $"{goldCoins}g {silverCoins}s {copperCoins}c";
            if (silverCoins > 0) return $"{silverCoins}s {copperCoins}c";
            return $"{copperCoins}c";
        }
    }

    /// <summary>Add money. Negative to deduct.</summary>
    public bool ModifyMoney(int copper)
    {
        int total = TotalCopper + copper;
        if (total < 0) return false;
        goldCoins = total / 10000;
        silverCoins = (total % 10000) / 100;
        copperCoins = total % 100;
        return true;
    }

    /// <summary>Get base buy price for an item (what player pays to buy).</summary>
    public int GetBuyPrice(ItemType item, float reputation)
    {
        int basePrice = GetBasePrice(item);
        // Reputation discount: +100 rep = 30% discount
        float repMult = 1f - (reputation / 100f) * 0.3f;
        return Mathf.Max(1, Mathf.RoundToInt(basePrice * repMult));
    }

    /// <summary>Get sell price (what player gets for selling).</summary>
    public int GetSellPrice(ItemType item, float reputation)
    {
        int basePrice = GetBasePrice(item);
        float repMult = 1f + (reputation / 100f) * 0.2f; // good rep = better prices
        return Mathf.Max(1, Mathf.RoundToInt(basePrice * 0.5f * repMult));
    }

    int GetBasePrice(ItemType t) => t switch
    {
        ItemType.WoodLog => 2, ItemType.StoneBlock => 3, ItemType.IronOre => 8,
        ItemType.CopperOre => 6, ItemType.Coal => 5, ItemType.GoldIngot => 50,
        ItemType.IronIngot => 20, ItemType.SteelIngot => 40, ItemType.BronzeIngot => 30,
        ItemType.RawMeat => 5, ItemType.CookedMeat => 12, ItemType.Bread => 8,
        ItemType.Berries => 3, ItemType.Wheat => 4, ItemType.Potato => 4,
        ItemType.Fish => 6, ItemType.RationPack => 15, ItemType.MedicalHerb => 8,
        ItemType.Coin => 1, ItemType.Bow => 25, ItemType.Crossbow => 50, ItemType.Musket => 100,
        _ => 5,
    };

    /// <summary>Generate a trader's inventory (what they have to sell).</summary>
    public List<(ItemType, int)> GenerateTraderStock()
    {
        var stock = new List<(ItemType, int)>();
        var rng = new System.Random(Time.frameCount);
        ItemType[] traderItems = { ItemType.WoodLog, ItemType.StoneBlock, ItemType.IronOre, ItemType.Coal,
            ItemType.RawMeat, ItemType.Bread, ItemType.Berries, ItemType.MedicalHerb, ItemType.Bow, ItemType.Crossbow,
            ItemType.Wheat, ItemType.Potato, ItemType.Fish, ItemType.RationPack, ItemType.CopperOre };
        int count = rng.Next(3, 8);
        for (int i = 0; i < count; i++)
        {
            ItemType t = traderItems[rng.Next(traderItems.Length)];
            int amount = rng.Next(1, 20);
            stock.Add((t, amount));
        }
        return stock;
    }
}
