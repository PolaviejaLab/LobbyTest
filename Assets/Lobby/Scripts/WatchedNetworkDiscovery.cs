using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

using System;
using System.Collections;

[Serializable]
public class NewServerEvent: UnityEvent<string, string>
{
}

public class WatchedNetworkDiscovery : NetworkDiscovery
{
    [SerializeField]
    public NewServerEvent OnNewServer = new NewServerEvent();
                                
    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        Debug.Log("Got server: " + fromAddress);
        OnNewServer.Invoke(fromAddress, data);
    }   
}