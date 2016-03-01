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


    [ClientRpc]
    void RpcUpdateClientList(string[] addresses, string[] captions)
    {
        clientList.items.Clear();
        for(var i = 0; i < captions.Length; i++) {
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
            captions[i] = connection.address;
        }

        RpcUpdateClientList(addresses, captions);
    }

	
	void Start() 
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

        SendClientListUpdate();
	}
	

	void Update () 
    {
	
	}
}
