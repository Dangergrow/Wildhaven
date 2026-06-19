using UnityEngine;
using Mirror;

public class WildhavenNetworkManager : NetworkManager
{
    public override void OnStartServer() => Debug.Log("[Network] Server started");
    public override void OnStartClient() => Debug.Log("[Network] Client connected");
    public void StartHosting() { if (!NetworkServer.active && !NetworkClient.active) StartHost(); }
    public void JoinGame(string ip) { networkAddress = ip; if (!NetworkServer.active && !NetworkClient.active) StartClient(); }
}
