using UnityEngine;
using System.Collections.Generic;

/// <summary>Cooking system: recipes, stations, food effects.</summary>
public class CookingSystem : MonoBehaviour
{
    public Recipe[] recipes;

    [System.Serializable]
    public struct Recipe
    {
        public string name;
        public ItemType[] ingredients;
        public int[] amounts;
        public ItemType result;
        public int resultCount;
        public float nutritionValue;
        public float moodBonus;
        public float cookTime;
        public string requiredStation; // "Campfire", "Stove", "Oven", "Brewery"
    }

    void Awake()
    {
        if (recipes == null || recipes.Length == 0) InitDefaultRecipes();
    }

    void InitDefaultRecipes()
    {
        recipes = new Recipe[]
        {
            // Basic cooking
            new() { name = "Cooked Meat", ingredients = new[]{ItemType.RawMeat}, amounts = new[]{1}, result = ItemType.CookedMeat, resultCount = 1, nutritionValue = 40, moodBonus = 5, cookTime = 3, requiredStation = "Campfire" },
            new() { name = "Bread", ingredients = new[]{ItemType.Wheat}, amounts = new[]{2}, result = ItemType.Bread, resultCount = 1, nutritionValue = 30, moodBonus = 3, cookTime = 5, requiredStation = "Stove" },
            new() { name = "Rations", ingredients = new[]{ItemType.RawMeat, ItemType.Potato}, amounts = new[]{1,1}, result = ItemType.RationPack, resultCount = 1, nutritionValue = 50, moodBonus = 5, cookTime = 4, requiredStation = "Campfire" },
            new() { name = "Fried Fish", ingredients = new[]{ItemType.Fish}, amounts = new[]{1}, result = ItemType.CookedMeat, resultCount = 1, nutritionValue = 35, moodBonus = 8, cookTime = 2, requiredStation = "Campfire" },
            new() { name = "Mushroom Stew", ingredients = new[]{ItemType.Mushroom, ItemType.Potato}, amounts = new[]{2,1}, result = ItemType.RationPack, resultCount = 2, nutritionValue = 45, moodBonus = 6, cookTime = 5, requiredStation = "Stove" },
            // Advanced cooking
            new() { name = "Berry Pie", ingredients = new[]{ItemType.Berries, ItemType.Wheat}, amounts = new[]{3,2}, result = ItemType.RationPack, resultCount = 2, nutritionValue = 55, moodBonus = 10, cookTime = 7, requiredStation = "Oven" },
            new() { name = "Meat Pie", ingredients = new[]{ItemType.RawMeat, ItemType.Wheat, ItemType.Potato}, amounts = new[]{2,2,1}, result = ItemType.RationPack, resultCount = 3, nutritionValue = 70, moodBonus = 12, cookTime = 10, requiredStation = "Oven" },
            new() { name = "Fish Soup", ingredients = new[]{ItemType.Fish, ItemType.Potato, ItemType.MedicalHerb}, amounts = new[]{2,1,1}, result = ItemType.RationPack, resultCount = 2, nutritionValue = 50, moodBonus = 8, cookTime = 6, requiredStation = "Stove" },
            new() { name = "Roasted Mushrooms", ingredients = new[]{ItemType.Mushroom}, amounts = new[]{3}, result = ItemType.CookedMeat, resultCount = 1, nutritionValue = 30, moodBonus = 4, cookTime = 3, requiredStation = "Campfire" },
            new() { name = "Potato Bake", ingredients = new[]{ItemType.Potato, ItemType.MedicalHerb}, amounts = new[]{3,1}, result = ItemType.RationPack, resultCount = 2, nutritionValue = 45, moodBonus = 7, cookTime = 5, requiredStation = "Oven" },
            new() { name = "Trail Mix", ingredients = new[]{ItemType.Berries, ItemType.Mushroom, ItemType.Wheat}, amounts = new[]{2,1,1}, result = ItemType.RationPack, resultCount = 1, nutritionValue = 35, moodBonus = 3, cookTime = 2, requiredStation = "Campfire" },
            new() { name = "Herbal Remedy", ingredients = new[]{ItemType.MedicalHerb, ItemType.MedicalHerb}, amounts = new[]{3,1}, result = ItemType.RationPack, resultCount = 1, nutritionValue = 10, moodBonus = 5, cookTime = 4, requiredStation = "Campfire" },
            // Brewing
            new() { name = "Beer", ingredients = new[]{ItemType.Wheat}, amounts = new[]{5}, result = ItemType.Beer, resultCount = 1, nutritionValue = 5, moodBonus = 12, cookTime = 8, requiredStation = "Brewery" },
            new() { name = "Ale", ingredients = new[]{ItemType.Wheat, ItemType.Berries}, amounts = new[]{3,1}, result = ItemType.Ale, resultCount = 1, nutritionValue = 5, moodBonus = 15, cookTime = 10, requiredStation = "Brewery" },
            new() { name = "Wine", ingredients = new[]{ItemType.Berries}, amounts = new[]{10}, result = ItemType.Wine, resultCount = 1, nutritionValue = 5, moodBonus = 18, cookTime = 15, requiredStation = "Brewery" },
            new() { name = "Cider", ingredients = new[]{ItemType.Berries}, amounts = new[]{5}, result = ItemType.Cider, resultCount = 1, nutritionValue = 3, moodBonus = 10, cookTime = 6, requiredStation = "Brewery" },
            new() { name = "Mead", ingredients = new[]{ItemType.Wheat, ItemType.Berries}, amounts = new[]{5,3}, result = ItemType.Mead, resultCount = 1, nutritionValue = 5, moodBonus = 20, cookTime = 20, requiredStation = "Brewery" },
            // Additional recipes
            new() { name = "Steak", ingredients = new[]{ItemType.RawMeat, ItemType.MedicalHerb}, amounts = new[]{2,1}, result = ItemType.CookedMeat, resultCount = 1, nutritionValue = 50, moodBonus = 12, cookTime = 4, requiredStation = "Campfire" },
            new() { name = "Fish Fillet", ingredients = new[]{ItemType.Fish, ItemType.Bread}, amounts = new[]{2,1}, result = ItemType.CookedMeat, resultCount = 2, nutritionValue = 40, moodBonus = 8, cookTime = 3, requiredStation = "Stove" },
            new() { name = "Berry Jam", ingredients = new[]{ItemType.Berries}, amounts = new[]{5}, result = ItemType.RationPack, resultCount = 1, nutritionValue = 20, moodBonus = 8, cookTime = 6, requiredStation = "Stove" },
            new() { name = "Pemmican", ingredients = new[]{ItemType.RawMeat, ItemType.Berries}, amounts = new[]{1,2}, result = ItemType.RationPack, resultCount = 3, nutritionValue = 60, moodBonus = 2, cookTime = 8, requiredStation = "Campfire" },
            new() { name = "Cannabis Brownie", ingredients = new[]{ItemType.HempDried, ItemType.Wheat}, amounts = new[]{2,3}, result = ItemType.RationPack, resultCount = 2, nutritionValue = 25, moodBonus = 25, cookTime = 8, requiredStation = "Oven" },
            new() { name = "Oatmeal", ingredients = new[]{ItemType.Wheat, ItemType.Berries}, amounts = new[]{3,1}, result = ItemType.RationPack, resultCount = 2, nutritionValue = 35, moodBonus = 5, cookTime = 3, requiredStation = "Campfire" },
            new() { name = "Baked Potato", ingredients = new[]{ItemType.Potato}, amounts = new[]{3}, result = ItemType.CookedMeat, resultCount = 1, nutritionValue = 30, moodBonus = 3, cookTime = 4, requiredStation = "Oven" },
            new() { name = "Sushi", ingredients = new[]{ItemType.Fish, ItemType.Wheat}, amounts = new[]{1,2}, result = ItemType.RationPack, resultCount = 1, nutritionValue = 25, moodBonus = 15, cookTime = 2, requiredStation = "Stove" },
            new() { name = "Mushroom Tea", ingredients = new[]{ItemType.Mushroom}, amounts = new[]{4}, result = ItemType.RationPack, resultCount = 1, nutritionValue = 15, moodBonus = 10, cookTime = 4, requiredStation = "Brewery" },
            new() { name = "Fish Stew", ingredients = new[]{ItemType.Fish, ItemType.Potato, ItemType.Mushroom}, amounts = new[]{3,2,1}, result = ItemType.RationPack, resultCount = 3, nutritionValue = 60, moodBonus = 10, cookTime = 9, requiredStation = "Stove" },
            new() { name = "Mutton Chop", ingredients = new[]{ItemType.RawMeat}, amounts = new[]{3}, result = ItemType.CookedMeat, resultCount = 2, nutritionValue = 55, moodBonus = 8, cookTime = 5, requiredStation = "Campfire" },
            new() { name = "Roasted Boar", ingredients = new[]{ItemType.RawMeat, ItemType.Potato}, amounts = new[]{3,2}, result = ItemType.CookedMeat, resultCount = 3, nutritionValue = 80, moodBonus = 15, cookTime = 12, requiredStation = "Oven" },
            new() { name = "Hardtack", ingredients = new[]{ItemType.Wheat}, amounts = new[]{1}, result = ItemType.Bread, resultCount = 2, nutritionValue = 20, moodBonus = 1, cookTime = 2, requiredStation = "Campfire" },
            new() { name = "Honeyed Berries", ingredients = new[]{ItemType.Berries}, amounts = new[]{3}, result = ItemType.RationPack, resultCount = 1, nutritionValue = 25, moodBonus = 12, cookTime = 2, requiredStation = "Campfire" },
            new() { name = "Vegetable Soup", ingredients = new[]{ItemType.Potato, ItemType.Mushroom}, amounts = new[]{2,1}, result = ItemType.RationPack, resultCount = 2, nutritionValue = 35, moodBonus = 6, cookTime = 5, requiredStation = "Stove" },
        };
    }

    /// <summary>Try to cook a recipe. Returns true if cooking started.</summary>
    public bool TryCook(Recipe recipe, Inventory inventory, out float cookTime)
    {
        cookTime = 0;
        for (int i = 0; i < recipe.ingredients.Length; i++)
        {
            if (!inventory.Has(recipe.ingredients[i], recipe.amounts[i]))
                return false;
        }
        for (int i = 0; i < recipe.ingredients.Length; i++)
            inventory.RemoveItem(recipe.ingredients[i], recipe.amounts[i]);
        inventory.AddItem(recipe.result, recipe.resultCount);
        cookTime = recipe.cookTime;
        return true;
    }
}
