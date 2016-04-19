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
    public TrunkNetworkingServer Server
    {
        get { return server;  }
    }
    protected int clientId = -1;
    protected TrunkNetworkDiscovery broadcaster = null;

    public override int VoiceChatID
    {
        get { return 2; }
    }
    public override void OnNewSampleCaptured(VoiceChat.VoiceChatPacket packet)
    {
        if (server == null) return;

        NetworkConnection client = server.FindConnection(clientId);
        if (client != null)
        {
            //Debug.Log("New sample captured: " + packet.PacketId);
            NetMessage.VoiceChatMsg msg = new NetMessage.VoiceChatMsg(packet);
            client.SendUnreliable(NetMessage.ID.VoiceChatPacket, msg);
        }
    }

    public override void Begin() 
    {
        server = new TrunkNetworkingServer();
        //server.Configure(topology);
        server.Initialize();
        server.RegisterHandler(MsgType.Connect, OnConnectMsg);
        server.RegisterHandler(MsgType.Disconnect, OnDisconnectMsg);
        server.RegisterHandler(NetMessage.ID.InitSession, OnInitSessionMsg);
        server.RegisterHandler(NetMessage.ID.Ping, OnPingMsg);
        server.RegisterHandler(NetMessage.ID.APB, OnAPBResponseMsg);
        server.RegisterHandler(NetMessage.ID.GameOver, OnGameOverMsg);

        server.RegisterHandler(NetMessage.ID.VoiceChatPacket, OnVoiceChatMsg);

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

#if UNITY_EDITOR
        Debug.DrawRay(pos, Vector3.forward * GameSettings.APB_RADIUS, Color.red, 5f);
        Debug.DrawRay(pos, Vector3.back * GameSettings.APB_RADIUS, Color.red, 5f);
        Debug.DrawRay(pos, Vector3.left * GameSettings.APB_RADIUS, Color.red, 5f);
        Debug.DrawRay(pos, Vector3.right * GameSettings.APB_RADIUS, Color.red, 5f);

        Vector3 prevPos = new Vector3(Mathf.Cos(0f) * GameSettings.APB_RADIUS, 0f, Mathf.Sin(0f) * GameSettings.APB_RADIUS);
        Vector3 nextPos = new Vector3();
        for (float i = 1f; i < 10f; i += 1f)
        {
            nextPos.x = Mathf.Cos((i / 10f) * Mathf.PI * 2f) * GameSettings.APB_RADIUS;
            nextPos.z = Mathf.Sin((i / 10f) * Mathf.PI * 2f) * GameSettings.APB_RADIUS;

            Debug.DrawLine(pos + prevPos, pos + nextPos, Color.red, 5f, false);

            prevPos = nextPos;
        }
        nextPos.Set(Mathf.Cos(0f), 0f, Mathf.Sin(0f));
        Debug.DrawLine(prevPos, nextPos * GameSettings.APB_RADIUS, Color.red, 5f, false);
#endif

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

    public void OnAPBResponseMsg(NetworkMessage msg)
    {
        NetMessage.APBResponse castedMsg = msg.ReadMessage<NetMessage.APBResponse>();
        Log("Attempting to find hostage at position " + castedMsg.origin);
        for (int i = 0; i < castedMsg.hints.Count; i++)
        {
            if (castedMsg.hints[i].type == NetMessage.APBResponse.Hint.HintType.Hostage)
            {
                Log("Hostage found!  You win!");
                OnGameWin.Invoke();
                Restart();

                NetMessage.GameOverMsg gameOverMsg = new NetMessage.GameOverMsg();
                gameOverMsg.timestamp = Network.time;
                msg.conn.Send(NetMessage.ID.GameOver, gameOverMsg);
            }
            else
            {
                string hintType = NetMessage.APBResponse.Hint.TypeToName(castedMsg.hints[i].type);
                Log(hintType + " found at position: " + castedMsg.hints[i].pos.ToString());
                OperatorIcons.NewIcon(castedMsg.hints[i].pos, castedMsg.hints[i].type);
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

    public void OnConnectMsg(NetworkMessage msg)
    {
        Log("New player connected!");
        broadcaster.StopBroadcast();
        clientId = msg.conn.connectionId;
    }

    public void OnDisconnectMsg(NetworkMessage msg)
    {
        Restart("Disconnect detected!");
    }

    public void OnInitSessionMsg(NetworkMessage msg)
    {
        // Don't really do anything with this yet.  Just for visual purposes.
        NetMessage.InitSessionMsg castedMsg = msg.ReadMessage<NetMessage.InitSessionMsg>();
        SetUpSession(castedMsg.citySeed, castedMsg.pathSeed);
    }

    public void OnPingMsg(NetworkMessage msg)
    {
        NetMessage.PingMsg castedMsg = msg.ReadMessage<NetMessage.PingMsg>();
        Log("Ping! " + castedMsg.msg);

        NetMessage.PingMsg response = new NetMessage.PingMsg();
        response.msg = "Hello to you too!";
        msg.conn.Send(NetMessage.ID.Ping, response);
    }

    public void OnGameOverMsg(NetworkMessage msg)
    {
        // If we're getting this from the client, the captors are out of range and we lost.
        Log("You took too long, the captors won!");
        Restart();
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
