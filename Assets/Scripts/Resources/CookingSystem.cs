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
