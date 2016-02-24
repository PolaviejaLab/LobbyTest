using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class WatchedNetworkDiscovery : NetworkDiscovery
{
    public delegate void OnReceivedBroadcastHandler(object sender, string fromAddress, string data);
    public event OnReceivedBroadcastHandler OnNewServer;
                                
    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        OnNewServer(this, fromAddress, data);
    }
}