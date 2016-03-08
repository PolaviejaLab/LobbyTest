using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class NewServerEvent: UnityEvent<string, string> { }


public struct DiscoveredServer {
    public string Address;
    public string Data;

    public DateTime Timestamp;
}


public class WatchedNetworkDiscovery : NetworkDiscovery
{
    public Dictionary<string, DiscoveredServer> servers = new Dictionary<string, DiscoveredServer>();

    [SerializeField]
    public NewServerEvent OnNewServer = new NewServerEvent();


    /**
     * Removes servers which haven't been seen for the last 5
     * broadcastIntervals. Returns whether servers have been
     * removed.
     */
    private bool PurgeOldServers()
    {
        bool hasChanged = false;

        foreach(var item in servers) {
            var offset = item.Value.Timestamp - DateTime.Today;

            if(offset.Milliseconds > 5 * broadcastInterval) {
                servers.Remove(item.Key);
                hasChanged = true;
            }
        }

        return hasChanged;
    }


    /**
     * Called when a broadcast is received, updates the list of
     * available servers and invokes the OnNewServer callback.
     */
    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        // Update information on current broadcast
        if(servers.ContainsKey(fromAddress)) {
            DiscoveredServer server = servers[fromAddress];
            server.Data = data;
            server.Timestamp = DateTime.Today;
        } else {
            DiscoveredServer server = new DiscoveredServer();
            server.Address = fromAddress;
            server.Data = data;
            server.Timestamp = DateTime.Today;
        }

        PurgeOldServers();

        OnNewServer.Invoke(fromAddress, data);
    }
}