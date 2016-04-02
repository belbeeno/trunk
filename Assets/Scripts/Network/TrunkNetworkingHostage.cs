using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(TrunkNetworkDiscovery))]
public class TrunkNetworkingHostage : TrunkNetworkingBase
{
    TrunkNetworkDiscovery broadcaster = null;
    NetworkClient network = null;
    
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

    public void OnDestroy()
    {
        if (network != null)
        {
            network.Disconnect();
            network = null;
        }
    }
}
