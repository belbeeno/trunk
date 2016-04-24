using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections;

using NetMessage;

[RequireComponent(typeof(TrunkNetworkDiscovery))]
public class TrunkNetworkingOperator : TrunkNetworkingBase
{
    protected static TrunkNetworkingOperator _instance = null;
    public static TrunkNetworkingOperator Get() 
    {
        return _instance;  
    }
    public override bool IsHost() { return true; }

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
            VoiceChatMsg msg = new VoiceChatMsg(packet);
            client.SendUnreliable(ID.VoiceChatPacket, msg);
        }
    }

    public override void Begin() 
    {
        server = new TrunkNetworkingServer();
        //server.Configure(topology);
        server.Initialize();
        base.Begin();
        for (int i = 0; i < initParams.Count; i++ )
        {
            server.RegisterHandler(initParams[i].msgId, initParams[i].message);
        }

        server.RegisterHandler(MsgType.Connect, OnConnectMsg);
        server.RegisterHandler(ID.APB, OnAPBResponseMsg);
        server.RegisterHandler(ID.GameOver, OnGameOverMsg);

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
    public void Start()
    {
        _instance = this;
        _baseInstance = this;
    }
    public void Update()
    {
        if (server != null)
        {
            if (GameManager.Get().LocalStatus == GameManager.PlayerStatus.LoadingFailed
                || GameManager.Get().RemoteStatus == GameManager.PlayerStatus.LoadingFailed)
            {
                Log("Loading failed!  Random seed validation step failed!");
                StartReloading();
            }

            server.Update();
        }
    }
    public override void OnDestroy()
    {
        if (server != null)
        {
            server.DisconnectAllConnections();
            server.Stop();
            server = null;
        }

        if (_instance == this)
        {
            _instance = null;
        }

        base.OnDestroy();
    }

    public override void SendMessage(short msgId, MessageBase msg)
    {
        NetworkConnection client = server.FindConnection(clientId);
        if (client != null)
        {
            client.Send(msgId, msg);
        }
        else
        {
            Debug.LogWarning("Connection for clientId " + clientId + " for message " + msgId + "not found.");
        }
    }

    public void StartReloading()
    {
        SeedMsg initMsg = new SeedMsg();
        initMsg.seed = Random.seed;
        SendMessage(ID.LoadSession, initMsg);

        GameManager.Get().SetUpGame(initMsg.seed, SendValidateMessageMsg);
    }
    private void SendValidateMessageMsg()
    {
        SeedMsg msg = new SeedMsg();
        msg.seed = Random.seed;
        SendMessage(ID.ValidateSession, msg);
    }

    public void RequestAPB(Vector3 pos)
    {
        if (server == null) return;

#if UNITY_EDITOR
        if (Debug.isDebugBuild)
        {
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
        }
#endif

        Log("Requesting APB at pos " + pos.ToString());
        APBRequest msg = new APBRequest();
        msg.position = pos;
        SendMessage(ID.APB, msg);
    }
    public void OnAPBResponseMsg(NetworkMessage msg)
    {
        APBResponse castedMsg = msg.ReadMessage<APBResponse>();
        Log("Attempting to find hostage at position " + castedMsg.origin);
        for (int i = 0; i < castedMsg.hints.Count; i++)
        {
            if (castedMsg.hints[i].type == APBResponse.Hint.HintType.Hostage)
            {
                Log("Hostage found!  You win!");
                OnGameWin.Invoke();
                Restart();

                GameOverMsg gameOverMsg = new GameOverMsg();
                gameOverMsg.timestamp = Network.time;
                msg.conn.Send(ID.GameOver, gameOverMsg);
            }
            else
            {
                string hintType = APBResponse.Hint.TypeToName(castedMsg.hints[i].type);
                Log(hintType + " found at position: " + castedMsg.hints[i].pos.ToString());
                OperatorIcons.NewIcon(castedMsg.hints[i].pos, castedMsg.hints[i].type);
            }
        }
    }

    public void TriggerHelicopterInHostageScene(float y, bool isRight)
    {
        Log("Requesting Helicoptor at y " + y.ToString());
        TriggerHelicopterMsg msg = new TriggerHelicopterMsg();
        msg.goingRight = isRight;
        msg.yPos = y;
        SendMessage(ID.TriggerHelicopter, msg);
    }

    public void TriggerPoliceInHostageScene(Vector2 pos)
    {
        Log("Requesting Police car at pos " + pos.ToString());
        TriggerPoliceMsg msg = new TriggerPoliceMsg();
        msg.position = pos;
        SendMessage(ID.TriggerPoliceCar, msg);
    }

    public void OnConnectMsg(NetworkMessage msg)
    {
        Log("New player connected!");
        broadcaster.StopBroadcast();
        clientId = msg.conn.connectionId;

        SeedMsg initMsg = new SeedMsg();
        initMsg.seed = Random.seed;
        SendMessage(ID.LoadSession, initMsg);

        SetUpSession(initMsg.seed);
    }

    public void OnGameOverMsg(NetworkMessage msg)
    {
        // If we're getting this from the client, the captors are out of range and we lost.
        Log("You took too long, the captors won!");
        Restart();
    }

}
