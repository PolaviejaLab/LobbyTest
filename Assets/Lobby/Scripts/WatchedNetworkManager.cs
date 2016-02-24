using UnityEngine;
using UnityEngine.Networking;

using System.Collections;
using System.Collections.Generic;

public class WatchedNetworkManager : NetworkManager 
{
    public delegate void ServerConnectHandler(NetworkConnection conn);
    public delegate void ServerDisconnectHandler(NetworkConnection conn);

    public event ServerConnectHandler ServerConnect;
    public event ServerDisconnectHandler ServerDisconnect;

    public List<NetworkConnection> connections = new List<NetworkConnection>();

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);

        connections.Add(conn);
        if(ServerConnect != null)
            ServerConnect(conn);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        connections.Remove(conn);
        if(ServerDisconnect != null)
            ServerDisconnect(conn);

        base.OnServerDisconnect(conn);
    }
}
