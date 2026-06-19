using UnityEngine;
using Mirror;

/// <summary>
/// Syncs block changes across network.
/// Attach to World GameObject alongside GridManager and NetworkIdentity.
/// </summary>
[RequireComponent(typeof(NetworkIdentity))]
public class NetworkWorldSync : NetworkBehaviour
{
    private GridManager _grid;

    void Awake()
    {
        _grid = GetComponent<GridManager>();
        if (_grid != null)
            _grid.OnBlockChanged += OnLocalBlockChanged;
    }

    void OnDestroy()
    {
        if (_grid != null)
            _grid.OnBlockChanged -= OnLocalBlockChanged;
    }

    // Called from GridManager.OnBlockChanged after any SetBlock/RemoveBlock
    public void OnLocalBlockChanged(int x, int y, int z)
    {
        if (!isServer)
        {
            // Client: send request to server
            CmdBlockUpdate(x, y, z, (byte)_grid.GetBlock(x, y, z));
        }
        else
        {
            // Server: broadcast to all clients
            RpcBlockUpdate(x, y, z, (byte)_grid.GetBlock(x, y, z));
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdBlockUpdate(int x, int y, int z, byte blockTypeByte)
    {
        BlockType type = (BlockType)blockTypeByte;
        if (type == BlockType.Air)
            _grid.RemoveBlock(x, y, z);
        else
            _grid.SetBlock(x, y, z, type);
        // Broadcast to all clients (including sender, for confirmation)
        RpcBlockUpdate(x, y, z, blockTypeByte);
    }

    [ClientRpc]
    public void RpcBlockUpdate(int x, int y, int z, byte blockTypeByte)
    {
        if (isServer) return;
        BlockType type = (BlockType)blockTypeByte;
        if (_grid.GetBlock(x, y, z) == type) return; // already correct
        if (type == BlockType.Air)
            _grid.RemoveBlock(x, y, z);
        else
            _grid.SetBlock(x, y, z, type);
    }

    /// <summary>Server sends full world to newly connected client.</summary>
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (isServer) return;
        // Request full world sync
        CmdRequestFullSync();
    }

    [Command(requiresAuthority = false)]
    void CmdRequestFullSync(NetworkConnectionToClient conn = null)
    {
        // Send entire world to the requesting client
        for (int x = 0; x < _grid.Width; x++)
        for (int y = 0; y < _grid.Height; y++)
        for (int z = 0; z < _grid.Depth; z++)
        {
            BlockType type = _grid.GetBlock(x, y, z);
            if (type != BlockType.Air)
                TargetFullSyncBlock(conn, x, y, z, (byte)type);
        }
        Debug.Log($"[Network] World sync complete");
    }

    [TargetRpc]
    void TargetFullSyncBlock(NetworkConnectionToClient conn, int x, int y, int z, byte blockTypeByte)
    {
        _grid.SetBlock(x, y, z, (BlockType)blockTypeByte);
    }
}
