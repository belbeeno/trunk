using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(TrunkNetworkDiscovery))]
public class TrunkNetworkingHostage : MonoBehaviour
{
    TrunkNetworkDiscovery broadcaster = null;
    NetworkClient network = null;

    public UnityEngine.UI.Text statusText = null;

    public void Log(string msg, bool asError = false)
    {
        statusText.text += "\n" + (asError ? "<color=\"red\">ERROR:</color> " : string.Empty) + msg;

        DebugConsole.SetText("NetworkStatus", msg);
    }
    
    public void Begin()
    {
        broadcaster = GetComponent<TrunkNetworkDiscovery>();
        broadcaster.Initialize();
        if (!broadcaster.StartAsClient(Connect))
        {
            Log("Unable to listen for host on network!", true);
            return;
        }
        Log("Listening for a host!");
    }

    public void Connect(string ip)
    {
        network = new NetworkClient();
        network.RegisterHandler(MsgType.Connect, OnConnect);
        network.RegisterHandler(MsgType.Disconnect, OnDisconnect);
        network.RegisterHandler(NetMessage.ID.Ping, OnPing);
        network.Connect(ip, TrunkNetworkingOperator.GAME_PORT);
    }

    public void OnConnect(NetworkMessage msg)
    {
        Log("Connected to server! " + msg.ToString());
        NetMessage.PingMsg ping = new NetMessage.PingMsg();
        ping.msg = "Hi!";
        msg.conn.Send(NetMessage.ID.Ping, ping);

        broadcaster.StopBroadcast();
    }

    public void OnDisconnect(NetworkMessage msg)
    {
        StopAllCoroutines();
        StartCoroutine(RestartingIn("Disconnect detected!"));
    }

    public void OnPing(NetworkMessage msg)
    {
        NetMessage.PingMsg castedMsg = msg.ReadMessage<NetMessage.PingMsg>();
        Log("Ping! " + castedMsg.msg);
    }

    public IEnumerator RestartingIn(string msg)
    {
        Log(msg + "  Resetting in...");
        for (int sec = 5; sec > 0; sec--)
        {
            Log(sec + "...");
            yield return new WaitForSeconds(1f);
        }
        SceneManager.LoadScene("Trunk", LoadSceneMode.Single);
    }
}
