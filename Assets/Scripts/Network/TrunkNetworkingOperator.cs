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
        server.RegisterHandler(NetMessage.ID.InitSession, OnInitSession);
        server.RegisterHandler(NetMessage.ID.Ping, OnPing);
        if (!server.Listen(GAME_PORT))
        {
            Restart("Unable to start a host!");
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
                Restart("Could not create broadcast!");
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
        Restart("Disconnect detected!");
    }

    public IEnumerator SetUpSession(int citySeed, int pathSeed)
    {
        // We have nothing to set up / syncronize, so just do nothing for now.
        var wfs = new WaitForSeconds(1f);
        for (int i = 0; i < 10; i++)
        {
            yield return wfs;
            Log("Setting up game: " + i * 10f + "% complete");
        }
        Log("Setting up game: Done!");
    }

    public void OnInitSession(NetworkMessage msg)
    {
        // Don't really do anything with this yet.  Just for visual purposes.
        NetMessage.InitSessionMsg castedMsg = msg.ReadMessage<NetMessage.InitSessionMsg>();
        StartCoroutine(SetUpSession(castedMsg.citySeed, castedMsg.pathSeed));
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

    public void OnPing(NetworkMessage msg)
    {
        NetMessage.PingMsg castedMsg = msg.ReadMessage<NetMessage.PingMsg>();
        Log("Ping! " + castedMsg.msg);

        NetMessage.PingMsg response = new NetMessage.PingMsg();
        response.msg = "Hello to you too!";
        msg.conn.Send(NetMessage.ID.Ping, response);
    }

    public void OnDestroy()
    {
        if (server != null)
        {
            server.Stop();
            server = null;
        }
    }
}
