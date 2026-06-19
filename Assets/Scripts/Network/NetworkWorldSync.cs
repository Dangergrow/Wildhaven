using UnityEngine;
using Mirror;

[RequireComponent(typeof(NetworkIdentity))]
public class NetworkWorldSync : NetworkBehaviour
{
    private GridManager _grid;

    void Awake()
    {
        _grid = GetComponent<GridManager>();
        if (_grid != null) _grid.OnBlockChanged += OnLocalBlockChanged;
    }
    void OnDestroy() { if (_grid != null) _grid.OnBlockChanged -= OnLocalBlockChanged; }

    void OnLocalBlockChanged(int x, int y, int z)
    {
        if (!isServer)
            CmdBlockUpdate(x, y, z, (byte)_grid.GetBlock(x, y, z));
        else
            RpcBlockUpdate(x, y, z, (byte)_grid.GetBlock(x, y, z));
    }

    [Command(requiresAuthority = false)]
    public void CmdBlockUpdate(int x, int y, int z, byte typeByte)
    {
        BlockType t = (BlockType)typeByte;
        if (t == BlockType.Air) _grid.RemoveBlock(x, y, z);
        else _grid.SetBlock(x, y, z, t);
        RpcBlockUpdate(x, y, z, typeByte);
    }

    [ClientRpc]
    public void RpcBlockUpdate(int x, int y, int z, byte typeByte)
    {
        if (isServer) return;
        BlockType t = (BlockType)typeByte;
        if (_grid.GetBlock(x, y, z) == t) return;
        if (t == BlockType.Air) _grid.RemoveBlock(x, y, z);
        else _grid.SetBlock(x, y, z, t);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!isServer) CmdRequestFullSync();
    }

    [Command(requiresAuthority = false)]
    void CmdRequestFullSync(NetworkConnectionToClient conn = null)
    {
        for (int x = 0; x < _grid.Width; x++)
        for (int y = 0; y < _grid.Height; y++)
        for (int z = 0; z < _grid.Depth; z++)
        {
            BlockType t = _grid.GetBlock(x, y, z);
            if (t != BlockType.Air)
                TargetFullSyncBlock(conn, x, y, z, (byte)t);
        }
    }

    [TargetRpc]
    void TargetFullSyncBlock(NetworkConnectionToClient conn, int x, int y, int z, byte typeByte)
    {
        _grid.SetBlock(x, y, z, (BlockType)typeByte);
    }
}
