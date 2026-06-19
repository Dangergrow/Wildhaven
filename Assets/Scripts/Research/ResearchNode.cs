/// <summary>
/// Defines a research node in the tech tree.
/// </summary>
[System.Serializable]
public class ResearchNode
{
    public string id;           // unique identifier
    public string name;         // display name
    public string description;
    public int era;             // 1-5
    public int scienceCost;     // science points needed
    public string[] prerequisiteIds; // research IDs needed before this one
    public ItemType[] resourceCosts; // items needed (paired with amounts)
    public int[] resourceAmounts;
    public bool isCompleted;
    public bool isAvailable; // visible in tree
    public float progress;   // 0 to scienceCost

    /// <summary>Returns completion percentage (0-1).</summary>
    public float Progress => scienceCost > 0 ? Mathf.Clamp01(progress / scienceCost) : 0f;
}

/// <summary>
/// Era names for the tech tree.
/// </summary>
public enum TechEra
{
    Survival = 1,    // Эпоха 1
    Settlement = 2,  // Эпоха 2
    Development = 3, // Эпоха 3
    Industry = 4,    // Эпоха 4
    Progress = 5,    // Эпоха 5
}
