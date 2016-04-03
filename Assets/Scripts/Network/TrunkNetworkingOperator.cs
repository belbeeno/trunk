using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(TrunkNetworkDiscovery))]
public class TrunkNetworkingOperator : TrunkNetworkingBase
{
    protected static TrunkNetworkingOperator _instance = null;
    public static TrunkNetworkingOperator Get() 
    {
        if (_instance == null) _instance = FindObjectOfType<TrunkNetworkingOperator>(); // Shouldn't happen but just in case...
        return _instance;  
    }

    public static int GAME_PORT = 7777;

    public HostTopology topology;
    protected TrunkNetworkingServer server = null;
    protected int clientId = -1;
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
        server.RegisterHandler(NetMessage.ID.APB, OnAPBResponse);
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

    public void RequestAPB(Vector3 pos)
    {
        if (server == null) return;

        NetworkConnection client = server.FindConnection(clientId);
        if (client != null)
        {
            Log("Requesting APB at pos " + pos.ToString());
            NetMessage.APBRequest msg = new NetMessage.APBRequest();
            msg.position = pos;
            client.Send(NetMessage.ID.APB, msg);
        }
        else
        {
            Debug.LogWarning("Attempting to find connection for clientId " + clientId + " but got nothin!");
        }
    }

    public void OnAPBResponse(NetworkMessage msg)
    {
        NetMessage.APBResponse castedMsg = msg.ReadMessage<NetMessage.APBResponse>();
        Log("Attempting to find hostage at position " + castedMsg.origin);
        for (int i = 0; i < castedMsg.hints.Count; i++)
        {
            if (castedMsg.hints[i].type == NetMessage.APBResponse.Hint.HintType.Hostage)
            {
                Log("Hostage found!  You win!  You can stop playing now.");
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
        clientId = msg.conn.connectionId;
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

    public void Start()
    {
        _instance = this;
    }

    public void OnDestroy()
    {
        if (server != null)
        {
            server.Stop();
            server = null;
        }

        _instance = null;
    }
}
