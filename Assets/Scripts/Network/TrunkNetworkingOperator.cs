using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(TrunkNetworkDiscovery))]
public class TrunkNetworkingOperator : MonoBehaviour
{
    public static int GAME_PORT = 7777;

    public HostTopology topology;
    protected TrunkNetworkingServer server = null;
    protected TrunkNetworkDiscovery broadcaster = null;

    public UnityEngine.UI.Text statusText = null;

    public void Log(string msg, bool asError = false)
    {
        statusText.text += "\n" + (asError ? "<color=\"red\">ERROR: " + msg + "</color>" : msg);
            
        DebugConsole.SetText("NetworkStatus", msg);
    }

    public void Begin() 
    {
        server = new TrunkNetworkingServer();
        //server.Configure(topology);
        server.Initialize();
        server.RegisterHandler(MsgType.Connect, OnConnect);
        server.RegisterHandler(MsgType.Disconnect, OnDisconnect);
        server.RegisterHandler(NetMessage.ID.Ping, OnPing);
        if (!server.Listen(GAME_PORT))
        {
            Log("Unable to start a host!", true);
        }
        else
        {
            Log("Server started, listening on port " + GAME_PORT + "...");
            broadcaster = GetComponent<TrunkNetworkDiscovery>();
            if (broadcaster.Initialize() && broadcaster.StartAsServer())
            {
                Log("Beginning broadcast...");
            }
            else
            {
                Log("Could not create broadcast!", true);
            }
        }
    }

    public void Update()
    {
        if (server != null)
        {
            server.Update();
        }
    }

    public void OnConnect(NetworkMessage msg)
    {
        Log("New player connected!");
        broadcaster.StopBroadcast();
    }

    public void OnDisconnect(NetworkMessage msg)
    {
        StopAllCoroutines();
        StartCoroutine(RestartingIn("Player disconnected!"));
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

    public void OnPing(NetworkMessage msg)
    {
        NetMessage.PingMsg castedMsg = msg.ReadMessage<NetMessage.PingMsg>();
        Log("Ping! " + castedMsg.msg);

        NetMessage.PingMsg response = new NetMessage.PingMsg();
        response.msg = "Hello to you too!";
        msg.conn.Send(NetMessage.ID.Ping, response);
    }

    public void OnApplicationQuit()
    {
        if (server != null)
        {
            server.Stop();
        }
    }

}
