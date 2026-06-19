using UnityEngine;
using Mirror;

/// <summary>Simple host/join UI for multiplayer testing.</summary>
public class NetworkUI : MonoBehaviour
{
    private WildhavenNetworkManager _nm;
    private string _ip = "localhost";
    private bool _show;

    void Start()
    {
        _nm = FindObjectOfType<WildhavenNetworkManager>();
        if (_nm == null)
            _nm = gameObject.AddComponent<WildhavenNetworkManager>();
    }

    void Update()
    {
        if (Keyboard.current == null) return;
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            _show = !_show;
    }

    void OnGUI()
    {
        if (!_show) return;

        GUILayout.BeginArea(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 80, 200, 160));
        GUILayout.Box("Multiplayer");

        if (!NetworkClient.active && !NetworkServer.active)
        {
            if (GUILayout.Button("Host Game")) _nm.StartHosting();
            GUILayout.Label("Join IP:");
            _ip = GUILayout.TextField(_ip);
            if (GUILayout.Button("Join Game")) _nm.JoinGame(_ip);
        }
        else
        {
            GUILayout.Label(NetworkServer.active ? "Server running" : "Client connected");
            if (GUILayout.Button("Stop")) _nm.StopHost();
        }
        GUILayout.EndArea();
    }
}
