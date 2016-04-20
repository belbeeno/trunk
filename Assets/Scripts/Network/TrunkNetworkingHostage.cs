using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(TrunkNetworkDiscovery))]
public class TrunkNetworkingHostage : TrunkNetworkingBase
{
    protected static TrunkNetworkingHostage _instance = null;
    public static TrunkNetworkingHostage Get()
    {
        if (_instance == null) _instance = FindObjectOfType<TrunkNetworkingHostage>();  // Shouldn't happen but just in case...
        return _instance;
    }

    TrunkNetworkDiscovery broadcaster = null;
    NetworkClient network = null;

    [SerializeField]
    TrunkMover mover = null;
    [SerializeField]
    Outside outsideTrunk = null;

    public override int VoiceChatID
    {
        get { return 1; }
    }
    public override void OnNewSampleCaptured(VoiceChat.VoiceChatPacket packet)
    {
        if (network == null) return;

        //Debug.Log("New sample captured: " + packet.PacketId);
        NetMessage.VoiceChatMsg msg = new NetMessage.VoiceChatMsg(packet);
        network.SendUnreliable(NetMessage.ID.VoiceChatPacket, msg);
    }
    
    public override void Begin()
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
        network.RegisterHandler(MsgType.Connect, OnConnectMsg);
        network.RegisterHandler(MsgType.Disconnect, OnDisconnectMsg);
        network.RegisterHandler(NetMessage.ID.Ping, OnPingMsg);
        network.RegisterHandler(NetMessage.ID.Ready, OnReadyMsg);
        network.RegisterHandler(NetMessage.ID.APB, OnAPBRequestMsg);
        network.RegisterHandler(NetMessage.ID.GameOver, OnGameOverMsg);

        network.RegisterHandler(NetMessage.ID.VoiceChatPacket, OnVoiceChatMsg);

        network.Connect(ip, TrunkNetworkingOperator.GAME_PORT);
    }

    public void OnConnectMsg(NetworkMessage msg)
    {
        Log("Connected to server! " + msg.ToString());

        NetMessage.PingMsg ping = new NetMessage.PingMsg();
        ping.msg = "Hi!";
        msg.conn.Send(NetMessage.ID.Ping, ping);

        broadcaster.StopBroadcast();
    }

    public void OnDisconnectMsg(NetworkMessage msg)
    {
        Restart("Disconnect detected!");
    }

    public void OnPingMsg(NetworkMessage msg)
    {
        NetMessage.PingMsg castedMsg = msg.ReadMessage<NetMessage.PingMsg>();
        Log("Ping! " + castedMsg.msg);

        NetMessage.InitSessionMsg initMsg = new NetMessage.InitSessionMsg();
        initMsg.seed = Random.seed;
        msg.conn.Send(NetMessage.ID.InitSession, initMsg);

        SetUpSession(initMsg.seed, SendReadyMsg);
    }
    
    private void SendReadyMsg()
    {
        if (network != null)
        {
            Log("Informing other player we're ready to start");
            NetMessage.ReadyMsg msg = new NetMessage.ReadyMsg();
            msg.seed = Random.seed;
            network.Send(NetMessage.ID.Ready, msg);
        }
        else
        {
            Debug.LogWarning("Network is null!");
        }
    }
    
    private void OnReadyMsg(NetworkMessage msg)
    {
        var gameObj = GameObject.Find("GameManager");
        var manager = gameObj.GetComponent<GameManager>();
        
        Log("Other player is ready!");
        manager.MarkOtherReady();
    }

    public void OnGameOverMsg(NetworkMessage msg)
    {
        // If we're getting this at the hostage end, then the operator found us!

        Log("Hostage found!  You win!");
        OnGameWin.Invoke();
        Restart();
    }

    public void OnAPBRequestMsg(NetworkMessage msg)
    {
        NetMessage.APBRequest castedMsg = msg.ReadMessage<NetMessage.APBRequest>();
        Log("APB requested at position " + castedMsg.position.ToString());

        // Will be needed for hints.
        //Physics.CheckSphere(castedMsg.position, GameSettings.APB_RADIUS, LayerMask.NameToLayer("ClientOnly"));

        NetMessage.APBResponse response = new NetMessage.APBResponse();
        response.origin = castedMsg.position;
        response.origin.y = 0f;

        float distFromOriginSqrd = (Camera.main.transform.position - response.origin).sqrMagnitude;
        if (distFromOriginSqrd <= GameSettings.APB_RADIUS * GameSettings.APB_RADIUS)
        {
            response.hints.Add(new NetMessage.APBResponse.Hint((mover != null ? mover.transform.position : Camera.main.transform.position), NetMessage.APBResponse.Hint.HintType.Hostage));
        }

        if (outsideTrunk == null)
        {
            Log("Trunk exterior missing!", true);
        }
        else 
        {
            var allDroppedItems = outsideTrunk.GetAllDroppedItems();
            for (int i = 0; i < allDroppedItems.Count; i++)
            {
                response.origin.y = allDroppedItems[i].positionDropped.y;
                distFromOriginSqrd = (allDroppedItems[i].positionDropped - response.origin).sqrMagnitude;
                if (distFromOriginSqrd <= GameSettings.APB_RADIUS * GameSettings.APB_RADIUS)
                {
                    response.hints.Add(new NetMessage.APBResponse.Hint(allDroppedItems[i].positionDropped
                                                                        , allDroppedItems[i].itemName));
                }
            }
        }

#if UNITY_EDITOR
        Debug.DrawLine(Camera.main.transform.position, response.origin, Color.red, 5f);
        Debug.DrawRay(response.origin, Vector3.forward * GameSettings.APB_RADIUS, Color.red, 5f);
        Debug.DrawRay(response.origin, Vector3.back * GameSettings.APB_RADIUS, Color.red, 5f);
        Debug.DrawRay(response.origin, Vector3.left * GameSettings.APB_RADIUS, Color.red, 5f);
        Debug.DrawRay(response.origin, Vector3.right * GameSettings.APB_RADIUS, Color.red, 5f);

        Vector3 prevPos = new Vector3(Mathf.Cos(0f) * GameSettings.APB_RADIUS, 0f, Mathf.Sin(0f) * GameSettings.APB_RADIUS);
        Vector3 nextPos = new Vector3();
        for (float i = 1f; i < 10f; i += 1f)
        {
            nextPos.x = Mathf.Cos((i / 10f) * Mathf.PI * 2f) * GameSettings.APB_RADIUS;
            nextPos.z = Mathf.Sin((i / 10f) * Mathf.PI * 2f) * GameSettings.APB_RADIUS;

            Debug.DrawLine(response.origin + prevPos, response.origin + nextPos, Color.red, 5f, false);

            prevPos = nextPos;
        }
        nextPos.Set(Mathf.Cos(0f), 0f, Mathf.Sin(0f));
        Debug.DrawLine(prevPos, nextPos * GameSettings.APB_RADIUS, Color.red, 5f);
#endif
        msg.conn.Send(NetMessage.ID.APB, response);
    }

    public void Start()
    {
        _instance = this;
    }

    public void Update()
    {
        if (Input.GetButtonUp("Back"))
        {
            Application.Quit();
        }
    }

    public void OnDestroy()
    {
        if (network != null)
        {
            network.Disconnect();
            network = null;
        }

        _instance = null;
    }
}
