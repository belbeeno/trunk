using UnityEngine;
using UnityEngine.Networking;

public class TrunkNetworkDiscovery : NetworkDiscovery
{
    public delegate void OnConnectionFound(string ip);
    public OnConnectionFound OnConnectionFoundCB = null;

    public bool StartAsClient(OnConnectionFound cb)
    {
        if (!isClient)
        {
            if (!StartAsClient())
            {
                return false;
            }
        }

        OnConnectionFoundCB += cb;
        return true;
    }

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        if (OnConnectionFoundCB == null) return;

        Debug.Log("HostID: " + hostId + ", fromAddress: " + fromAddress + ", data: " + data);
        base.OnReceivedBroadcast(fromAddress, data);
        OnConnectionFoundCB.Invoke(fromAddress);

        OnConnectionFoundCB = null;
    }

    public void OnDestroy()
    {
        if (running)
        {
            StopBroadcast();
        }
    }
}
