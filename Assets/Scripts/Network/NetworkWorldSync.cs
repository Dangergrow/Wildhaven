using UnityEngine;
using Mirror;

[RequireComponent(typeof(NetworkIdentity))]
public class NetworkWorldSync : NetworkBehaviour
{
    private GridManager _grid;

    void Awake()
    {
        _grid = GetComponent<GridManager>();
        if (_grid != null) _grid.OnBlockChanged += OnBlockChanged;
    }
    void OnDestroy() { if (_grid != null) _grid.OnBlockChanged -= OnBlockChanged; }

    void OnBlockChanged(int x, int y, int z)
    {
        if (!isServer)
            CmdBlockUpdate(x, y, z, (byte)_grid.GetBlock(x, y, z));
        else
            RpcBlockUpdate(x, y, z, (byte)_grid.GetBlock(x, y, z));
    }

    [Command(requiresAuthority = false)]
    void CmdBlockUpdate(int x, int y, int z, byte b)
    {
        BlockType t = (BlockType)b;
        if (t == BlockType.Air) _grid.RemoveBlock(x, y, z);
        else _grid.SetBlock(x, y, z, t);
        RpcBlockUpdate(x, y, z, b);
    }

    [ClientRpc]
    void RpcBlockUpdate(int x, int y, int z, byte b)
    {
        if (isServer) return;
        BlockType t = (BlockType)b;
        if (_grid.GetBlock(x, y, z) == t) return;
        if (t == BlockType.Air) _grid.RemoveBlock(x, y, z);
        else _grid.SetBlock(x, y, z, t);
    }
}
