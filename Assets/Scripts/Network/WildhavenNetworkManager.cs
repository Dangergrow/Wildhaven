using UnityEngine;
using Mirror;

/// <summary>
/// Custom NetworkManager for Wildhaven multiplayer.
/// Host = server + client, up to 3 players.
/// </summary>
public class WildhavenNetworkManager : NetworkManager
{
    [Header("Wildhaven")]
    public int maxPlayers = 3;
    public GameObject worldPrefab;

    public override void OnStartServer()
    {
        base.OnStartServer();
        maxConnections = maxPlayers;
        Debug.Log($"[Network] Server started. Max players: {maxPlayers}");
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("[Network] Client connected");
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Debug.Log($"[Network] Player joined: {conn.connectionId}");
        base.OnServerAddPlayer(conn);
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        Debug.Log($"[Network] Player left: {conn.connectionId}");
        base.OnServerDisconnect(conn);
    }

    /// <summary>Called from UI to start hosting.</summary>
    public void StartHosting()
    {
        if (NetworkServer.active || NetworkClient.active) return;
        StartHost();
    }

    /// <summary>Called from UI to join a game.</summary>
    public void JoinGame(string ip)
    {
        if (NetworkServer.active || NetworkClient.active) return;
        networkAddress = ip;
        StartClient();
    }
}
