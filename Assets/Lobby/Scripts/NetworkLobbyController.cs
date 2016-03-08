using UnityEngine;
using UnityEngine.Networking;

using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class ClientList : List<string> { }


public class NetworkLobbyController : NetworkBehaviour
{
    public ListBox clientList;
    private WatchedNetworkManager networkManager;

    private Dictionary<int, string> caption_map = new Dictionary<int, string>();

    [Command]
    void CmdSetClientName(int connectionId, string name)
    {
        Debug.Log("Set client name...");

        if(caption_map[connectionId] == name)
            return;

        caption_map[connectionId] = name;
        SendClientListUpdate();
    }


    [ClientRpc]
    void RpcUpdateClientList(string[] addresses, string[] captions)
    {
        Debug.Log("Updating client list, count: " + addresses.Length.ToString());
        clientList.items.Clear();
        for(var i = 0; i < captions.Length; i++) {
            Debug.Log(">>" + addresses[i]);
            clientList.items.Add(addresses[i], captions[i]);
        }
    }


    void SendClientListUpdate()
    {
        if(networkManager == null || clientList == null) return;
        if(!isServer) { return; }

        var numItems = networkManager.connections.Count;

        string[] addresses = new string[numItems];
        string[] captions = new string[numItems];

        for(var i = 0; i < numItems; i++) {
            NetworkConnection connection = networkManager.connections[i];

            addresses[i] = connection.address;

            if(caption_map.ContainsKey(connection.connectionId))
                captions[i] = caption_map[connection.connectionId];
            else
                captions[i] = "Unidentified client";
        }

        Debug.Log("Sending client list: " + addresses.Length.ToString());
        RpcUpdateClientList(addresses, captions);
    }

	
	void Awake() 
    {
        if(clientList == null) {
            Debug.LogWarning("clientList is not assigned.");
            return;
        }

        GameObject networkManagerObject = GameObject.Find("NetworkManager");

        if(networkManagerObject == null) {
            Debug.Log("Could not find NetworkManager object.");
            return;
        }

        networkManager = networkManagerObject.GetComponent<WatchedNetworkManager>();

        if(networkManager == null) {
            Debug.Log("Could not find WatchedNetworkManager component.");
            return;
        }

        networkManager.ServerConnect.AddListener((NetworkConnection conn) => SendClientListUpdate());
        networkManager.ServerDisconnect.AddListener((NetworkConnection conn) => SendClientListUpdate());
        networkManager.ServerReady.AddListener((NetworkConnection conn) => SendClientListUpdate());

        networkManager.ClientConnect.AddListener((NetworkConnection conn) => { 
            Debug.Log("Client connected...");
            CmdSetClientName(conn.connectionId, "Client " + conn.connectionId.ToString());
        });
    }

    void Start()
    {
        if(isServer)
            SendClientListUpdate();
    }
	

	void Update () 
    {
	
	}
}
