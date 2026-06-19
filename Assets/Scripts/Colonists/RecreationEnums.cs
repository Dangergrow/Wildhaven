/// <summary>
/// Types of recreation activities. Colonists need variety.
/// </summary>
public enum RecreationType
{
    Social,         // Chatting, party, telling stories
    Solitary,       // Meditation, stargazing, walking
    Physical,       // Horseshoes, sports, training dummies
    Intellectual,   // Chess, books, research (for fun)
    Music,          // Playing instruments, listening
    Relaxation,     // Sitting, napping on couch, hot bath
}

/// <summary>
/// Defines a recreation activity with its properties.
/// </summary>
[System.Serializable]
public struct RecreationActivity
{
    public string activityName;
    public RecreationType type;
    public float recreationValue;   // How much recreation it gives
    public float moodBonus;         // Extra mood from this activity
    public float duration;          // How long the activity takes (seconds)
    public bool requiresObject;     // Needs a built object (chess table, etc.)
    public string requiredObjectName;
}
