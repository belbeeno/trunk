using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(TrunkNetworkDiscovery))]
public class TrunkNetworkingOperator : TrunkNetworkingBase
{
    public static int GAME_PORT = 7777;

    public HostTopology topology;
    protected TrunkNetworkingServer server = null;
    protected TrunkNetworkDiscovery broadcaster = null;

    public override void Begin() 
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

    public void OnInitSession(NetworkMessage msg)
    {
        // Don't really do anything with this yet.  Just for visual purposes.
        NetMessage.InitSessionMsg castedMsg = msg.ReadMessage<NetMessage.InitSessionMsg>();
        StartCoroutine(SetUpSession(castedMsg.citySeed, castedMsg.pathSeed));
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
