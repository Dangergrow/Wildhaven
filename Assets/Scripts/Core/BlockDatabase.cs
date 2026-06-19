using UnityEngine;

/// <summary>Holds BlockData for all block types. Created in Editor once, referenced by GridManager.</summary>
[CreateAssetMenu(fileName = "BlockDatabase", menuName = "Wildhaven/Block Database")]
public class BlockDatabase : ScriptableObject
{
    public BlockData[] blocks;

    /// <summary>Returns color for a block type, or gray if not found.</summary>
    public Color GetColor(BlockType type)
    {
        if (blocks != null)
            foreach (var b in blocks)
                if (b != null && b.blockType == type)
                    return b.tintColor;
        return Color.gray;
    }
}
