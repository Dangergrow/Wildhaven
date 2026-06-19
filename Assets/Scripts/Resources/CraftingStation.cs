using UnityEngine;

/// <summary>
/// Represents a crafting station in the world.
/// </summary>
public class CraftingStation : MonoBehaviour
{
    public CraftingStationType stationType;
    public Inventory inputInventory;  // where ingredients go
    public Inventory outputInventory; // where products go

    [Header("Active Recipe")]
    public RecipeData currentRecipe;
    public float craftProgress;
    public bool isCrafting;

    [Header("Orders")]
    public CraftOrder[] orders; // up to 5
    public int currentOrderIndex = -1;

    private void Update()
    {
        if (!isCrafting && currentOrderIndex >= 0 && currentOrderIndex < orders.Length)
        {
            StartCrafting(orders[currentOrderIndex].recipe);
        }

        if (isCrafting)
        {
            craftProgress += Time.deltaTime;
            if (craftProgress >= currentRecipe.craftTime)
            {
                CompleteCraft();
            }
        }
    }

    /// <summary>
    /// Begins crafting a recipe if ingredients are available.
    /// </summary>
    public bool StartCrafting(RecipeData recipe)
    {
        if (!HasIngredients(recipe)) return false;

        currentRecipe = recipe;
        craftProgress = 0f;

        // Consume ingredients
        foreach (Ingredient ing in recipe.ingredients)
            inputInventory.RemoveItem(ing.itemType, ing.amount);

        isCrafting = true;
        return true;
    }

    /// <summary>
    /// Completes the crafting process.
    /// </summary>
    private void CompleteCraft()
    {
        if (outputInventory != null)
            outputInventory.AddItem(currentRecipe.resultItem, currentRecipe.resultAmount);

        isCrafting = false;
        currentRecipe = null;
        craftProgress = 0f;

        // Move to next order
        currentOrderIndex++;
        if (currentOrderIndex >= orders.Length || orders[currentOrderIndex].recipe == null)
            currentOrderIndex = -1;
    }

    /// <summary>
    /// Checks if all ingredients are available.
    /// </summary>
    private bool HasIngredients(RecipeData recipe)
    {
        if (inputInventory == null) return false;
        foreach (Ingredient ing in recipe.ingredients)
            if (!inputInventory.Has(ing.itemType, ing.amount)) return false;
        return true;
    }
}

/// <summary>
/// Crafting order — what to make and how many.
/// </summary>
[System.Serializable]
public class CraftOrder
{
    public RecipeData recipe;   // NULL = stop processing orders
    public int count = 1;       // how many times to craft
    public int completed;
}
