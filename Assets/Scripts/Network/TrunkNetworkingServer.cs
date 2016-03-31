using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;

public class TrunkNetworkingServer : NetworkServerSimple
{
    public override void Initialize()
    {
        base.Initialize();
    }
    public override void OnConnected(NetworkConnection conn)
    {
        base.OnConnected(conn);
    }
    public override void OnConnectError(int connectionId, byte error)
    {
        base.OnConnectError(connectionId, error);
    }
    public override void OnData(NetworkConnection conn, int receivedSize, int channelId)
    {
        base.OnData(conn, receivedSize, channelId);
    }
    public override void OnDataError(NetworkConnection conn, byte error)
    {
        base.OnDataError(conn, error);
    }
    public override void OnDisconnected(NetworkConnection conn)
    {
        base.OnDisconnected(conn);
    }
    public override void OnDisconnectError(NetworkConnection conn, byte error)
    {
        base.OnDisconnectError(conn, error);
    }
}
