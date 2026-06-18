/// <summary>Represents a single cell in the voxel grid.</summary>
public struct GridCell
{
    public BlockType blockType;
    public byte health;

    public bool IsEmpty => blockType == BlockType.Air;

    public static readonly GridCell Empty = new GridCell { blockType = BlockType.Air, health = 0 };

    public GridCell(BlockType type, byte health = 100)
    {
        this.blockType = type;
        this.health = health;
    }
}
