using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

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
        network.RegisterHandler(MsgType.Connect, OnConnect);
        network.RegisterHandler(MsgType.Disconnect, OnDisconnect);
        network.RegisterHandler(NetMessage.ID.Ping, OnPing);
        network.RegisterHandler(NetMessage.ID.APB, OnAPBRequest);
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

    public void OnAPBRequest(NetworkMessage msg)
    {
        NetMessage.APBRequest castedMsg = msg.ReadMessage<NetMessage.APBRequest>();
        Log("APB requested at position " + castedMsg.position.ToString());

        // Will be needed for hints.
        //Physics.CheckSphere(castedMsg.position, GameSettings.APB_RADIUS, LayerMask.NameToLayer("ClientOnly"));

        NetMessage.APBResponse response = new NetMessage.APBResponse();
        response.origin = castedMsg.position;
#if UNITY_EDITOR
        Debug.DrawLine(Camera.main.transform.position, response.origin, Color.red, 5f);
        Debug.DrawRay(response.origin, Vector3.forward * GameSettings.APB_RADIUS, Color.red, 5f);
        Debug.DrawRay(response.origin, Vector3.back * GameSettings.APB_RADIUS, Color.red, 5f);
        Debug.DrawRay(response.origin, Vector3.left * GameSettings.APB_RADIUS, Color.red, 5f);
        Debug.DrawRay(response.origin, Vector3.right * GameSettings.APB_RADIUS, Color.red, 5f);
#endif
        float distFromOrigin = (Camera.main.transform.position - response.origin).sqrMagnitude;
        if (distFromOrigin <= GameSettings.APB_RADIUS * GameSettings.APB_RADIUS)
        {
            response.hints.Add(new NetMessage.APBResponse.Hint((mover != null ? mover.transform.position : Camera.main.transform.position), NetMessage.APBResponse.Hint.HintType.Hostage));
        }


        msg.conn.Send(NetMessage.ID.APB, response);
    }

    public void Start()
    {
        _instance = this;
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
