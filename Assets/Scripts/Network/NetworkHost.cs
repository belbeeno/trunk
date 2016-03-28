using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class NetworkHost : MonoBehaviour 
{
    public int port = 5555;
    public string password = "TrunkPW";

    public void Start()
    {
        Network.incomingPassword = password;
        Network.InitializeServer(1, port, false);
        DebugConsole.SetText("(Host) IPAddress", Network.player.ipAddress + ":" + Network.player.port);
    }
}
