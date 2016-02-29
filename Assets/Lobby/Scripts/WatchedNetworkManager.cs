using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

using System.Collections;
using System.Collections.Generic;

public class ServerConnectEvent: UnityEvent<NetworkConnection>
{
}

public class ServerDisconnectEvent: UnityEvent<NetworkConnection>
{
}

public class WatchedNetworkManager : NetworkManager 
{
    public ServerConnectEvent ServerConnect = new ServerConnectEvent();
    public ServerDisconnectEvent ServerDisconnect = new ServerDisconnectEvent();

    public List<NetworkConnection> connections = new List<NetworkConnection>();

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);

        connections.Add(conn);
        ServerConnect.Invoke(conn);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        connections.Remove(conn);
        ServerDisconnect.Invoke(conn);

        base.OnServerDisconnect(conn);
    }
}
