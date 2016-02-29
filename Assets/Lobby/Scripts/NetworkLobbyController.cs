using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class NetworkLobbyController : MonoBehaviour 
{
    public ListBox clientList;
    private WatchedNetworkManager networkManager;


    void RefreshClientList()
    {
        if(networkManager == null || clientList == null) return;

        clientList.items.Clear();
        foreach(var item in networkManager.connections) {
            clientList.items.Add(item.address, item.address);
        }
    }

	
	void Start() 
    {
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

        networkManager.ServerConnect.AddListener((NetworkConnection conn) => RefreshClientList());
        networkManager.ServerDisconnect.AddListener((NetworkConnection conn) => RefreshClientList());
        RefreshClientList();
	}
	

	void Update () 
    {
	
	}
}
