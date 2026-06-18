using UnityEngine;

/// <summary>
/// ScriptableObject containing all properties for a specific block type.
/// </summary>
[CreateAssetMenu(fileName = "BlockData", menuName = "Wildhaven/Block Data")]
public class BlockData : ScriptableObject
{
    [Tooltip("Block type identifier")]
    public BlockType blockType;

    [Tooltip("Display name in UI")]
    public string blockName;

    [Tooltip("Is this block solid (can collide)")]
    public bool isSolid;

    [Tooltip("Is this block transparent")]
    public bool isTransparent;

    [Tooltip("Hardness: how many hits to destroy (0 = instant)")]
    public float hardness;

    [Tooltip("Required tool type to mine efficiently. 0 = hand, 1 = pickaxe, 2 = axe, 3 = shovel")]
    public int requiredToolType;

    [Tooltip("Icon for UI")]
    public Sprite icon;

    [Tooltip("Color tint for block faces")]
    public Color tintColor;
}
