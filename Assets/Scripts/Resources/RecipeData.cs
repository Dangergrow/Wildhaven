using UnityEngine;

/// <summary>
/// Defines a crafting recipe.
/// </summary>
[CreateAssetMenu(fileName = "Recipe", menuName = "Wildhaven/Recipe")]
public class RecipeData : ScriptableObject
{
    public string recipeName;
    public ItemType resultItem;
    public int resultAmount = 1;
    public float craftTime = 3f; // seconds
    public int requiredSkillLevel;

    [Header("Ingredients")]
    public Ingredient[] ingredients;

    [Header("Requirements")]
    public CraftingStationType requiredStation = CraftingStationType.None;
    public bool requiresResearch;
    public string researchName;
}

[System.Serializable]
public struct Ingredient
{
    public ItemType itemType;
    public int amount;
}

public enum CraftingStationType
{
    None,           // craft by hand
    Campfire,
    Workbench,
    Furnace,
    Anvil,
    Laboratory,
    Loom,
    Tannery,
    Brewery,
    Stonecutter,
    Carpenter,
    AlchemyTable,
    Oven,
    Smelter,
}
