using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(TrunkNetworkDiscovery))]
public class TrunkNetworkingHostage : MonoBehaviour
{
    TrunkNetworkDiscovery broadcaster = null;
    NetworkClient network = null;

    public UnityEngine.UI.Text statusText = null;

    public UnityEvent OnSessionEstablished;

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
            Restart("Unable to listen for host on network!");
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

    public IEnumerator SetUpSession(int citySeed, int pathSeed)
    {
        // We have nothing to set up / syncronize, so just do nothing for now.
        var wfs = new WaitForSeconds(1f);
        for (int i = 0; i < 10; i++ )
        {
            yield return wfs;
            Log("Setting up game: " + i * 10f + "% complete");
        }
        Log("Setting up game: Done!");
        OnSessionEstablished.Invoke();
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
        Restart("Disconnect detected!");
    }

    public void OnPing(NetworkMessage msg)
    {
        NetMessage.PingMsg castedMsg = msg.ReadMessage<NetMessage.PingMsg>();
        Log("Ping! " + castedMsg.msg);

        NetMessage.InitSessionMsg initMsg = new NetMessage.InitSessionMsg();
        initMsg.citySeed = Random.seed;
        initMsg.pathSeed = Random.Range(int.MinValue, int.MaxValue);
        msg.conn.Send(NetMessage.ID.InitSession, initMsg);

        StartCoroutine(SetUpSession(initMsg.citySeed, initMsg.pathSeed));
    }

    public void Restart(string msg)
    {
        StopAllCoroutines();
        StartCoroutine(RestartingIn(msg));
    }

    public IEnumerator RestartingIn(string msg)
    {
        Log(msg + "  Resetting in...", true);
        for (int sec = 5; sec > 0; sec--)
        {
            Log(sec + "...");
            yield return new WaitForSeconds(1f);
        }
        SceneManager.LoadScene("Trunk", LoadSceneMode.Single);
    }

    public void OnDestroy()
    {
        if (network != null)
        {
            network.Disconnect();
            network = null;
        }
    }
}
